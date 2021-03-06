﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageMuseum.Models;
using ManageMuseumData.GetData;
using Microsoft.Ajax.Utilities;

namespace ManageMuseum.Controllers
{
    public class LoginController : Controller
    {
        private UserData userData = new UserData();
        // GET: Login
        public ActionResult ConfirmLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmLogin(UserModelView user)
        {
            
            
            if (userData.IsValid(user.UserName,user.Password))
            {
                var role = userData.Role(user.UserName, user.Password);
                var userId = userData.GetId(user.UserName, user.Password);
                switch (role)
                {
                    case "spacemanager":
                    {
                        HttpCookie cookie = Request.Cookies["UserId"];
                        HttpCookie cookie1 = Request.Cookies["Role"];
                        if (cookie ==null | cookie1 == null)
                        {
                            cookie = new HttpCookie("UserId");
                            cookie1 = new HttpCookie("Role");
                            cookie1.Value = role;
                            cookie.Value = userId.ToString();
                        }
                        else
                        {
                            cookie.Value = userId.ToString();
                            cookie1.Value = role;
                            }
                        Response.Cookies.Add(cookie);
                        Response.Cookies.Add(cookie1);


                            return RedirectToAction("SheduleEvent", "SheduleEvent");
                        }
                    case "artpiecemanager":
                        {
                            HttpCookie cookie = Request.Cookies["UserId"];
                            HttpCookie cookie1 = Request.Cookies["Role"];
                            if (cookie == null | cookie1 == null)
                            {
                                cookie = new HttpCookie("UserId");
                                cookie1 = new HttpCookie("Role");
                                cookie1.Value = role;
                                cookie.Value = userId.ToString();
                            }
                            else
                            {
                                cookie.Value = userId.ToString();
                                cookie1.Value = role;
                            }
                            Response.Cookies.Add(cookie);
                            Response.Cookies.Add(cookie1);
                            return RedirectToAction("SheduleExhibition", "ExhibitionShedule");
                        }
                }
            }

            return View();
        }

        public ActionResult LogOut()
        {

            if (Request.Cookies["Role"] != null)
            {
                HttpCookie myCookie = new HttpCookie("Role");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}