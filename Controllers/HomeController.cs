using Blockbuster_Rental_Software.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Blockbuster_Rental_Software.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(UserModel userModel)
        {
            //if user signed in
            if (Request.Cookies.ContainsKey("authToken"))
            {
                UserModel loggedInUser = new UserModel();
                
                string cookieKey = "";
                
                Request.Cookies.TryGetValue("authToken", out cookieKey);

                string command = "SELECT * FROM georgedatabase.users WHERE `SessionToken` = @cookieKey;";

                DB dB= new DB();

                List<object> reader = dB.QueryRow(command, new List<string> { "@cookieKey" }, new List<string>{ cookieKey });
                if (reader.Count <= 0) return RedirectToAction("Signout", "Login");

                if (reader[0] != null)
                {
                    loggedInUser = new UserModel { UserID = (int)reader[0], FirstName = (string)reader[1], LastName = (string)reader[2], Email = (string)reader[3]};

                    return View(loggedInUser);
                }
                
                
            }
            

            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public IActionResult products()
        {
            return RedirectToAction("", "products");
        }
    }
}