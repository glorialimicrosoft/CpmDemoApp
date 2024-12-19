using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CpmDemoApp.Controllers
{
    [Authorize]
    [Route("EmployeePortal")]
    public class EmployeePortalController : Controller
    {
        [Route("index")]
        public ActionResult display()
        {
            // Path to the index.html file in the wwwroot directory
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

            // Return the file as a response
            return PartialView("EmployeePortal");
        }
    }
}
