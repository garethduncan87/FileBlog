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
    public class SectionController : Controller
    {
        public ActionResult List(int page = 1)
        {
            SectionHandler handler = new SectionHandler();
            int take = 10;
            int skip = (page - 1) * take;
            int total;
            IEnumerable<SectionModel> sections = handler.List(skip, take, out total);
             List<SectionViewModel> results = new List<SectionViewModel>();
            foreach (var section in sections)
            {
                var sectionviewmodel = new SectionViewModel();
                sectionviewmodel.section = section;
                results.Add(sectionviewmodel);
            }

            return View(results);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SectionModel section)
        {
            SectionHandler handler = new SectionHandler();
            handler.Save(section);
            return RedirectToAction("List");
        }

        public ActionResult Delete(string section)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(string section, string Command)
        {
            if (Command == "delete")
            {
                //Get number of pages in section including archived
                PageHandler pagehandler = new PageHandler(section);
                SectionHandler handler = new SectionHandler();
                
                if (pagehandler.SectionTotalPages(true) == 0)
                    handler.Delete(section);
                else
                    ViewBag.Message = "There are still pages within this section. Delete pages first or disable this section.";

                return View();
            }
            return View();
        }

        public ActionResult Disable(string section)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Disable(string section, string Command)
        {
            if(Command == "disable")
            {
                SectionHandler handler = new SectionHandler();
                handler.Disable(section);
            }
            return View();
        }
    }
}