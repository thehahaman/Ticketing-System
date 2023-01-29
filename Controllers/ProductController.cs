using Blockbuster_Rental_Software.Models;
using Blockbuster_Rental_Software.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Diagnostics;



namespace Blockbuster_Rental_Software.Controllers
{
    public class ProductController : Controller
    {
        [Route("/products")]
        public IActionResult products()
        {

            UserModel loggedInUser;
            Product product;
            if (Request.Cookies.ContainsKey("authToken"))
            {
                DB db = new DB();
                string loggedInID = "";
                Request.Cookies.TryGetValue("authToken", out loggedInID);
                List<object> reader = db.QueryRow("SELECT * from georgedatabase.users WHERE `SessionToken` = '" + loggedInID + "' ;");
                if (reader.Count <= 0) return RedirectToAction("Signout", "Login");
                loggedInUser = new UserModel { UserID = (int)reader[0], FirstName = (string)reader[1], LastName = (string)reader[2], Email = (string)reader[3], isDeveloper = Convert.ToBoolean(reader[5]) };


            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            List<Product> productList = new List<Product>();
            List<UserModel> userList = new List<UserModel>();
            try
            {
                var connection = new MySqlConnection("Server=localhost; port=1433;User ID=root;Password=Specialpotato1*;Database=georgedatabase");

                connection.Open();

                var command = new MySqlCommand("SELECT * FROM georgedatabase.products;", connection);

                MySqlDataReader reader = command.ExecuteReader();



                while (reader.Read())
                {
                    productList.Add(new Product { Id = (int)reader[0], Name = (string)reader[1], Date = (DateTime)reader[2], openedBy = (int)reader[3], Description = (string)reader[4], assignedDeveloper = (int)reader[5] });

                    //add description to notes of not present
                    DB dB = new DB();
                    if (dB.QueryRow("SELECT * from georgedatabase.notes where `pId` = '" + (int)reader[0] + "';").Count <= 0)
                    {
                        var notesCommand = "INSERT INTO georgedatabase.notes (`pID`, `note`, `date`, `userID`, `userName`) VALUES(@pID, @note, @date, @userID, @userName);";
                        List<string> param = new List<string> { "@pID", "@note", "@date", "@userID", "@userName" };
                        List<string> values = new List<string> { (int)reader[0] + "", (string)reader[4], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loggedInUser.UserID + "", loggedInUser.FirstName + " " + loggedInUser.LastName };

                        dB.NoDataQuery(notesCommand, param, values);
                    }

                    //get the author of the product
                    DB db = new DB();
                    List<object> userReader = db.QueryRow("SELECT * FROM georgedatabase.users WHERE `UserID`='" + (int)reader[3] + "';");
                    userList.Add(new UserModel { UserID = (int)userReader[0], FirstName = (string)userReader[1], LastName = (string)userReader[2], Email = (string)userReader[3] });
                }

                reader.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }


            var viewModel = new ProductsViewModel
            {
                Users = userList,
                productList = productList
            };

            return View(viewModel);
        }

        [Route("/products/deleteProduct")]
        public ActionResult deleteProduct(string pId)
        {
            UserModel loggedInUser;
            Product product;

            if (Request.Cookies.ContainsKey("authToken"))
            {
                DB db = new DB();
                string loggedInID = "";
                Request.Cookies.TryGetValue("authToken", out loggedInID);
                List<object> reader = db.QueryRow("SELECT * from georgedatabase.users WHERE `SessionToken` = '" + loggedInID + "' ;");
                if (reader.Count <= 0) return RedirectToAction("Signout", "Login");
                loggedInUser = new UserModel { UserID = (int)reader[0], FirstName = (string)reader[1], LastName = (string)reader[2], Email = (string)reader[3], isDeveloper = Convert.ToBoolean(reader[5]) };
                
                db = new DB();
                reader = db.QueryRow("SELECT * from georgedatabase.products WHERE `id` = '" + pId + "' ;");
                product = new Product { openedBy = (int)reader[3] , assignedDeveloper = (int)reader[5]};
            } else
            {
                return RedirectToAction("Index", "Login");
            }


            if (loggedInUser.UserID == product.openedBy || (loggedInUser.UserID == product.assignedDeveloper && loggedInUser.isDeveloper))
            {
                List<Product> productList = new List<Product>();

                var command = "DELETE FROM georgedatabase.products WHERE id = " + pId + ";";

                DB db = new DB();
                db.NoDataQuery(command);
            } else
            {
                ViewBag.ErrorMessage = "You can only alter your own posts";
                return View("alterFailure");
            }
                

            return RedirectToAction("products");
        }

        [HttpPost]
        [Route("/products/edit")]
        public ActionResult edit(string pId, IFormCollection form)
        {
            try
            {
                UserModel loggedInUser;
                Product product;
                string? addedNote = form["note"];
                if (addedNote.Equals("")) return RedirectToAction("products");

                if (Request.Cookies.ContainsKey("authToken"))
                {
                    DB db = new DB();
                    string loggedInID = "";
                    Request.Cookies.TryGetValue("authToken", out loggedInID);

                    List<object> reader = db.QueryRow("SELECT * from georgedatabase.users WHERE `SessionToken` = '" + loggedInID + "' ;");
                    if (reader.Count <= 0) return RedirectToAction("Signout", "Login");
                    loggedInUser = new UserModel { UserID = (int)reader[0], FirstName = (string)reader[1], LastName = (string)reader[2], Email = (string)reader[3] };

                    db = new DB();
                    reader = db.QueryRow("SELECT * from georgedatabase.products WHERE `id` = '" + pId + "' ;");
                    product = new Product { openedBy = (int)reader[3], Name = (string)reader[1], Date = (DateTime)reader[2], assignedDeveloper = (int)reader[5] };
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                
                var command = "INSERT INTO georgedatabase.notes (`pID`, `note`, `date`, `userID`, userName) VALUES(@pID, @note, @date, @userID, @userName);";
                DB database = new DB();
                List<string> param = new List<string> { "@pID", "@note", "@date", "@userID", "@userName" };
                List<string> values = new List<string> { pId, addedNote, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loggedInUser.UserID + "", loggedInUser.FirstName + " " + loggedInUser.LastName };

                database.NoDataQuery(command, param, values);


                ViewBag.openedByUserName = loggedInUser.FirstName + " " + loggedInUser.LastName;
                return RedirectToAction("editView", new { pId = pId, openedBy = product.openedBy, title = product.Name, date = product.Date, assignedDeveloper = product.assignedDeveloper});
            } catch
            {
                return RedirectToAction("products");
            }
            
        }

        [Route("/products/editView")]
        public ActionResult editView(int pId, string openedByUserName, string title, DateTime date, int assignedDeveloper)
        {           

            if (Request.Cookies.ContainsKey("authToken"))
            {
               
                List<Notes> notes = new List<Notes>();
                Product p = new Product();
                var connection = new MySqlConnection("Server=localhost; port=1433;User ID=root;Password=Specialpotato1*;Database=georgedatabase");

                connection.Open();

                var command = new MySqlCommand("SELECT * FROM georgedatabase.products as products JOIN georgedatabase.notes as notes ON products.id = notes.pID WHERE `id` = '"+pId+"';", connection);

                MySqlDataReader reader = command.ExecuteReader();
                
                

                if(reader.Read())
                {
                    p.Id = (int)reader[0];
                    p.Name = (string)reader[1];
                    p.Date = (DateTime)reader[2];
                    p.openedBy= (int)reader[3];
                    p.assignedDeveloper = (int)reader[5];
                    notes.Add(new Notes { pID = (int)reader[7], text = (string)reader[8], date = (DateTime)reader[9], userID = (int)reader[10], userName = (string)reader[11] });
                }else
                {
                    p.Id = pId;
                    p.Name = title;
                    p.Date = date;
                    p.assignedDeveloper = assignedDeveloper;
                }

                while (reader.Read())
                {
                    notes.Add(new Notes { pID = (int)reader[7], text = (string)reader[8], date = (DateTime)reader[9], userID = (int)reader[10], userName = (string)reader[11] });
                       
                }

                reader.Close();
                connection.Close();

                ViewBag.openedByUserName = openedByUserName;
                p.notes = notes;

                return View(p);
            }else
            {
                return RedirectToAction("Index", "Login");
            }
                
        }

        [Route("/products/addTicket")]
        public ActionResult addProduct()
        {
            if (Request.Cookies.ContainsKey("authToken"))
            {
                
                Product product = new Product { Name = "", Date = DateTime.Now, Description = "" };

                ProductsViewModel loggedInUserProduct = new ProductsViewModel { Users = new List<UserModel> { }, productList = new List<Product> { product } };
                return View(loggedInUserProduct);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

 
            
        }

        
        public ActionResult processAddProduct(ProductsViewModel productViewModel)
        {
            if (Request.Cookies.ContainsKey("authToken"))
            {
                
                DB db = new DB();
                string loggedInID = "";
                Request.Cookies.TryGetValue("authToken", out loggedInID);
                List<object> reader = db.QueryRow("SELECT * from georgedatabase.users WHERE `SessionToken` = '" + loggedInID + "' ;");
                if (reader.Count <= 0) return RedirectToAction("Signout", "Login");
                UserModel loggedInUser = new UserModel { UserID = (int)reader[0], FirstName = (string)reader[1], LastName = (string)reader[2], Email = (string)reader[3] };

                var command = "INSERT INTO georgedatabase.products (`Name`, `Date`, `openedBy`, `Description`, `assignedDeveloper`) VALUES (@Title, @Date, @UserID, @Description, '0');";

                DB dB = new DB();

                List<object> lol = dB.QueryRow(command, new List<string> { "@Title", "@Date", "@UserID", "@Description" },
                                                                new List<string> { productViewModel.productList[0].Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") , loggedInUser.UserID + "", productViewModel.productList[0].Description });

                return RedirectToAction("products");

                
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            
        }
    }
}
