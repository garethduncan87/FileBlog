using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Duncan.FileCanDB;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;

namespace FileCanBlog.Code
{
    public class PageHandler
    {
        private FileCanDB<PageModel> FileCanDbPosts;
        public PageHandler()
        {
            FileCanDbPosts = new FileCanDB<PageModel>(DatabaseLocation, "Blogs", "Articles", true, StorageType.json);
        }

        private string DatabaseLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
        /// <summary>
        /// Create or save a page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool Save(PageModel page)
        { 
            FileCanDbPosts.Update(page.TitleUrlFriendly, page);
            Cache();
            return true;
        }

        public bool PageTitleAlreadyExists(string title)
        {
            var data = FileCanDbPosts.Read(title);
            if (data == null)
                return false;

            return true;
        }

        public bool Archive(string Title)
        {
            PageModel page = FileCanDbPosts.Read(Title).Data;
            page.Archive = true;
            Cache();
            return FileCanDbPosts.Update(Title, page);
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
        public PageModel Load(string Title, bool archived = false)
        {
            if (string.IsNullOrEmpty(Title))
                return null;
            if (archived)
                return FileCanDbPosts.Read(Title).Data; //archived pages are not stored in cache

            if (HttpRuntime.Cache[Title] == null)
                Cache();
            
            return HttpRuntime.Cache[Title] as PageModel;
        }

        public List<PageModel> List(string category, int skip, int take, string sort, bool descending, bool archived, out int total)
        {
            List<PageModel> pages;
            if (archived)
                pages = GetPagesArchived(category);
            else
                pages = GetPages(category);
                
            total = pages.Count();
            
            if(!string.IsNullOrEmpty(sort))
            {
                PropertyInfo prop = typeof(PageModel).GetProperty(sort);
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

        private List<PageModel> GetPagesArchived(string Category = "")
        {
            string CategoryPagesCacheName = string.Format("{0}-Pages", Category);
            var pages = FileCanDbPosts.ReadList(0, 1000).Select(x => x.Data).Where(y => y.Archive).ToList();
            if (pages != null)
                return pages;

            return new List<PageModel>();
        }

        private List<PageModel> GetPages(string Category = "")
        {
            string CacheName = "Pages";
            if(!string.IsNullOrEmpty(Category))
                CacheName = string.Format("{0}-Pages", Category);

            //Get pages from non category cache
            if (HttpRuntime.Cache[CacheName] == null)
                Cache();

            if (HttpRuntime.Cache[CacheName] != null)
                return HttpRuntime.Cache[CacheName] as List<PageModel>;
            
            return new List<PageModel>();
        }

        /// <summary>
        /// Load all pages into cache.
        /// A cache for each category.
        /// Could be intensive if alot of pages.... will need to look into async solution
        /// </summary>
        private void Cache()
        {
            List<PageModel> results = FileCanDbPosts.ReadList(0, 1000).Select(x => x.Data).Where(y=>y.Archive != true).Where(y => y.PublishDate <= DateTime.Now).ToList();

            List<string> Categories = new List<string>();
            Parallel.ForEach(results, page =>
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
                List<PageModel> CategoryPages = results.Where(x => x.Categories != null && x.Categories == category).ToList();
                HttpRuntime.Cache.Insert(ListPagesCacheName, CategoryPages);
            });

            HttpRuntime.Cache.Insert("Pages", results);  
        }        
    }
}