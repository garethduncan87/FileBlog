using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileCanBlog.ViewModels
{
    public class PageViewModel
    {
        public PageModel page { get; set; }
        public List<string> categories { get; set; }
    }
}