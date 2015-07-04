using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FileCanDB.Blog.Models
{
    public class PostModel
    {

        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public HttpPostedFileBase Image { get; set; }



        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Archive { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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