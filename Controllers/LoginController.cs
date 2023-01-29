using Blockbuster_Rental_Software.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Blockbuster_Rental_Software.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (Request.Cookies.ContainsKey("authToken"))
            {
                ViewBag.ErrorMessage = "There is a user already logged in clear cookies";
                return View("LoginFailure");
            }
            return View();
        }

        public IActionResult ProcessLogin(UserModel userModel)
        {
            //if someone is already logged in
            if (Request.Cookies.ContainsKey("authToken"))
            {
                DB db = new DB();
                string loggedInID = "";
                Request.Cookies.TryGetValue("authToken", out loggedInID);
                List<object> reader = db.QueryRow("SELECT * from georgedatabase.users WHERE `SessionToken` = '" + loggedInID + "' ;");
                if (reader.Count <= 0) return RedirectToAction("Signout", "Login"); //if auth token oes not exist in databse sign out current user

                ViewBag.ErrorMessage = "someone is already logged in sign out first";
                return View("LoginFailure");
            }  
            
            UserModel loggedInUser = new UserModel();
            try 
            {
                //password encryption
                HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String("8jLlugGgN/FzhUG+Wt9s/+crFg8GTUqOCYi/t88WziOIjDLZsHxmGlpAXP28kcM6eFn9bxOVCQuWP+uAk8H4Pg=="));
                byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userModel.PasswordHash)); //PasswordHash is not yet hashed this is the step that hashes
                //

                //find requested user from database
                string command = "SELECT * FROM georgedatabase.users WHERE `Email` = '" + userModel.Email + "' AND `PasswordHash` = '" + Convert.ToBase64String(passwordHash) + "';";

                DB db = new DB();

                List<object> reader = db.QueryRow(command);
                //

                //if user exists
                if (reader[0] != null)
                {
                    loggedInUser = new UserModel { UserID = (int)reader[0], FirstName = (string)reader[1], LastName = (string)reader[2], Email = (string)reader[3]};



                    //make sure there are no duplicate auth token
                    String authToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); //get random session 
                    
                    while (db.QueryRow("SELECT * FROM georgedatabase.users WHERE `SessionToken` = '"+authToken+"';").Count != 0)
                    {
                        authToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                    }
                    //

                    db.NoDataQuery("UPDATE georgedatabase.users SET `SessionToken` = '" + authToken + "' WHERE (`UserID` = '" + loggedInUser.UserID + "');");


                    //authToken expires in 1 year
                    if (userModel.RememberMe)
                    {
                        DateTimeOffset? offset = new DateTimeOffset(DateTime.Now);

                        var cookie = new CookieOptions();

                        cookie.Expires = DateTime.Now.AddYears(1);
                        cookie.HttpOnly = true;

                        Response.Cookies.Append("authToken", authToken, cookie);

                    } else{ //authToken expires with session
                        var cookie = new CookieOptions();
                        cookie.HttpOnly = true;
                        cookie.Secure = true;
                        
                        Response.Cookies.Append("authToken", authToken);
                        
                    }
                    

                    return RedirectToAction("Index", "Home", loggedInUser);
                }
                else//if user doesnt exist
                {
                    ViewBag.ErrorMessage = "Wrong Username or Password";
                    return View("LoginFailure");
                }

                


            }
            catch (Exception ex)
            {
                //return View("LoginFailure", userModel);
                return Content(ex.Message);
            }
            
        }

        public IActionResult Signout()
        {
            DB db = new DB();

            string? authToken = "";

            Request.Cookies.TryGetValue("authToken", out authToken);

            Response.Cookies.Delete("authToken");
            return RedirectToAction("Index", "Home");
        }


    }
}
