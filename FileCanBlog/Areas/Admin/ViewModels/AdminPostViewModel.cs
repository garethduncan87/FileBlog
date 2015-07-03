using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileCanBlog.Areas.Admin.ViewModels
{
    public class AdminPostViewModel
    {
        public PostModel Post { get; set; }
        public HttpPostedFileBase Image { get; set; }
    }
}
