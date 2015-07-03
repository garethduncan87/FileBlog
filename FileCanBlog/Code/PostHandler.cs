using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Duncan.FileCanDB;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;
using Duncan.FileCanDB.Models;

namespace FileCanBlog.Code
{
    public class PostHandler
    {
        private FileCanDB<PostModel> FileCanDbPosts;
        public PostHandler()
        {
            FileCanDbPosts = new FileCanDB<PostModel>(DatabaseLocation, "Blogs", "Articles", true, StorageType.json);
        }

        public string DatabaseLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
        /// <summary>
        /// Create or save a page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool Save(PostModel page)
        {
            if (FileCanDbPosts.Update(page.TitleUrlFriendly, page))
            {
                Cache();
                return true;
            }
            return false;
        }

        public bool Create(PostModel page)
        {
            if (FileCanDbPosts.Insert(page.TitleUrlFriendly, page))
            {
                Cache();
                return true;
            }
            return false;
        }

        public bool PostTitleAlreadyExists(string title)
        {
            var data = FileCanDbPosts.Read(title);
            if (data == null)
                return false;

            return true;
        }

        public bool Archive(string Title)
        {
            PostModel page = FileCanDbPosts.Read(Title).Data;
            page.Archive = true;

            if (FileCanDbPosts.Update(Title, page))
            {
                Cache();
                return true;
            }
            return false;
        }

        public bool UnArchive(string Title)
        {
            PostModel page = FileCanDbPosts.Read(Title).Data;
            page.Archive = false;

            if (FileCanDbPosts.Update(Title, page))
            {
                Cache();
                return true;
            }
            return false;
        }

        public bool Delete(string Title)
        {
            return FileCanDbPosts.Delete(Title);
        }

        /// <summary>
        /// If page not in cache, reload async cache.
        /// Get page from file system.
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        public PostModel Load(string Title)
        {
            
            if (string.IsNullOrEmpty(Title))
                return null;

            if (HttpRuntime.Cache[Title] == null)
                Cache();

            if (HttpRuntime.Cache[Title] != null)
                return HttpRuntime.Cache[Title] as PostModel;

            PostModel page = FileCanDbPosts.Read(Title).Data; //archived pages are not stored in cache

            if (page != null)
                return page;

            return null;
        }

        public List<PostModel> List(string category, int skip, int take, string sort, bool descending, bool archived, out int total)
        {
            List<PostModel> pages = new List<PostModel>();
            if (archived)
                pages = GetPagesArchived(category);
            else
                pages = GetPages(category);
                
            total = pages.Count();
            
            if(!string.IsNullOrEmpty(sort))
            {
                PropertyInfo prop = typeof(PostModel).GetProperty(sort);
                pages = pages.OrderBy(x => prop.GetValue(x, null)).ToList();
            }

            if (descending)
            {
                PropertyInfo prop = typeof(string).GetProperty(sort);
                pages = pages.OrderByDescending(x => prop.GetValue(x, null)).ToList();
            }

            pages = pages.Skip(skip).Take(take).ToList();
            return pages;
        }

        private List<PostModel> GetPagesArchived(string Category = "")
        {
            string CategoryPagesCacheName = string.Format("{0}-Pages", Category);
            var pages = FileCanDbPosts.ReadList(0, 1000).Select(x => x.Data).Where(y => y.Archive).ToList();
            if (pages != null)
                return pages;

            return new List<PostModel>();
        }

        private List<PostModel> GetPages(string Category = "")
        {
            string CacheName = "Pages";
            if(!string.IsNullOrEmpty(Category))
                CacheName = string.Format("{0}-Pages", Category);

            //Get pages from non category cache
            if (HttpRuntime.Cache[CacheName] == null)
                Cache();

            List<PostModel> pages = new List<PostModel>();
            if (HttpRuntime.Cache[CacheName] != null)
                pages = HttpRuntime.Cache[CacheName] as List<PostModel>;

            if (pages != null)
                return pages;

            return new List<PostModel>();
        }

        /// <summary>
        /// Load all pages into cache.
        /// A cache for each category.
        /// Could be intensive if alot of pages.... will need to look into async solution
        /// </summary>
        private void Cache()
        {
            //List<PostModel> results = FileCanDbPosts.ReadList(0, 1000).Select(x => x.Data).Where(y=>y.Archive != true).Where(y => y.PublishDate <= DateTime.Now).ToList();
            IList<PacketModel<PostModel>> results = FileCanDbPosts.ReadList(0, 1000);
            IList<PostModel> data = results.Select(x => x.Data).ToList();
            data = data.Where(y => y.Archive != true).Where(y=>y.PublishDate <= DateTime.Now).ToList();


            List<string> Categories = new List<string>();
            Parallel.ForEach(data, page =>
            {
                //add each page to cache
                HttpRuntime.Cache.Insert(page.TitleUrlFriendly, page);
                if(page.Categories != null)
                    Categories.AddRange(page.CategoriesList);
                
                //Create cache of categories with pages in
            });

            List<string> DistinctCategories = Categories.Distinct().ToList();
            HttpRuntime.Cache.Insert("Categories", DistinctCategories);


            Parallel.ForEach(DistinctCategories, category =>
            {
                string ListPagesCacheName = string.Format("{0}-Pages", category);
                List<PostModel> CategoryPages = data.Where(x => x.Categories != null && x.Categories == category).ToList();
                HttpRuntime.Cache.Insert(ListPagesCacheName, CategoryPages);
            });

            HttpRuntime.Cache.Insert("Pages", data);  
        }        
    }
}