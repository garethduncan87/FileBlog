using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileCanBlog.Code
{
    public class FileHandler
    {
        private string _directory;
        public FileHandler(string DirectoryPath)
        {
            this._directory = HttpContext.Current.Server.MapPath(DirectoryPath);
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);
        }

        /// <summary>
        /// Returns file path of file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public string SaveFile(HttpPostedFileBase file, string FileName)
        {
            string extension = Path.GetExtension(file.FileName);
            try
            {
                string filepath = _directory + "\\" + UrlHandler.UrlFriendlyName(FileName) + extension;

                if (File.Exists(filepath))
                    File.Delete(filepath);

                file.SaveAs(filepath);
                return filepath;
            }
            catch (Exception ex) { throw ex; }
        }

        public static string GetRelativeUrl(string filepath)
        {
            var uri = new System.Uri(filepath);
            return uri.AbsolutePath;
        }
    }
}
