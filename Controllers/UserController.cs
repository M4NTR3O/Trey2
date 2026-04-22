using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;
using Trey2.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Trey2.Controllers
{
    /// <summary>
    /// A controller for interacting with the "User" model
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly JwtService _jwtService;
        public UserController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }
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
        ///         "user_gender": true,
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
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("No valid Data");
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
        [Authorize]
        [HttpGet]
        [Route("Get")]
        async public Task<ActionResult> Select(IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id_user < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var user = await IReadFunction.GetUser(id_user, factory);
            if (user == null)
            {
                return BadRequest("Подан ошибочный id");
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
                    var result = await IReadFunction.GetAuth(request, factory);
                    if (!result.flag)
                    {
                        return Unauthorized("Invalid credentials");
                    }

                    var access = _jwtService.GenerateAccessToken(result.user.id, result.user.user_nickname);

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // true в HTTPS
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddMinutes(15)
                    };

                    Response.Cookies.Append("access_token", access, cookieOptions);

                    var refresh = _jwtService.GenerateRefreshToken();
                    Response.Cookies.Append("refresh_token", refresh, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });
                    await ICreateFunction.SaveRefreshToken(result.user.id, refresh, factory);
                    return Ok(result.user);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            }
            else
            {
                return BadRequest();
            }
            
        }

        /// <summary>
        /// Get information about user zones from his ID
        /// </summary>
        /// <returns>Возвращает информацию о зонах пользователя по его ID</returns>
        /// <response code="200">Возвращает данные зон пользователя</response>
        /// <response code="400">ID является не натуральным числом; Подан ошибочный id</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpGet]
        [Route("UserZones/{id}")]
        async public Task<ActionResult> GetAllUserZones(IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id_user < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var user_zones = await IReadFunction.GetAllUserZones(id_user, factory);
            if (user_zones == null)
            {
                return BadRequest("Подан ошибочный id");
            }
            else
            {
                return Ok(user_zones);
            }
        }

        /// <summary>
        /// Delete information about user zones from his ID
        /// </summary>
        /// <returns>Удаляет информацию о пользователе по его ID</returns>
        /// <response code="200">Успешно удалены данные</response>
        /// <response code="400">ID является не натуральным числом</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpDelete]
        [Route("Delete")]
        async public Task<ActionResult> DeleteUser(IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id_user < 1)
            {
                return BadRequest("ID является не натуральным числом");
            }
            var result = await IDeleteFunction.DeleteUser(id_user, factory);
            if (!result)
                return BadRequest("No Delete data");
            return Ok();
        }

        /// <summary>
        /// Change user data (without password, nickname, email)\
        /// Изменяет пароль пользователя
        /// </summary>
        /// <param name="user">Model User data</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="400">Ошибка валидации данных; другие</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpPut]
        [Route("Update")]
        async public Task<IActionResult> UpdateUser(User user, IDatabaseFactory factory)
        {
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    await IUpdateFunction.UpdateUser(user, factory);
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
        /// Change user password\
        /// Изменяет пароль пользователя
        /// </summary>
        /// <param name="id">ID User</param>
        /// <param name="last_password">Current user password</param>
        /// <param name="new_password">New user password</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="400">Invalid credentials (Неправильно введены данные)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpPatch]
        [Route("ChangePassword")]
        public async Task<IActionResult> PatchPassword(string last_password, string new_password, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IPatchFunction.PatchPassword(id_user, last_password, new_password, factory);
            if (!result)
                return BadRequest("Invalid credentials");

            return Ok();
        }

        /// <summary>
        /// Change user nickname (if it is unique)\
        /// Изменяет никнейм пользователя (если он уникален)
        /// </summary>
        /// <param name="new_nickname">New user Nickname MaxLength=50</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введён аналогичный никнейм либо нет пользователя)</response>
        /// <response code="409">Данный никнейм уже привязана к другому аккаунту</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpPatch]
        [Route("ChangeNickname")]
        public async Task<IActionResult> PatchNickname([MaxLength(50)] string new_nickname, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IPatchFunction.PatchUserNickname(id_user, new_nickname, factory);
            if (result == -1)
                return Conflict("Invalid Nickname already exists");
            else if (result == 0)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }


        /// <summary>
        /// Change user email (if it is unique)\
        /// Изменяет почту пользователя (если она уникальна)
        /// </summary>
        /// <param name="new_email">New user Email MaxLength=50</param>
        /// <returns>При успешном изменении ничего не возвращает</returns>
        /// <response code="200"></response>
        /// <response code="200">Данные не изменены (Введён аналогичный email либо нет пользователя)</response>
        /// <response code="409">Данная почта уже привязана к другому аккаунту</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpPatch]
        [Route("ChangeEmail")]
        public async Task<IActionResult> PatchEmail([MaxLength(50)] string new_email, IDatabaseFactory factory)
        {
            ulong id_user = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await IPatchFunction.PatchUserEmail(id_user, new_email, factory);
            if (result == -1)
                return Conflict("Invalid Email already exists");
            else if (result == 0)
                return Ok("No change data (equal or not in DB)");

            return Ok();
        }

        /// <summary>
        /// Exit account and delete JWT
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(string refreshToken, IDatabaseFactory factory)
        {
            await IDeleteFunction.deleteRefresh(refreshToken, factory);
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Ok();
        }

        /// <summary>
        /// Update Access token (use when access token expires)
        /// Code for axios:
        /// axios.interceptors.response.use(
        /// response => response,
        ///async error => {
        ///if (error.response.status === 401) {
        /// // вызываем refresh
        ///await axios.post('/api/refresh', { }, { withCredentials: true });
        /// // повторяем исходный запрос
        ///return axios(error.config);
        /// }
        /// return Promise.reject(error);
        ///  }
        ///);
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(IDatabaseFactory factory)
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (refreshToken == null)
                return Unauthorized();

            var userId = await IReadFunction.ValidateRefresh(refreshToken, factory);

            if (userId == null)
                return Unauthorized();

            // старый refresh удаляем
            await IDeleteFunction.deleteRefresh(refreshToken, factory);

            var newAccess = _jwtService.GenerateAccessToken(userId.Value, "user");
            var newRefresh = _jwtService.GenerateRefreshToken();

            await ICreateFunction.SaveRefreshToken(userId.Value, newRefresh, factory);

            var result = new {
                AccessToken = newAccess,
                RefreshToken = newRefresh
            };

            if (result == null)
                return Unauthorized();

            Response.Cookies.Append("access_token", newAccess, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax, // важно для фронта на другом домене
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });
            Response.Cookies.Append("refresh_token", newRefresh, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            return Ok();
        } 
    }
}
