using Game.Models;
using Microsoft.AspNetCore.Mvc;

namespace Game.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult MakeResponse<T>(Response<T> result) => result.IsSuccessStatusCode ? Ok(result.Content) : StatusCode((int)result.StatusCode, result.ErrorMessage);
        protected IActionResult MakeResponse(Response result) => result.IsSuccessStatusCode ? Ok() : StatusCode((int)result.StatusCode, result.ErrorMessage);
    }
}
