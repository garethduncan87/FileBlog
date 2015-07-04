using FileCanDB.Blog.Code;
using FileCanDB.Blog.Models;
using FileCanDB.Blog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileCanDB.Blog.Controllers
{
    public class HomeController : Controller
    {
        private PostHandler _postHandler;
        public HomeController()
        {
            this._postHandler = new PostHandler();
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }


    }
}