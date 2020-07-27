using muscshop.Context;
using muscshop.filters;
using muscshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace muscshop.Controllers
{
    [Authorize(Roles = "Admin")]
    [SessionFilter]
    public class UserController : Controller
    {

        StoreContext _storeContext = new StoreContext();

        public ActionResult Index()
        {
            
            var users = _storeContext.Users.Include("Roles");

            return View(users);
            
        }

        [HttpPost]
        public ActionResult ChangeRole(int? id)
        {
            var user = _storeContext.Users.Include("Roles").Where(x => x.UserId == id).FirstOrDefault();

            var currentrole = string.Empty;

            var roleslist = user.Roles.Select(x => x.RoleName).ToList();

            if (roleslist.Contains("Manager"))
            {
                user.Roles = _storeContext.Roles.Where(x => x.RoleName == "Customer").ToList();
                currentrole = "გახდეს მენეჯერი";  
            }

            else
            {
                user.Roles = _storeContext.Roles.Where(x => x.RoleName == "Manager").ToList();
                currentrole = "ჩამოერთვას მენეჯერი";   
            }

            _storeContext.SaveChanges();
            //return RedirectToAction("index");
            return Json(currentrole, JsonRequestBehavior.AllowGet);


        }

        public ActionResult Search(string parameter)
        {
            Thread.Sleep(1000);

            if(string.IsNullOrEmpty(parameter))
            {
                var users = _storeContext.Users.Include("Roles");
                return PartialView(users);
            }
            
            var user = _storeContext.Users.Include("Roles").Where(x => x.Username.ToLower().Contains(parameter.ToLower()));

            return PartialView(user);
        }

        public ActionResult UserList(string parameter)
        {
            var result = new List<User>();

            if (parameter == null)
            {
                result = _storeContext.Users.ToList();
            }

            else
            {
                result = _storeContext.Users.Where(x => x.Username.ToLower().Contains(parameter.ToLower())).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}