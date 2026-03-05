using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;
using Trey2.ViewModel;

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
        /// <remarks>
        /// Need fill data marked with "/!", data that is not required is marked with "/*", fill in the remaining data as in the example  
        /// Sample request:
        /// 
        ///     POST api/TrainingController/upload-training
        ///     {
        ///         "id": 0,
        ///         "sportsman_id": 1, /!
        ///         "sport_id": 1, /!
        ///         "type_training": 1, /!
        ///         "name_training": "Контрольный бег (3 км)", /*
        ///         "time_start": "2026-02-11T07:32:31.916Z",
        ///         "local_timestamp": "2026-02-11T07:32:31.916Z",
        ///         "duration": 0,
        ///         "distance": 0,
        ///         "avg_heart_rate": 0,
        ///         "min_heart_rate": 0,
        ///         "max_heart_rate": 0,
        ///         "avg_speed": 0,
        ///         "max_speed": 0,
        ///         "avg_running_cadence": 0,
        ///         "max_running_cadence": 0,
        ///         "avg_power": 0,
        ///         "max_power": 0,
        ///         "total_descent": 0,
        ///         "total_ascent": 0,
        ///         "start_position_lat": 0,
        ///         "start_position_long": 0,
        ///         "max_altituide": 0,
        ///         "calories": 0,
        ///         "is_gps": false,
        ///         "stress_score": 9, /*
        ///         "feeling_score": 3, /*
        ///         "comment_portsman": "Hard working", /*
        ///         "coach_creator": null, /*
        ///         "comment_coach": null, 
        ///         "status_training": 1,
        ///         "laps": null,
        ///         "trackpoints": null
        ///     }
        ///     
        /// </remarks>
        /// <returns>Загружает тренировочную сессию на сервер и возвращает данные о ней</returns>
        /// <response code="201">Создаёт тренировку и возвращает данные о ней</response>
        /// <response code="400">Данные не валидны</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("upload-training")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadToPython([Required] IFormFile file, [FromForm] TrainingCreate training, IDatabaseFactory factory)
        {
            using var client = new HttpClient();

            using var content = new MultipartFormDataContent();
            await using var fs = file.OpenReadStream();

            content.Add(
                new StreamContent(fs),
                "file",
                file.FileName
            );
            Console.WriteLine($"Отправка запроса на Python-Server {DateTime.UtcNow}");
            var response = await client.PostAsync(
                "http://localhost:8001/convert",
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
            return Ok(await IReadFunction.GetFullTraining(id, factory));
        }


        [HttpPost]
        [Route("CreateTraining")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateWithoutFile(TrainingCreate training, IDatabaseFactory factory)
        {
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
                    await ICreateFunction.createTrainingWithoutFile(training, factory);
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
            return Ok(training);
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
        [Route("Get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTraining))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFullTraining(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var training = await IReadFunction.GetFullTraining(id, factory);
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
        /// Get brief data of trainings from start day to end day including it
        /// </summary>
        /// <param name="sportsman_id">ID Sportsman</param>
        /// <param name="start">The start date of the segment including id</param>
        /// <param name="end">The start end of the segment including id</param>
        /// <param name="factory"></param>
        /// <returns>Возвращает краткую информацию о тренировочных активностях с даты начала по дате конца, включая их</returns>
        /// <response code="200">Возвращает краткие данные о тренировочных сессиях за период</response>
        /// <response code="400">Неправильно задан диапазон времени; Нет данных</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("GetLow")]
        public async Task<IActionResult> GetLowTrainings(ulong sportsman_id, DateTime start, DateTime end, IDatabaseFactory factory)
        {
            if (end < start)
            {
                return BadRequest("Неправильно задан диапазон времени");
            }
            var trainings = await IReadFunction.GetPeriodTrainings(sportsman_id, start, end, factory);
            if (trainings == null)
            {
                return BadRequest("Нет данных");
            }
            else
            {
                return Ok(trainings);
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
        [Route("GetMid/{id}")]
        public async Task<IActionResult> GetMidTraining(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return BadRequest("ID являентся не натуральным числом");
            }
            var training = await IReadFunction.GetMidTraining(id, factory);
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
        [Route("Delete/{id}")]
        async public Task<ActionResult> DeleteTraining(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return BadRequest("ID являентся не натуральным числом");
            }
            var result = await IDeleteFunction.DeleteTraining(id, factory);
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
        [Route("Update")]
        async public Task<ActionResult> UpdateTraining(Training training, IDatabaseFactory factory)
        {
            var validationContext = new ValidationContext(training);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(training, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    await IUpdateFunction.UpdateTraining(training, factory);
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
        [Route("Patch/Name/{id}")]
        async public Task<IActionResult> PatchNameTraining(ulong id, [MaxLength(100)] string name, IDatabaseFactory factory)
        {
            var result = await IPatchFunction.PatchNameTraining(id, name, factory);
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
        [Route("Patch/StressScore/{id}")]
        async public Task<IActionResult> PatchStressScoreTraining(ulong id, [Range(1,10)] byte score, IDatabaseFactory factory)
        {
            var result = await IPatchFunction.PatchStressScoreTraining(id, score, factory);
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
        [Route("Patch/FeelingScore/{id}")]
        async public Task<IActionResult> PatchFeelingScoreTraining(ulong id, [Range(1, 5)] byte score, IDatabaseFactory factory)
        {
            var result = await IPatchFunction.PatchFeelingScoreTraining(id, score, factory);
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
        [Route("Patch/CommentSportsman/{id}")]
        async public Task<IActionResult> PatchCommentSportsman(ulong id, [MaxLength(1000)] string? comment, IDatabaseFactory factory)
        {
            if (comment is null)
            {
                return BadRequest("comment is null");
            }
            var result = await IPatchFunction.PatchCommentSportsmanTraining(id, comment, factory);
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
        [Route("Patch/LapCaption/{id}")]
        async public Task<IActionResult> PatchLapCaption(ulong id, int lap_id, string? caption, IDatabaseFactory factory)
        {
            if (caption is null)
            {
                return BadRequest("caption is null");
            }
            var result = await IPatchFunction.PatchLapCaption(lap_id, id, caption, factory);
            if (!result)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }
    }
}
