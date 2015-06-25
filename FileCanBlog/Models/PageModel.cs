using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FileCanBlog.Models
{
    public class PageModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Archive { get; set; }
        public DateTime PublishDate { get; set; }
        public string TitleUrlFriendly
        {
            get
            {
                string FriendlyTitle = Regex.Replace(Title, @"[^A-Za-z0-9_\.~]+", "-");
                FriendlyTitle = FriendlyTitle.TrimStart('-').TrimEnd('-');
                return FriendlyTitle;
            }
        }

        public string Categories { get; set; }
        public List<string> CategoriesList
        {
            get
            {
                if(!string.IsNullOrEmpty(Categories))
                {
                    return Categories.Split(',')
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();
                }
                return null;
                
            }
        }
    }
}