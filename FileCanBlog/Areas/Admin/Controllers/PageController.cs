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
        public ActionResult Index(string title)
        {
            PageHandler handler = new PageHandler();
            PageModel page = handler.Load(title);
            if(page == null)
                throw new HttpException(404, "Page doesn't exist");

            PageViewModel pageview = new PageViewModel();
            pageview.page = page;

            return View(pageview);
        }

        public ActionResult Create()
        {
            PageHandler handler = new PageHandler();
            PageViewModel pageview = new PageViewModel();
            pageview.page = new PageModel();
            pageview.page.PublishDate = DateTime.Now;
            pageview.categories = handler.GetCurrentCategories();
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

            PageHandler handler = new PageHandler();
            if (handler.PageTitleAlreadyExists(pageview.page.TitleUrlFriendly))
            {
                ModelState.AddModelError("Title", "Title is already in use");
                return View(pageview);
            }

            if(handler.Save(pageview.page))
                return View(pageview);

            throw new HttpException(404, "Something went wrong...");
        }

        public ActionResult Archive(string title)
        {
            PageHandler handler = new PageHandler();
            PageModel page = handler.Load(title);
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
                PageHandler handler = new PageHandler();
                if (handler.Archive(title))
                    return RedirectToAction("List", new { page = 1 });
            }
            else
                return View();

            throw new HttpException(404, "Something went wrong...");
        }

        public ActionResult Delete(string title)
        {
            PageHandler handler = new PageHandler();
            PageModel page = handler.Load(title);
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
                PageHandler handler = new PageHandler();

                if (handler.Delete(title))
                    return RedirectToAction("List", new {  page = 1 });
            }
            else
                return View();
            
            throw new HttpException(404, deletemessage);
        }

        public ActionResult Edit(string title)
        {
            PageHandler handler = new PageHandler();
            PageModel page = handler.Load(title);
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
            if (Command != "save")
                return View();

            PageHandler handler = new PageHandler();
            if(handler.Save(page))
            {
                PageViewModel pageview = new PageViewModel();
                pageview.page = page;
                return View(pageview);
            }
               
            throw new HttpException(404, "Something went wrong...");
        }

        public ActionResult List(int page = 1, bool archived = false, bool descending = false, string sort = "", string category = "")
        {
            PageHandler handler = new PageHandler();
            int take = 10;
            int skip = (page - 1) * take;
            int total;
            List<PageModel> pages = new List<PageModel>();

            pages = handler.List(category, skip, take, sort, descending, archived, out total);

            List<PageViewModel> PageViewModels = new List<PageViewModel>();

            foreach (PageModel p in pages)
            {
                PageViewModel PageView = new PageViewModel();
                PageView.page = p;
                PageViewModels.Add(PageView);
            }

            return View(PageViewModels);
        }
    }
}