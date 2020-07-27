using muscshop.Context;
using muscshop.filters;
using muscshop.Helper;
using muscshop.Models;
using muscshop.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace muscshop.Controllers
{
    [SessionFilter]
    public class AccountController : Controller
    {
        private StoreContext _storeContext = new StoreContext();

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User newUser)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = _storeContext.Users.Where(x => x.Username == newUser.Username).FirstOrDefault();

            if (result != null)
            { 
                ModelState.AddModelError("Username", "Username already Exists");
                return View();
            }

            User user = new User();

            user.Email = newUser.Email;
            user.Username = newUser.Username;
            user.Password = Generatehash(newUser.Password + hashstr);
            user.Confirmation = Guid.NewGuid();
            user.PassRecovery = Guid.NewGuid();
            user.Roles = _storeContext.Roles.Where(x => x.RoleName == "Customer").ToList();
            

            Uri uri = new Uri(Request.Url.AbsoluteUri);

            var urlHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

            var text = $"რეგისტრაციის დასასრულებლად გადადით ლინკზე: {urlHost}/Account/Confirmation/{user.Confirmation}";
            SendConfirmation(user.Email, text);


            _storeContext.Users.Add(user);
            _storeContext.SaveChanges();
            Meilconfirmation();


            return RedirectToAction("Meilconfirmation");
        }

        public ActionResult Meilconfirmation()
        {
            return View();
        }


        private void SendConfirmation(string to, string text)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress("sannyybg@gmail.com");
            mail.To.Add(to);
            mail.Subject = "Registration Confirm";
            mail.Body = text;
            mail.IsBodyHtml = true;

            System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient("in-v3.mailjet.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("b75cfa38ea9ea632fd334a6cd62ba54d", "6ac57d919fbb7d38f1b87ed7895df893");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);

        }

        public ActionResult LogIn()
        {
            if(!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                return RedirectToAction("index", "store");
            }

            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User user)
        {
            var pass = Generatehash(user.Password + hashstr);
            var userxists = _storeContext.Users.Any(x => x.Username == user.Username && x.Password == pass && x.Active);
            if (!userxists)
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                return View();
            }

            FormsAuthentication.SetAuthCookie(user.Username, false);

            MigrationShoppingCart(user.Username);


            return RedirectToAction("index", "store");
            

        }

        public ActionResult Confirmation(string id)
        {
            var user = _storeContext.Users.Where(x => x.Confirmation.ToString().ToLower() == id.ToString().ToLower()).FirstOrDefault();
            user.Active = true;
            user.ConfigmPassword = user.Password;
            _storeContext.SaveChanges();
            Confirm();

            return RedirectToAction("Confirm");
        }

        public ActionResult Confirm()
        {
            return View();
        }






        public ActionResult SendRecovery(string email)
        {
            var user = _storeContext.Users.Where(x => x.Email == email).FirstOrDefault();
            if (user != null)
            {
                SendRecovery2(user);
                return RedirectToAction("Login");
            }

            else
            {
                ModelState.AddModelError("Email", "Email is not registered");
                return RedirectToAction("Login");
            }

         
        }

       

        private ActionResult SendRecovery2(User user)
        {
            Uri uri = new Uri(Request.Url.AbsoluteUri);

            var urlHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

            var text = $"პაროლის აღსადგენად გადადით ბმულზე: {urlHost}/Account/Recovery/{user.PassRecovery}";
            SendtoFile(user.Email, text);

            return RedirectToAction("login");
        }

        private void SendtoFile(string email, string text)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress("sannyybg@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Registration Confirm";
            mail.Body = text;
            mail.IsBodyHtml = true;

            System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient("in-v3.mailjet.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("bf7c394833ccc58d80a6ea5d0c5a4878", "3d2ad3bd2a675935d32066d9a1cab769");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }





        public ActionResult Recovery(string id)
        {
            
            var user = _storeContext.Users.Where(x => x.PassRecovery.ToString().ToLower() == id.ToString().ToLower()).FirstOrDefault();

            return View(user);
        }


        [HttpPost]
        public ActionResult Recovery(User userpass)
        {

            var olduserpass = _storeContext.Users.Where(x => x.UserId == userpass.UserId).FirstOrDefault();

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Incorrect Password, min: 8, max: 24 symbols");
                return View();
            }

            if (userpass.Password == userpass.ConfigmPassword)
            {
                olduserpass.Password = Generatehash(userpass.Password+hashstr);
                olduserpass.ConfigmPassword = Generatehash(userpass.ConfigmPassword+hashstr);
                _storeContext.SaveChanges();
                return RedirectToAction("login");
            }

            else
            {
                ModelState.AddModelError("", "Passwords don't Match");
                return View();
            }
        }



        private const string hashstr = "agqnlvG0oYvUsglS58cX";
        
        private string Generatehash(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                for(int i = 0; i<hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            Session.Abandon();
            //Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            return RedirectToAction("login");
        }

        public ActionResult ChangePass()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ChangePass(ChangePassViewModel changepass)
        {

            var user = _storeContext.Users.Where(x => x.Username == changepass.Username).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError("", "Incorrect Username");
                return View();
            }

            var passw = Generatehash(changepass.CurrentPassword + hashstr);
            if (user.Password == passw && changepass.NewPassword.Length > 7 && changepass.NewPassword == changepass.ConfirmnewPassword)
            {
                user.Password = Generatehash(changepass.NewPassword+hashstr);
                _storeContext.SaveChanges();
                Session["User"] = null;
                return View("Chngpass");
            }


            ModelState.AddModelError("", "Incorrect Username or Password");
            return View();
        }

        public void MigrationShoppingCart(string username)
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            cart.MigrateCart(username);

            Session[ShoppingCart.CartSessionKey] = username;
        }




    }
}