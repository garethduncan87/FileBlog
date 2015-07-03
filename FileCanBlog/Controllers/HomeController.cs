using FileCanBlog.Code;
using FileCanBlog.Models;
using FileCanBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileCanBlog.Controllers
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