﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBuilder.Business.Entities;
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

            //var user = _applicationUnit.UserRepository.Get().First();

            //for (int i = 0; i < 101; i++)
            //{
            //    var group = new Group();
            //    group.GroupName = "Awesome group" + i;
            //    group.CreatedDate = DateTime.Now;
            //    group.Description = "This is a good one" + i;
            //    group.Users.Add(user);     
            //    _applicationUnit.GroupRepository.Insert(group);           
            //}

            //_applicationUnit.SaveChanges();

            if (WebSecurity.IsAuthenticated)
            {
                User currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

                ViewBag.Id = currentUser.Id;
                ViewBag.DOB = currentUser.DOB;
                ViewBag.City = currentUser.City;
                ViewBag.AboutMe = currentUser.AboutMe;
                ViewBag.NickName = currentUser.NickName;
                ViewBag.CreatedDate = currentUser.CreatedDate;                
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
