using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.BRC.Jobs
{
    [Route("/")]
    [Authorize]
    public class IndexController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return "hello";
        }
    }
}
