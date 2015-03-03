using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Duncan.FileCanDB;

namespace FileCanBlog.Code
{
    public class PageHandler
    {
        /// <summary>
        /// Create or save a page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool Save(Page page, string SectionTitle)
        {
            IFileCanDB<Page> FileCanDbPosts = new FileCanDB<Page>("", SectionTitle, "Pages", StorageType.json, true);
            FileCanDbPosts.InsertPacket(page, page.TitleUrlFriendly);
            LoadPages(SectionTitle);
            return true;
        }

        public bool Archive(string Title, string SectionTitle)
        {
            IFileCanDB<Page> FileCanDbPosts = new FileCanDB<Page>("", SectionTitle, "Pages", StorageType.json, true);
            Page page = FileCanDbPosts.GetPacket(Title).Data;
            page.Archive = true;
            return FileCanDbPosts.UpdatePacket(Title, page);
        }

        public bool Delete(string Title, string SectionTitle, out string Message)
        {
            IFileCanDB<Page> FileCanDbPosts = new FileCanDB<Page>("", SectionTitle, "Pages", StorageType.json, true);
            Page page = FileCanDbPosts.GetPacket(Title).Data;
            if (page.Archive)
            {
                Message = "Deleted";
                return FileCanDbPosts.DeletePacket(Title);
            }
            Message = "File not archived. Arhive first before deleting.";
            return false;
        }

        public Page Load(string Title, string SectionTitle)
        {
            return GetPages(SectionTitle).Where(x => x.TitleUrlFriendly == Title).FirstOrDefault();
        }

        private List<Page> GetPages(string SectionTitle)
        {
            if (HttpRuntime.Cache[SectionTitle + "Pages"] == null)
                LoadPages(SectionTitle);

            if (HttpRuntime.Cache[SectionTitle + "Pages"] != null)
                return HttpRuntime.Cache[SectionTitle + "Pages"] as List<Page>;

            return new List<Page>();
        }

        private void LoadPages(string SectionTitle)
        {
            IFileCanDB<Page> FileCanDbPosts = new FileCanDB<Page>("", SectionTitle, "Pages", StorageType.json, true);
            List<Page> results = FileCanDbPosts.GetPackets(0, 1000).Select(x => x.Data).ToList();
            results.Sort((p1, p2) => p2.Created.CompareTo(p1.Created));
            HttpRuntime.Cache.Insert(SectionTitle + "Pages", results);
        }

        public List<Page> List(string SectionTitle, int skip, int take, out int total)
        {
            var pages = GetPages(SectionTitle);
            total = pages.Count;
            return pages.Skip(skip).Take(take).ToList();
        }
    }
}