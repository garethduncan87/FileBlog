using FileCanDB.Blog.Code;
using FileCanDB.Blog.Models;
using FileCanDB.Blog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FileCanDB.Blog.Controllers
{
    public class BlogController : Controller
    {
        private PostHandler _postHandler;
        public BlogController()
        {
            this._postHandler = new PostHandler();
        }

        public ActionResult Post(string Title)
        {
            PostModel Post = _postHandler.Load(Title);
            if (Post == null)
                throw new HttpException(404, "Post doesn't exist");

            PostViewModel Postview = new PostViewModel();
            Postview.Post = Post;

            return View(Postview);
        }
    }
}
