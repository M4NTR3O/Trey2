using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;

namespace Trey2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : Controller
    {
        [HttpPost]
        [Route("upload-training")]
        public async Task<IActionResult> UploadToPython(IFormFile file, [FromForm] Training training, IDatabaseFactory factory)
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
            Console.WriteLine(json);
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
            if (isValid)
            {
                try
                {
                    training_python.comment_sportsmen = training.comment_sportsmen != null? training.comment_sportsmen: null;
                    training_python.stress_score = training.stress_score;
                    training_python.feeling_score = training.feeling_score;
                    training_python.type_training_id = training.type_training_id;
                    training_python.name_training = training.name_training;
                    training_python.coach_creator = training.coach_creator != null ? training.coach_creator : null;
                    training_python.planned_training = training.planned_training;
                    training_python.sportsmen_id = training.sportsmen_id;
                    training_python.sport_id = training.sport_id;
                    Console.WriteLine("Загрузка в БД");
                    //Загрузка на сервер итоговая
                    await ICreateFunction.createTraining(training_python, factory);
                }
                catch (Exception ex)
                {
                    return new StatusCodeResult(500);
                }
            }
            else
            {
                return new BadRequestResult();
            }
            return Ok(training_python);
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> GetFullTraining(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return new BadRequestObjectResult("ID являентся не натуральным числом");
            }
            var training = await IReadFunction.GetFullTraining(id, factory);
            if (training == null)
            {
                return new BadRequestObjectResult("Подан ошибочный id");
            }
            else
            {
                return new ObjectResult(training);
            }
        }

        /*[HttpGet]
        [Route("GetLow")]
        public async Task<IActionResult> GetLowTrainings(DateOnly start, DateOnly end, IDatabaseFactory factory)
        {
            if (end < start)
            {
                return new BadRequestObjectResult("Неправильно задан диапазон времени");
            }
            var trainings = await IReadFunction.GetLowTrainings(start, end, factory);
            if (trainings == null)
            {
                return new BadRequestObjectResult("Нет данных");
            }
            else
            {
                return new ObjectResult(trainings);
            }
            return View();
        }*/

        [HttpGet]
        [Route("GetMid/{id}")]
        public async Task<IActionResult> GetMidTraining(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return new BadRequestObjectResult("ID являентся не натуральным числом");
            }
            var training = await IReadFunction.GetMidTraining(id, factory);
            if (training == null)
            {
                return new BadRequestObjectResult("Подан ошибочный id");
            }
            else
            {
                return new ObjectResult(training);
            }
        }
    }
}
