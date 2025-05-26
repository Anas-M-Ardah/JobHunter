using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
     
    }
}
