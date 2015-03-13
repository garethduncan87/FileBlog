using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileCanBlog.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int LoginCount { get; set; }
        public DateTime EnabledDate { get; set; }



    }
}