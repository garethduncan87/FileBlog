using FileCanBlog.Code;
using FileCanBlog.Models;
using FileCanBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileCanBlog.Areas.Admin.Controllers
{
    public class PageController : Controller
    {
        private PageHandler _pageHandler;
        public PageController()
        {
            _pageHandler = new PageHandler();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string title)
        {
            PageModel page = _pageHandler.Load(title);
            if(page == null)
                throw new HttpException(404, "Page doesn't exist");

            PageViewModel pageview = new PageViewModel();
            pageview.page = page;

            return View(pageview);
        }

        public ActionResult Create()
        {
            PageViewModel pageview = new PageViewModel();
            pageview.page = new PageModel();
            pageview.page.PublishDate = DateTime.Now;
            return View(pageview);
        }

        [HttpPost]
        public ActionResult Create(PageViewModel pageview, string Command)
        {
            pageview.page.Modified = DateTime.Now;
            pageview.page.Created = DateTime.Now;
            pageview.page.Archive = false;
            if (!ModelState.IsValid)
                return View(pageview);

            if (_pageHandler.PageTitleAlreadyExists(pageview.page.TitleUrlFriendly))
            {
                ModelState.AddModelError("Title", "Title is already in use");
                return View(pageview);
            }

            if (_pageHandler.Save(pageview.page))
                return RedirectToAction("Edit", "Page", new { title = pageview.page.TitleUrlFriendly });

            throw new HttpException(404, "Something went wrong...");
        }

        public ActionResult Archive(string title)
        {
            PageModel page = _pageHandler.Load(title);
            if (page == null)
                throw new HttpException(404, "Page doesn't exist");

            PageViewModel pageview = new PageViewModel();
            pageview.page = page;

            return View(pageview);
        }

        [HttpPost]
        public ActionResult Archive(string title, string Command)
        {
            if (Command == "archive")
            {
                if (_pageHandler.Archive(title))
                    return RedirectToAction("List", new { page = 1 });
            }
            else
                return View();

            throw new HttpException(404, "Something went wrong...");
        }

        public ActionResult Delete(string title)
        {
            PageModel page = _pageHandler.Load(title);
            if (page == null)
                throw new HttpException(404, "Page doesn't exist");

            PageViewModel pageview = new PageViewModel();
            pageview.page = page;

            return View(pageview);
        }

        [HttpPost]
        public ActionResult Delete(string title, string Command)
        {
            string deletemessage = string.Empty;
            if (Command == "delete")
            {
                if (_pageHandler.Delete(title))
                    return RedirectToAction("List", new {  page = 1 });
            }
            else
                return View();
            
            throw new HttpException(404, deletemessage);
        }

        public ActionResult Edit(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new HttpException(404, "Page doesn't exist");

            PageModel page = _pageHandler.Load(title);
            if (page == null)
                throw new HttpException(404, "Page doesn't exist");

            PageViewModel pageview = new PageViewModel();
            pageview.page = page;

            return View(pageview);
        }

        [HttpPost]
        public ActionResult Edit(PageModel page, string Command)
        {
            page.Modified = DateTime.Now;

            if(_pageHandler.Save(page))
            {
                PageViewModel pageview = new PageViewModel();
                pageview.page = page;
                if (Command == "Save")
                    return View(pageview);
                else
                    return RedirectToAction("Details", "Page", new { title = page.TitleUrlFriendly });
               
            }
               
            throw new HttpException(404, "Something went wrong...");
        }

        public ActionResult List(int page = 1, bool archived = false, bool descending = false, string sort = "", string category = "")
        {
            int take = 10;
            int skip = (page - 1) * take;
            int total;
            List<PageModel> pages = new List<PageModel>();

            pages = _pageHandler.List(category, skip, take, sort, descending, archived, out total);

            PageListViewModel MyPageListViewModel = new PageListViewModel();
            MyPageListViewModel.Pages = pages;

            return View(MyPageListViewModel);
        }
    }
}