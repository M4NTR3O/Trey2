using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Trey2.DB;
using Trey2.DB.IFunction;

namespace Trey2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        // POST: UserController/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("create")]
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
                    return new CreatedResult();
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
        }

        [HttpGet]
        [Route("get/{id}")]
        async public Task<ActionResult> Select(ulong id, IDatabaseFactory factory)
        {
            if (id < 1)
            {
                return new BadRequestObjectResult("ID являентся не натуральным числом");
            }
            var user = await IReadFunction.getUser(id, factory);
            if (user == null)
            {
                return new BadRequestObjectResult("Подан ошибочный id");
            }
            else
            {
                return new ObjectResult(user);
            }
        }
    }
}
