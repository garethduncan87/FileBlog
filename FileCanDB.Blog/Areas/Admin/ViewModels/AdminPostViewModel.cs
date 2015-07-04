using FileCanDB.Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileCanDB.Blog.Areas.Admin.ViewModels
{
    public class AdminPostViewModel
    {
        public PostModel Post { get; set; }
        public HttpPostedFileBase Image { get; set; }
    }
}
