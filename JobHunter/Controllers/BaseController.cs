using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    [Authorize(Roles="EndUser")]
    public abstract class BaseController : Controller
    {
     
    }
}
