using muscshop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace muscshop.filters
{
    public class SessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            StoreContext _storeContext = new StoreContext();

            var currentUser = HttpContext.Current.User.Identity.Name;

            if (!string.IsNullOrEmpty(currentUser))
            {
                var userRoles = (List<string>)filterContext.HttpContext.Session["UserRoles"];
                if (userRoles == null)
                {
                    var user = _storeContext.Users.Include("Roles").Where(x => x.Username == currentUser).FirstOrDefault();
                    filterContext.HttpContext.Session["UserRoles"] = user.Roles.Select(x => x.RoleName).ToList();
                }
            }
        }


    }
}