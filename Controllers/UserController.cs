using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;

namespace Trey2.Controllers
{
    /// <summary>
    /// A controller for interacting with the "User" model
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns>Возвращает данные созданного пользователя</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/UserController/Create
        ///     {
        ///         "id": 0,
        ///         "user_surname": "Иванов",
        ///         "user_name": "Иван",
        ///         "user_fathername": "Иванович",
        ///         "user_email": "ivanov_ivan_ivanovich@mail.ru",
        ///         "user_nickname": "triplei",
        ///         "password": "qwerty",
        ///         "user_birthday": "2000-01-01",
        ///         "usert_gender": true,
        ///         "user_height": 1.85,
        ///         "user_weight": 75.65,
        ///         "user_only_coach": false,
        ///         "user_hr_rest": 60,
        ///         "user_hr_max": 200,
        ///         "user_pace_max": null
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Возвращает данные созданного пользователя</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        // POST: UserController/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Create")]
        [ProducesResponseType(typeof(Models.User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        async public Task<ActionResult> Create(Models.User _user, IDatabaseFactory factory)
        {
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    await ICreateFunction.createUser(_user, factory);
                    return new ObjectResult(_user);
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
            else
            {
                return new BadRequestObjectResult("No valid Data");
            }
        }

        /// <summary>
        /// Get information about user from his ID
        /// </summary>
        /// <param name="id">ID user</param>
        /// <returns>Возвращает информацию о пользователе по его ID</returns>
        /// <response code="200">Возвращает данные пользователя</response>
        /// <response code="400">ID является не натуральным числом; Подан ошибочный id</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("Get/{id}")]
        async public Task<ActionResult> Select(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return new BadRequestObjectResult("ID являентся не натуральным числом");
            }
            var user = await IReadFunction.GetUser(id, factory);
            if (user == null)
            {
                return new BadRequestObjectResult("Подан ошибочный id");
            }
            else
            {
                return new ObjectResult(user);
            }
        }

        /// <summary>
        /// Get information about user from his ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/UserController/Auth
        ///     {
        ///         "login": "ivanov_ivan_ivanovich@mail.ru",
        ///         "password": "qwerty"
        ///     }
        ///     
        /// </remarks>
        /// <returns>Возвращает информацию о пользователе по его ID</returns>
        /// <response code="200">Возвращает данные пользователя</response>
        /// <response code="400">Нет пользователя; Неверный пароль</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("Auth")]
        async public Task<ActionResult> Auth(AuthRequest request, IDatabaseFactory factory)
        {
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    (ulong id, bool read) = await IReadFunction.GetAuthEmail(request, factory);
                    if (id == 0 && (!read))
                    {
                        return new BadRequestObjectResult("Нет пользователя");
                    }
                    else if (id == 0)
                    {
                        return new BadRequestObjectResult("Неверный пароль");
                    }
                    else
                    {
                        var user = (await IReadFunction.GetUser(id, factory));
                        return new ObjectResult(user);
                    }
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.ToString());
                }
            }
            else
            {
                return new BadRequestResult();
            }
            
        }

        [HttpGet]
        [Route("UserZones/{id}")]
        async public Task<ActionResult> GetAllUserZones(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return new BadRequestObjectResult("ID являентся не натуральным числом");
            }
            var user_zones = await IReadFunction.GetAllUserZones(id, factory);
            if (user_zones == null)
            {
                return new BadRequestObjectResult("Подан ошибочный id");
            }
            else
            {
                return new ObjectResult(user_zones);
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        async public Task<ActionResult> DeleteUser(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return new BadRequestObjectResult("ID являентся не натуральным числом");
            }
            await IDeleteFunction.DeleteUser(id, factory);
            return new OkResult();
        }

        [HttpPut]
        [Route("Update")]
        async public Task<ActionResult> UpdateUser(User user, IDatabaseFactory factory)
        {
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    await IUpdateFunction.UpdateUser(user, factory);
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
            else
            {
                return new BadRequestResult();
            }
        }


        [HttpPatch]
        [Route("ChangePassword/{id}")]
        public async Task<IActionResult> PatchPassword(ulong id, string last_password, string new_password, IDatabaseFactory factory)
        {
            var result = await IPatchFunction.PatchPassword(id, last_password, new_password, factory);
            if (!result)
                return BadRequest("Invalid credentials");

            return Ok();
        }
    }
}
