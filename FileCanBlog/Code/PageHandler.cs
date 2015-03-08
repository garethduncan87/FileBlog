﻿using FileCanBlog.Models;
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
        private IFileCanDB<PageModel> FileCanDbPosts;
        private string SectionTitle;
        public PageHandler(string SectionTitle)
        {
            FileCanDbPosts = new FileCanDB<PageModel>(DatabaseLocation, "Pages", SectionTitle, StorageType.json, true);
            this.SectionTitle = SectionTitle;
        }

        private string DatabaseLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
        /// <summary>
        /// Create or save a page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool Save(PageModel page)
        {
            FileCanDbPosts.InsertPacket(page.TitleUrlFriendly, page);
            LoadPagesIntoCache();
            return true;
        }

        public List<string> GetCurrentCategories()
        {
            return HttpRuntime.Cache[SectionTitle + "Categories"] as List<string>;
        }

        public bool PageTitleAlreadyExists(string title)
        {
            if (FileCanDbPosts.GetPacket(title) != null)
                return false;

            return true;
        }

        public bool Archive(string Title)
        {
            PageModel page = FileCanDbPosts.GetPacket(Title).Data;
            page.Archive = true;
            LoadPagesIntoCache();
            return FileCanDbPosts.UpdatePacket(Title, page);
        }

        public bool Delete(string Title)
        {
            return FileCanDbPosts.DeletePacket(Title);
        }

        public int SectionTotalPages(bool IncludeArchive)
        {
            if (IncludeArchive)
                return FileCanDbPosts.GetPackets(0, 1000).Select(x => x.Data).ToList().Count;

            if (HttpRuntime.Cache[SectionTitle + "Pages"] == null)
                LoadPagesIntoCache();

            return (HttpRuntime.Cache[SectionTitle + "Pages"] as List<PageModel>).Count;
        }

        public bool DeleteAll()
        {
            //delete all pages in section
            List<PageModel> pages = FileCanDbPosts.GetPackets(0, 1000).Select(x => x.Data).ToList();
            foreach (PageModel page in pages)
            {
                Delete(page.TitleUrlFriendly);
            }
            return true;
        }

        /// <summary>
        /// If page not in cache, reload async cache.
        /// Get page from file system.
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        public PageModel Load(string Title, bool archived = false)
        {
            if (archived)
                return FileCanDbPosts.GetPacket(Title).Data; //archived pages are not stored in cache

            if (HttpRuntime.Cache[SectionTitle + Title] == null)
                LoadPagesIntoCache();
            
            return HttpRuntime.Cache[SectionTitle + Title] as PageModel;
        }

        

        public List<PageModel> List(string category, int skip, int take, string sort, bool descending, bool archived, out int total)
        {
            var pages = GetPages(archived, category);
            total = pages.Count;
            pages = pages.Skip(skip).Take(take).ToList();
            
            if(!string.IsNullOrEmpty(sort))
            {
                PropertyInfo prop = typeof(string).GetProperty(sort);
                pages = pages.OrderBy(x => prop.GetValue(x, null)).ToList();
            }

            if (descending)
            {
                PropertyInfo prop = typeof(string).GetProperty(sort);
                pages = pages.OrderByDescending(x => prop.GetValue(x, null)).ToList();
            }

            return pages;
        }

        private List<PageModel> GetPages(bool archived, string Category = "")
        {
            string ListPagesCacheName = string.Format("{0}-{1}-Pages", SectionTitle, Category);
            if (archived)
                return FileCanDbPosts.GetPackets(0, 1000).Select(x => x.Data).Where(y => y.Archive).ToList();

            List<PageModel> pages = new List<PageModel>();
            if (HttpRuntime.Cache[ListPagesCacheName] == null)
                LoadPagesIntoCache();

            if (HttpRuntime.Cache[ListPagesCacheName] != null)
                pages = HttpRuntime.Cache[ListPagesCacheName] as List<PageModel>;

            return pages;
        }

        /// <summary>
        /// Load all pages into cache.
        /// A cache for each section.
        /// A cache for each category.
        /// Could be intensive if alot of pages.... will need to look into async solution
        /// </summary>
        /// <param name="SectionTitle"></param>
        private void LoadPagesIntoCache()
        {
            
            List<PageModel> results = FileCanDbPosts.GetPackets(0, 1000).Select(x => x.Data).Where(y=>y.Archive != true).Where(y => y.PublishDate >= DateTime.Now).ToList();

            List<string> Categories = new List<string>();
            Parallel.ForEach(results, page =>
            {
                //add each page to cache
                HttpRuntime.Cache.Insert(SectionTitle + page.TitleUrlFriendly, page);
                Categories.AddRange(page.Categories);
                
                //Create cache of categories with pages in
            });
            List<string> DistinctCategories = Categories.Distinct().ToList();
            HttpRuntime.Cache.Insert(SectionTitle + "Categories", DistinctCategories);
            Parallel.ForEach(DistinctCategories, category =>
            {
                string ListPagesCacheName = string.Format("{0}-{1}-Pages", SectionTitle, category);
                List<PageModel> CategoryPages = results.Where(x => x.Categories.Equals(category)).ToList();
                HttpRuntime.Cache.Insert(ListPagesCacheName, CategoryPages);
            });

            HttpRuntime.Cache.Insert(SectionTitle + "Pages", results);  
        }


    }
}