using MessagePack.Resolvers;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Services;
using Trey2.Services.Interfaces;
using Trey2.ViewModel;
using System.Net.Http.Headers;
using Trey2.ViewModel.Report;

namespace Trey2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoachController : Controller
    {
        private readonly ICoachService _service;
        private readonly ITrainingService _trainingService;

        public CoachController(ICoachService service, ITrainingService trainingService)
        {
            _service = service;
            _trainingService = trainingService;
        }

        /// <summary>
        /// Get list all coaches by sportsman (Roles = "Sportsman")
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/Coaches")]
        [Authorize(Roles = "Sportsman")]
        public async Task<IActionResult> getCoaches()
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                var result = await _service.getCoaches(id_user);
                if (result is null)
                    return BadRequest("Invalid credentials");

                return Ok(result);
            }
            catch( Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get list sportsman by coach (Roles = "Coach")
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/Sportsmen")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> getSportsmen()
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                var result = await _service.getSportsmen(id_user);
                if (result is null)
                    return BadRequest("Invalid credentials");

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// create plan of training by coach (Roles = "Coach")
        /// </summary>
        /// <param name="training">training</param>
        /// <param name="id_sportsman">ID подопечного</param>
        /// sportsman_id, local_timestamp (format: 2026-01-01T00:00:00+07:00), sport_id, type_training_id, name_training, description, duration, distance (may be null), avg_heart_rate (may be null), avg_speed (may be null), coach_creator = null, status_training_id = 2</param>
        /// <returns></returns>
        [Authorize(Roles = "Coach")]
        [HttpPost]
        [Route("CreatePlannedTrainingByCoach")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlannedTrainingCoach(ulong id_sportsman, TrainingCreatePlan training)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                if (!(await _service.CheckRelation(id_sportsman, id_coach)))
                {
                    return Forbid("Отсутсвует доступ к данным");
                }
                training.coach_creator = id_coach;
                training.sportsman_id = id_sportsman;
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
                        id = await _trainingService.createPlannedTraining(trainingCreate);
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
                var view_training = await _trainingService.GetTrainingCreatePlanned(id);
                return Ok(view_training);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get full data of training by coach (Roles = "Coach")
        /// </summary>
        /// <param name="id_training">ID Training</param>
        /// <param name="id_sportsman">ID подопечного</param>
        /// <returns>Возвращает полную информацию о тренировочной активности</returns>
        /// <response code="200">Возвращает данные о тренировочной сессии</response>
        /// <response code="400">ID является не натуральным числом; Подан ошибочный id</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Authorize(Roles = "Coach")]
        [Route("Sportsman/Training/Get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingFull))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFullTraining(ulong id_training, ulong id_sportsman, IHttpClientFactory clientFactory)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (!(await _service.CheckRelation(id_sportsman, id_coach)))
            {
                return Forbid("Отсутсвует доступ к данным");
            }
            if (id_training < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var (training, points) = await _trainingService.GetFullTraining(id_training, id_sportsman);
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
        /// Get brief data of trainings from start day to end day including it by coach (Roles = "Coach")
        /// </summary>
        /// <param name="id_sportsman">ID Sportsman</param>
        /// <param name="start">The start date of the segment including id</param>
        /// <param name="end">The end sate of the segment including id</param>
        /// <param name="start_report">The start date for report of the segment including id</param>
        /// <param name="end_report">The end date for report of the segment including id</param>
        /// <returns>Возвращает краткую информацию о тренировочных активностях с даты начала по дате конца, включая их</returns>
        /// <response code="200">Возвращает краткие данные о тренировочных сессиях за период</response>
        /// <response code="400">Неправильно задан диапазон времени; Нет данных</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Authorize(Roles = "Coach")]
        [Route("Sportsman/Training/GetLow")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CalendarViewer))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLowTrainings(ulong id_sportsman, DateTime start, DateTime end, DateTime start_report, DateTime end_report)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                if (!(await _service.CheckRelation(id_sportsman, id_coach)))
                {
                    return Forbid("Отсутсвует доступ к данным");
                }
                if (end < start)
                {
                    return BadRequest("Неправильно задан диапазон времени");
                }
                var calendarViewer = await _trainingService.GetPeriodTrainings(id_sportsman, start, end, start_report, end_report);
                if (calendarViewer.trainings == null)
                {
                    return BadRequest("Нет данных");
                }
                else
                {
                    return Ok(calendarViewer);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get brief data of trainings with track (if it is not null) by coach (Roles = "Coach")
        /// </summary>
        /// <param name="sportsman_id">ID подопечного</param>
        /// <param name="id_training">ID Training</param>
        /// <returns>Возвращает краткую информацию о тренировочной активности с треком тренировки (при наличии)</returns>
        /// <response code="200">Возвращает данные о тренировочной сессии с треком тренировки (при наличии)</response>
        /// <response code="400">ID является не натуральным числом; Подан ошибочный id</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Authorize(Roles = "Coach")]
        [Route("Sportsman/Training/GetMid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ViewTrainingMid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMidTraining(ulong id_training, ulong sportsman_id)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                if (!(await _service.CheckRelation(sportsman_id, id_coach)))
                {
                    return Forbid("Отсутсвует доступ к данным");
                }
                if (id_training < 1)
                {
                    return BadRequest("ID является не натуральным числом");
                }
                var training = await _trainingService.GetMidTraining(id_training, sportsman_id);
                if (training == null)
                {
                    return BadRequest("Подан ошибочный id");
                }
                else
                {
                    return Ok(training);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        /// <summary>
        /// Report data for graphics by coach (Roles = "Coach")
        /// </summary>
        /// <param name="sportsman_id">ID полопечного</param>
        /// <param name="start">День начала отчёта</param>
        /// <param name="end">День завершения отчёта (включая его)</param>
        /// <param name="bin">bin infromation, sample bin: "day", "week", "month". Совет: Если меньше недели - день; если меньше месяца: предлагать на выбор: день, неделя; если больше месяца, но меньше полугода - любой, но день странно; больше полугода - неделя/месяц</param>
        /// <param name="factory"></param>
        /// <returns>Возвращает данные для отображения в графиках</returns>
        /// <response code="200">object</response>
        /// <response code="200">Not information OR Error in bin name</response>
        [HttpGet]
        [Authorize(Roles = "Coach")]
        [Route("Sportsman/GetReport")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReportPage))]
        async public Task<IActionResult> GetReport(ulong sportsman_id, DateTime start, DateTime end, string bin)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                if (!(await _service.CheckRelation(sportsman_id, id_coach)))
                {
                    return Forbid("Отсутсвует доступ к данным");
                }
                var result = await _trainingService.GetReportPage(sportsman_id, start, end, bin);
                if (result is null)
                    return Ok("Not information OR Error in bin name");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create invite coach by sportsman
        /// </summary>
        /// <param name="id_coach"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("CreateInvite")]
        async public Task<IActionResult> CreateInvite(ulong id_coach)
        {
            ulong id_sportsman = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                if ((!await _service.CheckRelation(id_sportsman, id_coach)) && (!await _service.CheckPendingInvite(id_sportsman, id_coach)))
                {
                    await _service.CreateInvite(id_sportsman, id_coach);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get invites for user 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("Invites")]
        async public Task<IActionResult> GetInvites()
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                var result = await _service.GetInvites(id_user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Change status invite on accepted by coach (Roles = "Coach")
        /// </summary>
        /// <param name="id_invite"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Coach")]
        [Route("AcceptedInvite")]
        async public Task<IActionResult> AcceptedInvite(ulong id_invite)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _service.AcceptedInvite(id_invite);
                return Ok();
            }
            catch(Exception ex)
            {
            return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Change status invite on rejected by coach (Roles = "Coach")
        /// </summary>
        /// <param name="id_invite"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Coach")]
        [Route("RejectedInvite")]
        async public Task<IActionResult> RejectedInvite(ulong id_invite)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _service.RejectedInvite(id_invite);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Change status on cancelled by sportsman (Roles = "Sportsman")
        /// </summary>
        /// <param name="id_invite"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Sportsman")]
        [Route("CanceledInvite")]
        async public Task<IActionResult> CanceledInvite(ulong id_invite)
        {
            ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            try
            {
                await _service.CanceledInvite(id_invite);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove Invite
        /// </summary>
        /// <param name="id_relation"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("RemoveRelation")]
        async public Task<IActionResult> RemoveRelation(ulong id_relation)
        {
            try
            {
                ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _service.RemoveRelation(id_relation, id_user);
                if (result)
                {
                    return Ok("removed relation");
                }
                else return BadRequest("Don't find relation");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "Coach")]
        [Route("Training/PatchComment")]
        async public Task<IActionResult> PatchCoachComment(ulong id_training, string comment)
        {
            try
            {
                ulong id_coach = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _service.PatchCommentCoach(id_coach, id_training, comment);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
