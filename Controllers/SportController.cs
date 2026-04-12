using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;

namespace Trey2.Controllers
{
    /// <summary>
    /// A controller for interacting with the "Sport" model
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SportController: ControllerBase
    {
        /// <summary>
        /// Get a list of sports
        /// </summary>
        /// <returns>Возвращает список видов спорта</returns>
        /// <response code="200">Возвращает список видов спорта</response>
        /// <response code="400">Данные в БД отсутствуют</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSports(IDatabaseFactory factory)
        {
            var sports = await IReadFunction.GetSports(factory);
            if (sports == null)
            {
                return new BadRequestObjectResult("Data not found");
            }
            else
            {
                return new ObjectResult(sports);
            }
        }
    }
}
