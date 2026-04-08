using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trey2.DB.IFunction;
using Trey2.DB;

namespace Trey2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeTrainingController : ControllerBase
    {
        /// <summary>
        /// Get a list of types of training
        /// </summary>
        /// <returns>Возвращает список типов тренировки</returns>
        /// <response code="200">Возвращает список типов тренировки</response>
        /// <response code="400">Данные в БД отсутствуют</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTypesTraining(IDatabaseFactory factory)
        {
            var types = await IReadFunction.GetTypesTraining(factory);
            if (types == null)
            {
                return BadRequest("Ошибка");
            }
            else
            {
                return Ok(types);
            }
        }
    }
}
