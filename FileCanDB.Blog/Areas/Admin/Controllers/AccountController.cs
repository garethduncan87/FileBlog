using FileCanDB.Blog.Code;
using FileCanDB.Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileCanDB.Blog.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            AccountHandler handler = new AccountHandler();
            string loginmessage;
            UserModel LoggedInUser = handler.Login(user.Username, user.Password, out loginmessage);
            if (LoggedInUser != null)
                Session["UserModel"] = LoggedInUser;
            else
            {
                ViewBag.Message = loginmessage;
                return View(user);
            }
            return RedirectToAction("List", "Page");
        }

        public ActionResult Logout()
        {
            Session["UserModel"] = null;
            return RedirectToAction("List", "Page");
        }

        public ActionResult Create()
        {
            AccountHandler handler = new AccountHandler();
            List<UserModel> users = handler.GetUsers();
            if (users.Count > 0)
                throw new HttpException(404, "Not found");

            return View();
        }

        [HttpPost]
        public ActionResult Create(UserModel user)
        {
            AccountHandler handler = new AccountHandler();
            if (handler.CreateUser(user))
            {
                string message;
                UserModel LoggedInUser = handler.Login(user.Username, user.Password, out message);
                Session["UserModel"] = LoggedInUser;
                return RedirectToAction("List", "Page");
            }
            else
            {
                ViewBag.Message = "Failed to create user";
                return View();
            }
        }

    }
}