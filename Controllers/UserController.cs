using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;
using Trey2.DB;
using Trey2.DB.IFunction;
using Trey2.Models;

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

        [HttpPost]
        [Route("auth")]
        async public Task<ActionResult> Auth(AuthRequest request, IDatabaseFactory factory)
        {
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
            if (isValid)
            {
                try
                {
                    (ulong id, bool read) = await IReadFunction.getAuthEmail(request, factory);
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
                        var user = (await IReadFunction.getUser(id, factory));
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
    }
}
