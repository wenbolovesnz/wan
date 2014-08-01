using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBuilder.Business.Entities;
using FormBuilder.Business.Entities.Enums;
using FormBuilder.Data.Contracts;
using WebMatrix.WebData;

namespace Wan.Controllers
{
    public class HomeController : Controller
    {
        private IApplicationUnit _applicationUnit;


        public HomeController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;           
        }

        public ActionResult Index()
        {
            if (WebSecurity.IsAuthenticated)
            {
                User currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

                ViewBag.Id = currentUser.Id;
                ViewBag.DOB = currentUser.DOB;
                ViewBag.City = currentUser.City;
                ViewBag.AboutMe = currentUser.AboutMe;
                ViewBag.NickName = currentUser.NickName;
                ViewBag.CreatedDate = currentUser.CreatedDate;
                ViewBag.UserImage = currentUser.ProfileImage ?? "/Content/images/defaultUserIcon.jpg";
                ViewBag.IsAdmin = currentUser.Roles.SingleOrDefault(m => m.RoleId == (int) RoleTypes.GroupManager) !=
                                  null;
            }               
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
