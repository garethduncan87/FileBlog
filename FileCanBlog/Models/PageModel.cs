using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FileCanBlog.Models
{
    public class PageModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Section { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Categories { get; set; }
        public bool Archive { get; set; }
        public DateTime PublishDate { get; set; }
        public string TitleUrlFriendly
        {
            get
            {
                return new string(Title.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)).ToArray());
            }
        }

        public List<string> CategoriesList
        {
            get
            {

                if (!string.IsNullOrEmpty(Categories))
                {
                    return Categories.Split(',').ToList();
                }
                return new List<string>();
            }
        }
    }
}