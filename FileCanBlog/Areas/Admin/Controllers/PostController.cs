using FileCanBlog.Areas.Admin.ViewModels;
using FileCanBlog.Code;
using FileCanBlog.Models;
using FileCanBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileCanBlog.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        private PostHandler _postHandler;
        private FileHandler _fileHandler;
        private string _webFilesLocation;
        public PostController()
        {
            this._webFilesLocation = ConfigurationManager.AppSettings["WebFilesLocation"];
            this._fileHandler = new FileHandler(_webFilesLocation);
            this._postHandler = new PostHandler();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string title)
        {
            PostModel Post = _postHandler.Load(title);
            if(Post == null)
                throw new HttpException(404, "Post doesn't exist");

            AdminPostViewModel Postview = new AdminPostViewModel();
            Postview.Post = Post;

            return View(Postview);
        }

        public ActionResult Create()
        {
            AdminPostViewModel Postview = new AdminPostViewModel();
            Postview.Post = new PostModel();
            Postview.Post.PublishDate = DateTime.Now;
            return View(Postview);
        }

        [HttpPost]
        public ActionResult Create(AdminPostViewModel Postview, string Command)
        {
            Postview.Post.Modified = DateTime.Now;
            Postview.Post.Created = DateTime.Now;
            Postview.Post.Archive = false;
            if (!ModelState.IsValid)
                return View(Postview);

            if (_postHandler.PostTitleAlreadyExists(Postview.Post.TitleUrlFriendly))
            {
                ModelState.AddModelError("Title", "Title is already in use");
                return View(Postview);
            }

            //store image
            if (Postview.Image != null)
            {
                string imagePath = _fileHandler.SaveFile(Postview.Image, Postview.Post.TitleUrlFriendly);
                Postview.Post.ImageName = Path.GetFileName(imagePath);
            }


            if (_postHandler.Create(Postview.Post))
            {
                return RedirectToAction("Edit", "Post", new { title = Postview.Post.TitleUrlFriendly });
            }


            ViewBag.Error = "Failed to create post";
            return View(Postview);
        }

        public ActionResult UnArchive(string title)
        {
            PostModel Post = _postHandler.Load(title);
            if (Post == null)
                throw new HttpException(404, "Post doesn't exist");

            AdminPostViewModel Postview = new AdminPostViewModel();
            Postview.Post = Post;

            return View(Postview);
        }

        [HttpPost]
        public ActionResult UnArchive(string title, string Command)
        {
            if (Command.ToLower() == "yes")
            {
                if (_postHandler.UnArchive(title))
                    return RedirectToAction("List", new { Post = 1 });
            }

            ViewBag.Error = "Failed to unarchive post";
            return View();
        }

        public ActionResult Archive(string title)
        {
            PostModel Post = _postHandler.Load(title);
            if (Post == null)
                throw new HttpException(404, "Post doesn't exist");

            AdminPostViewModel Postview = new AdminPostViewModel();
            Postview.Post = Post;

            return View(Postview);
        }

        [HttpPost]
        public ActionResult Archive(string title, string Command)
        {
            if (Command.ToLower() == "yes")
            {
                if (_postHandler.Archive(title))
                    return RedirectToAction("List", new { archived = true });
            }

            ViewBag.Error = "Failed to archive post";
            return View();
        }

        public ActionResult Delete(string title)
        {
            PostModel Post = _postHandler.Load(title);
            if (Post == null)
                throw new HttpException(404, "Post doesn't exist");

            AdminPostViewModel Postview = new AdminPostViewModel();
            Postview.Post = Post;
            return View(Postview);
        }

        [HttpPost]
        public ActionResult Delete(string title, string Command)
        {
            string deletemessage = string.Empty;
            if (Command.ToLower() == "yes")
            {
                if (_postHandler.Delete(title))
                    return RedirectToAction("List", new { Post = 1 });
            }

            ViewBag.Error = "Failed to delete post";
            return View();
        }



        public ActionResult Edit(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new HttpException(404, "Post doesn't exist");

            PostModel Post = _postHandler.Load(title);
            if (Post == null)
                throw new HttpException(404, "Post doesn't exist");

            AdminPostViewModel Postview = new AdminPostViewModel();
            Postview.Post = Post;

            return View(Postview);
        }

        [HttpPost]
        public ActionResult Edit(AdminPostViewModel PostViewModel, string Command)
        {
            PostViewModel.Post.Modified = DateTime.Now;

            //store image
            if (PostViewModel.Image != null)
            {
                string imagePath = _fileHandler.SaveFile(PostViewModel.Image, PostViewModel.Post.TitleUrlFriendly);
                PostViewModel.Post.ImageName = Path.GetFileName(imagePath);
            }


            if (_postHandler.Save(PostViewModel.Post))
            {
                if (Command == "Save")
                    return View(PostViewModel);
                else
                    return RedirectToAction("Details", "Post", new { title = PostViewModel.Post.TitleUrlFriendly });
               
            }

            ViewBag.Error = "Failed to save post";
            return View(PostViewModel);
        }

        public ActionResult List(int Post = 1, bool archived = false, bool descending = false, string sort = "", string category = "")
        {
            int take = 10;
            int skip = (Post - 1) * take;
            int total;
            List<PostModel> Posts = new List<PostModel>();

            Posts = _postHandler.List(category, skip, take, sort, descending, archived, out total);

            PostListViewModel MyPostListViewModel = new PostListViewModel();
            MyPostListViewModel.Posts = Posts;

            return View(MyPostListViewModel);
        }
    }
}