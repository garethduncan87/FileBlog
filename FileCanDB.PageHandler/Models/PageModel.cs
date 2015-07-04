using FileCanDB.Blog.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FileCanDB.Blog.Models
{
    public class PageModel
    {
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WebFilesLocation"] + "\\" + ImageName;
            }
        }

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
                return UrlHandler.UrlFriendlyName(Title);
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