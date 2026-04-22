using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;
using Trey2.ViewModel;
using Trey2.ViewModel.Report;
using Trey2.ViewModels;

namespace Trey2.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : ControllerBase
    {

        /// <summary>
        /// Upload training files on server and return data about it
        /// </summary>
        /// <param name="file">File *.fit, *.tcx, *.gpx</param>
        /// <param name="training">Training</param>
        /// <param name="factory"></param>
        /// <returns>Загружает тренировочную сессию на сервер и возвращает данные о ней</returns>
        /// <response code="201">Создаёт тренировку и возвращает данные о ней</response>
        /// <response code="400">Данные не валидны</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpPost]
        [Route("upload-training")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadToPython([Required] IFormFile file, [FromForm] TrainingCreate training, IDatabaseFactory factory, IHttpClientFactory clientFactory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            using var client = clientFactory.CreateClient("python");

            using var content = new MultipartFormDataContent();
            await using var fs = file.OpenReadStream();

            content.Add(
                new StreamContent(fs),
                "file",
                file.FileName
            );
            Console.WriteLine($"Отправка запроса на Python-Server {DateTime.UtcNow}");
            var response = await client.PostAsync(
                "/convert",
                content
            );
            Console.WriteLine($"Получен ответ с Python-Server {DateTime.UtcNow}");
            var json = await response.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            opts.Converters.Add(new PlainDateTimeConverter());
            var training_python = JsonSerializer.Deserialize<Training>(
                json,
                opts
            );
            Console.WriteLine($"Обработан JSON {DateTime.UtcNow}");
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            Console.WriteLine($"Валидность данных - {isValid}");
            ulong id;
            if (isValid)
            {
                try
                {
                    training_python.comment_sportsman = training.comment_sportsman != null? training.comment_sportsman: null;
                    training_python.stress_score = training.stress_score;
                    training_python.feeling_score = training.feeling_score;
                    training_python.type_training_id = training.type_training_id;
                    training_python.name_training = training.name_training;
                    training_python.coach_creator = training.coach_creator != null ? training.coach_creator : null;
                    training_python.status_training = training.status_training;
                    training_python.sportsman_id = training.sportsman_id;
                    training_python.sport_id = training.sport_id;
                    Console.WriteLine("Загрузка в БД");
                    //Загрузка на сервер итоговая
                    id = await ICreateFunction.createTraining(training_python, factory);
                    //Подготовить данные для отображения! Функция перевода данных в TrainingView (имеющихся) и вызов функции
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid data");
            }           
            var (view_training, points) = await IReadFunction.GetFullTraining(id, id_user, factory);
            TrainingGraphData data = new TrainingGraphData { TimeStartDt = view_training.time_start, trackpoints = points, is_gps = view_training.is_gps };
            var options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            byte[] bytes = MessagePackSerializer.Serialize(data, options);
            var content_second = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-msgpack");
            var response_second = await client.PostAsync(
                "/data_to_graph",
                content_second
            );
            //Обработка результатов
            var result = await response_second.Content.ReadFromJsonAsync<GraphModel>();
            view_training.powers = result.GetPowers();
            view_training.heart_rates = result.GetHeart_rates();
            view_training.trackpoints = result.GetTrackpoints();
            view_training.paces = result.GetPaces();
            view_training.cadences = result.GetCadences();
            view_training.pauses = result.pauses;
            view_training.altitudes = result.GetAltitudes();
            return Ok(view_training);
        }

        /// <summary>
        /// Create training without file
        /// </summary>
        /// <param name="training">training create model</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("CreateTraining")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateWithoutFile(TrainingCreate training, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            if (training.local_timestamp.ToUniversalTime() > DateTime.UtcNow)
            {
                return BadRequest("Planning time not too late yet");
            }
            Console.WriteLine($"Валидность данных - {isValid}");
            ulong id;
            if (isValid)
            {
                try
                {
                    //Загрузка на сервер итоговая
                    id = await ICreateFunction.createTrainingWithoutFile(training, factory);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid data");
            }
            //Функция переход с TrainigCreate в TrainingView
            var view_training = await IReadFunction.GetTrainingCreateWithoutFiles(id, factory);
            return Ok(view_training);
        }

        /// <summary>
        /// Create plan of training by sportsman
        /// </summary>
        /// <param name="training">training plan. params:
        /// sportsman_id = null, local_timestamp (format: 2026-01-01T00:00:00+07:00), sport_id, type_training_id, name_training, description, duration, distance (may be null), avg_heart_rate (may be null), avg_speed (may be null), coach_creator = null, status_training_id = 2</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("CreatePlannedTrainingBySportsman")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlannedTrainingSportsman(TrainingCreatePlan training, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            training.sportsman_id = id_user;
            training.coach_creator = id_user;
            TrainingCreate trainingCreate;
            if (training.local_timestamp.UtcDateTime <= DateTime.UtcNow)
            {
                return BadRequest("Planning time already passed");
            }
            try
            {
                trainingCreate = new TrainingCreate(training);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            Console.WriteLine($"Валидность данных - {isValid}");
            ulong id;
            if (isValid)
            {
                try
                {
                    //Загрузка на сервер итоговая
                    id = await ICreateFunction.createPlannedTraining(trainingCreate, factory);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid data");
            }
            //Функция переход с TrainigCreate в TrainingView
            var view_training = await IReadFunction.GetTrainingCreatePlanned(id, factory);
            return Ok(view_training);
        }

        /// <summary>
        /// create plan of training by coach
        /// </summary>
        /// <param name="training">training plan. params:
        /// sportsman_id, local_timestamp (format: 2026-01-01T00:00:00+07:00), sport_id, type_training_id, name_training, description, duration, distance (may be null), avg_heart_rate (may be null), avg_speed (may be null), coach_creator = null, status_training_id = 2</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("CreatePlannedTrainingByCoach")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlannedTrainingCoach(TrainingCreatePlan training, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            training.coach_creator = id_user;
            if (training.local_timestamp.UtcDateTime <= DateTime.UtcNow)
            {
                return BadRequest("Planning time already passed");
            }
            TrainingCreate trainingCreate;
            try
            {
                trainingCreate = new TrainingCreate(training);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            Console.WriteLine($"Валидность данных - {isValid}");
            ulong id;
            if (isValid)
            {
                try
                {
                    //Загрузка на сервер итоговая
                    id = await ICreateFunction.createPlannedTraining(trainingCreate, factory);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid data");
            }
            //Функция переход с TrainigCreate в TrainingView
            var view_training = await IReadFunction.GetTrainingCreatePlanned(id, factory);
            return Ok(view_training);
        }

        /// <summary>
        /// Get full data of training
        /// </summary>
        /// <param name="id">ID Training</param>
        /// <returns>Возвращает полную информацию о тренировочной активности</returns>
        /// <response code="200">Возвращает данные о тренировочной сессии</response>
        /// <response code="400">ID является не натуральным числом; Подан ошибочный id</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Authorize]
        [Route("Get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFullTraining(ulong id, IDatabaseFactory factory, IHttpClientFactory clientFactory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var (training, points) = await IReadFunction.GetFullTraining(id, id_user, factory);
            if (training == null)
            {
                return BadRequest("Подан ошибочный id");
            }
            else
            {
                if (points == null || points.Count == 0)
                {
                    return Ok(training);
                }
                try
                {
                    //Отправляем запрос на преобразование данных
                    using var client = clientFactory.CreateClient("python");
                    TrainingGraphData data = new TrainingGraphData { TimeStartDt = training.time_start, trackpoints = points, is_gps = training.is_gps };
                    var options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
                    byte[] bytes = MessagePackSerializer.Serialize(data, options);
                    var content = new ByteArrayContent(bytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-msgpack");
                    var response = await client.PostAsync(
                        "/data_to_graph",
                        content
                    );
                    //Обработка результатов
                    var result = await response.Content.ReadFromJsonAsync<GraphModel>();
                    training.powers = result.GetPowers();
                    training.heart_rates = result.GetHeart_rates();
                    training.trackpoints = result.GetTrackpoints();
                    training.paces = result.GetPaces();
                    training.cadences = result.GetCadences();
                    training.pauses = result.pauses;
                    training.altitudes = result.GetAltitudes();
                    return Ok(training);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Get brief data of trainings from start day to end day including it
        /// </summary>
        /// <param name="sportsman_id">ID Sportsman</param>
        /// <param name="start">The start date of the segment including id</param>
        /// <param name="end">The end sate of the segment including id</param>
        /// <param name="start_report">The start date for report of the segment including id</param>
        /// <param name="end_report">The end date for report of the segment including id</param>
        /// <param name="factory"></param>
        /// <returns>Возвращает краткую информацию о тренировочных активностях с даты начала по дате конца, включая их</returns>
        /// <response code="200">Возвращает краткие данные о тренировочных сессиях за период</response>
        /// <response code="400">Неправильно задан диапазон времени; Нет данных</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Authorize]
        [Route("GetLow")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CalendarViewer))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLowTrainings(DateTime start, DateTime end, DateTime start_report, DateTime end_report, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (end < start)
            {
                return BadRequest("Неправильно задан диапазон времени");
            }
            var calendarViewer = await IReadFunction.GetPeriodTrainings(id_user, start, end, start_report, end_report, factory);
            if (calendarViewer.trainings == null)
            {
                return BadRequest("Нет данных");
            }
            else
            {
                return Ok(calendarViewer);
            }
        }

        /// <summary>
        /// Get brief data of trainings with track (if it is not null)
        /// </summary>
        /// <param name="id">ID Training</param>
        /// <param name="factory"></param>
        /// <returns>Возвращает краткую информацию о тренировочной активности с треком тренировки (при наличии)</returns>
        /// <response code="200">Возвращает данные о тренировочной сессии с треком тренировки (при наличии)</response>
        /// <response code="400">ID является не натуральным числом; Подан ошибочный id</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Authorize]
        [Route("GetMid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingMid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMidTraining(ulong id, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var training = await IReadFunction.GetMidTraining(id, id_user, factory);
            if (training == null)
            {
                return BadRequest("Подан ошибочный id");
            }
            else
            {
                return Ok(training);
            }
        }

        /// <summary>
        /// Delete information about training from its ID
        /// </summary>
        /// <returns>Удаляет информацию о тренировке по её ID</returns>
        /// <response code="200">Успешно удалены данные</response>
        /// <response code="400">ID является не натуральным числом</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete]
        [Authorize]
        [Route("Delete/{id}")]
        async public Task<ActionResult> DeleteTraining(ulong id, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id < 1)
            {
                return BadRequest("ID являентся не натуральным числом");
            }
            var result = await IDeleteFunction.DeleteTraining(id, id_user, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }


        /// <summary>
        /// Update training params\
        /// (params for update: sport_id, type_training_id, name_training, duration, distance, calories, stress_score, feeling_score, comment_sportsman)
        /// </summary>
        /// <param name="training">Model training</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("Update")]
        async public Task<ActionResult> UpdateTraining(Training training, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    await IUpdateFunction.UpdateTraining(training, id_user, factory);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update planned to completed training params\
        /// (params for update: stress_score, feeling_score, comment_sportsman, file)
        /// </summary>
        /// <param name="training">Model training</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("UpdatePlanToCompleteFile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePlanToCompleteWithFileTraining([Required] IFormFile file, [FromForm] UpdatePlanTraining training, IDatabaseFactory factory, IHttpClientFactory clientFactory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            using var client = clientFactory.CreateClient("python");

            using var content = new MultipartFormDataContent();
            await using var fs = file.OpenReadStream();

            content.Add(
                new StreamContent(fs),
                "file",
                file.FileName
            );
            Console.WriteLine($"Отправка запроса на Python-Server {DateTime.UtcNow}");
            var response = await client.PostAsync(
                "/convert",
                content
            );
            Console.WriteLine($"Получен ответ с Python-Server {DateTime.UtcNow}");
            var json = await response.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            opts.Converters.Add(new PlainDateTimeConverter());
            var training_python = JsonSerializer.Deserialize<Training>(
                json,
                opts
            );
            Console.WriteLine($"Обработан JSON {DateTime.UtcNow}");
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            Console.WriteLine($"Валидность данных - {isValid}");
            ulong id;
            if (isValid)
            {
                try
                {
                    training_python.id = training.id;
                    training_python.comment_sportsman = training.comment_sportsman != null ? training.comment_sportsman : null;
                    training_python.stress_score = training.stress_score;
                    training_python.feeling_score = training.feeling_score;
                    training_python.sportsman_id = id_user;
                    Console.WriteLine("Загрузка в БД");
                    //Загрузка на сервер итоговая
                    await IUpdateFunction.UpdatePlanToCompleteTraining(training_python, id_user, factory);
                    //Подготовить данные для отображения! Функция перевода данных в TrainingView (имеющихся) и вызов функции
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid data");
            }
            var (view_training, points) = await IReadFunction.GetFullTraining(training.id, id_user, factory);
            TrainingGraphData data = new TrainingGraphData { TimeStartDt = view_training.time_start, trackpoints = points, is_gps = view_training.is_gps };
            var options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            byte[] bytes = MessagePackSerializer.Serialize(data, options);
            var content_second = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-msgpack");
            var response_second = await client.PostAsync(
                "/data_to_graph",
                content_second
            );
            //Обработка результатов
            var result = await response_second.Content.ReadFromJsonAsync<GraphModel>();
            view_training.powers = result.GetPowers();
            view_training.heart_rates = result.GetHeart_rates();
            view_training.trackpoints = result.GetTrackpoints();
            view_training.paces = result.GetPaces();
            view_training.cadences = result.GetCadences();
            view_training.pauses = result.pauses;
            view_training.altitudes = result.GetAltitudes();
            return Ok(view_training);
        }

        /// <summary>
        /// Update planned to completed training without file params\
        /// (params for update: stress_score, feeling_score, comment_sportsman, time_start, local_timestamp, duration, distance, ang_hr, avg_speed, calories)
        /// </summary>
        /// <param name="training">Model training</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("UpdatePlanToCompleteWithoutFile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePlanToCompleteWithoutFile(UpdatePlanTrainingWithoutFile training, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            Console.WriteLine($"Валидность данных - {isValid}");
            if (isValid)
            {
                try
                {
                    //Загрузка на сервер итоговая
                    await IUpdateFunction.UpdateTrainingWithoutFile(training, id_user, factory);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid data");
            }
            var view_training = await IReadFunction.GetTrainingCreateWithoutFiles(training.id, factory);
            return Ok(view_training);
        }

        /// <summary>
        /// Change training name\
        /// Изменяет наименование тренировки
        /// </summary>
        /// <param name="id">ID training</param>
        /// <param name="name">New name training MaxLength=100</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введено аналогичное имя либо нет тренировки)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPatch]
        [Authorize]
        [Route("Patch/Name/{id}")]
        async public Task<IActionResult> PatchNameTraining(ulong id, [MaxLength(100)] string name, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IPatchFunction.PatchNameTraining(id, name, id_user, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }

        /// <summary>
        /// Change training stress score\
        /// Изменяет оценку напряжения тренировочной сессии
        /// </summary>
        /// <param name="id">ID training</param>
        /// <param name="score">New stress score</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введена аналогичная оценка либо нет тренировки)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPatch]
        [Authorize]
        [Route("Patch/StressScore/{id}")]
        async public Task<IActionResult> PatchStressScoreTraining(ulong id, [Range(1,10)] byte score, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IPatchFunction.PatchStressScoreTraining(id, score, id_user, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }

        /// <summary>
        /// Change training feeeling score\
        /// Изменяет оценку самочувствия тренировочной сессии
        /// </summary>
        /// <param name="id">ID training</param>
        /// <param name="score">New feeling score</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введена аналогичная оценка либо нет тренировки)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPatch]
        [Authorize]
        [Route("Patch/FeelingScore/{id}")]
        async public Task<IActionResult> PatchFeelingScoreTraining(ulong id, [Range(1, 5)] byte score, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IPatchFunction.PatchFeelingScoreTraining(id, score, id_user, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }


        /// <summary>
        /// Change sportsman comment for training\
        /// Изменяет комментарий спортсмена у тренировки
        /// </summary>
        /// <param name="id">ID training</param>
        /// <param name="comment">New sportsman comment MaxLength=1000</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введён аналогичный комментарий либо нет тренировки)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPatch]
        [Authorize]
        [Route("Patch/CommentSportsman/{id}")]
        async public Task<IActionResult> PatchCommentSportsman(ulong id, [MaxLength(1000)] string? comment, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (comment is null)
            {
                return BadRequest("comment is null");
            }
            var result = await IPatchFunction.PatchCommentSportsmanTraining(id, comment, id_user, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }


        /// <summary>
        /// Change lap caption for training\
        /// Изменяет описание круга для тренировки
        /// </summary>
        /// <param name="id">ID Traininng</param>
        /// <param name="lap_id">Lap id</param>
        /// <param name="caption">New caption lap</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введено аналогичное описание либо нет тренировки/круга)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPatch]
        [Authorize]
        [Route("Patch/LapCaption/{id}")]
        async public Task<IActionResult> PatchLapCaption(ulong id, int lap_id, string? caption, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (caption is null)
            {
                return BadRequest("caption is null");
            }
            var result = await IPatchFunction.PatchLapCaption(lap_id, id, caption, id_user, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }


        /// <summary>
        /// Report data for graphics
        /// </summary>
        /// <param name="start">День начала отчёта</param>
        /// <param name="end">День завершения отчёта (включая его)</param>
        /// <param name="bin">bin infromation, sample bin: "day", "week", "month". Совет: Если меньше недели - день; если меньше месяца: предлагать на выбор: день, неделя; если больше месяца, но меньше полугода - любой, но день странно; больше полугода - неделя/месяц</param>
        /// <param name="factory"></param>
        /// <returns>Возвращает данные для отображения в графиках</returns>
        /// <response code="200">object</response>
        /// <response code="200">Not information OR Error in bin name</response>
        [HttpGet]
        [Authorize]
        [Route("GetReport")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReportPage))]
        async public Task<IActionResult> GetReport(DateTime start, DateTime end, string bin, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IReadFunction.GetReportPage(id_user, start, end, bin, factory);
            if (result is null)
                return Ok("Not information OR Error in bin name");
            return Ok(result);
        }

    }
}
