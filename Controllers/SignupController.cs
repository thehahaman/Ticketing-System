using Blockbuster_Rental_Software.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;

namespace Blockbuster_Rental_Software.Controllers
{
    public class SignupController : Controller
    {
        public IActionResult Index()
        {
            if (Request.Cookies.ContainsKey("authToken"))
            {
                ViewBag.ErrorMessage = "There is a user already logged in clear cookies";
                return View("SignupFailure");
            }
            return View();
        }

        public IActionResult ProcessSignup(UserModel signUpModel)
        {
            if (Request.Cookies.ContainsKey("authToken"))
            {
                ViewBag.ErrorMessage = "There is a user already logged in clear cookies";
                return View("SignupFailure");
            }

            UserModel loggedInUser = new UserModel();
            try
            {
                var connection = new MySqlConnection("Server=localhost; port=1433;User ID=root;Password=Specialpotato1*;Database=georgedatabase");

                connection.Open();
                MySqlCommand command;
                command = new MySqlCommand("SELECT * FROM georgedatabase.users WHERE `Email` = '" + signUpModel.Email + "';", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read() && reader[0] != null)
                {
                    ViewBag.ErrorMessage = "A user already Exsists with this Email please login";
                    return View("SignupFailure");
                } else {
                    connection.Close();
                    connection.Open();
                    HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String("8jLlugGgN/FzhUG+Wt9s/+crFg8GTUqOCYi/t88WziOIjDLZsHxmGlpAXP28kcM6eFn9bxOVCQuWP+uAk8H4Pg=="));
                    byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signUpModel.PasswordHash)); //PasswordHash is not yet hashed this is the step that hashes

                    command = new MySqlCommand("INSERT INTO `georgedatabase`.`users` ( `FirstName`, `LastName`, `Email`, `PasswordHash`, `SessionToken`) VALUES (@FirstName, @LastName, @Email, @PasswordHash, '0');", connection);

                    command.Parameters.AddWithValue("@FirstName", signUpModel.FirstName);
                    command.Parameters.AddWithValue("@LastName", signUpModel.LastName);
                    command.Parameters.AddWithValue("@Email", signUpModel.Email);
                    command.Parameters.AddWithValue("@PasswordHash", Convert.ToBase64String(passwordHash));

                    command.ExecuteScalar();

                    connection.Close();


                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Sign up failure, server error. Please report this issue or try again later";
                return View("SignupFailure");
                //return Content(ex.Message);
            }

        }
    }
}
