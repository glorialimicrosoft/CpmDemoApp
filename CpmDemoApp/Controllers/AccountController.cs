using CpmDemoApp.Models;
using CpmDemoApp.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CpmDemoApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = MockDatabase.Users
                    .FirstOrDefault(u => u.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase) && u.Password.Equals(model.Password));


                if (user != null)
                {
                    // Create user claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                    };

                    if (user.Role == "RegularAgent")
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, Data.AgentLists.First(agent => agent.Name == user.Username).Id));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Sign in user
                    await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                    if (user.Role == "AdminAgent")
                    {
                        return RedirectToAction("display", "AdminPortal");
                    }
                    else
                    {
                        return RedirectToAction("display", "EmployeePortal");
                    }
                }

                ViewBag.Error = "Invalid username or password.";
            }
            return View(model);
        }

        [Route("account/currentUser")]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return Ok(new
                {
                    Username = User.Identity.Name,
                    Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value
                });
            }
            return Unauthorized();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("index", "Home");
        }
    }
}
