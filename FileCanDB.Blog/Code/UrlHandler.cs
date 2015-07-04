using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileCanDB.Blog.Code
{
    public static class UrlHandler
    {
        public static string UrlFriendlyName(string name)
        {
            return Regex.Replace(name, @"[^A-Za-z0-9_\.~]+", "-").TrimStart('-').TrimEnd('-');
        }
    }
}
