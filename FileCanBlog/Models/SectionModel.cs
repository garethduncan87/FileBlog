using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FileCanBlog.Models
{
    public class SectionModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool Enabled { get; set; }
        public string TitleUrlFriendly
        {
            get
            {
                return new string(Title.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)).ToArray());
            }
        }
    }
}