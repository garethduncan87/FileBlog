using Duncan.FileCanDB;
using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FileCanBlog.Code
{
    public class SectionHandler
    {
        private string DatabaseLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
        private FileCanDB<SectionModel> FileCanDbSection;
        public SectionHandler()
        {
            FileCanDbSection = new FileCanDB<SectionModel>(DatabaseLocation, "Sections", "Sections", true, StorageType.json);
        }

        

        public SectionModel Load(string SectionTitle)
        {
            if (HttpRuntime.Cache[SectionTitle] == null)
                LoadSectionsIntoCache();

            return HttpRuntime.Cache[SectionTitle] as SectionModel;
        }

        public bool Save(SectionModel section)
        {
            FileCanDbSection.Insert(section.TitleUrlFriendly, section);
            LoadSectionsIntoCache();
            return true;
        }

        public bool Disable(string SectionTitle)
        {
            SectionModel section = FileCanDbSection.Read(SectionTitle).Data;
            section.Enabled = false;
            if (FileCanDbSection.Update(SectionTitle, section))
                LoadSectionsIntoCache();
            else
                return false;
            
            return true;
        }

        public bool Delete(string SectionTitle)
        {
            return FileCanDbSection.Delete(SectionTitle);
        }

        public IEnumerable<SectionModel> List(int skip, int take, out int total)
        {
            var sections = GetSections();
            total = sections.Count;
            return sections.Skip(skip).Take(take);
        }

        private List<SectionModel> GetSections()
        {
            if (HttpRuntime.Cache["Sections"] == null)
                LoadSectionsIntoCache();

            if (HttpRuntime.Cache["Sections"] != null)
                return HttpRuntime.Cache["Sections"] as List<SectionModel>;

            return new List<SectionModel>();
        }

        private void LoadSectionsIntoCache()
        {
            List<SectionModel> sections = FileCanDbSection.ReadList(0, 10000).Select(x =>x.Data).ToList();
            sections.Sort((p1, p2) => p2.Created.CompareTo(p1.Created));

            Parallel.ForEach(sections, section =>
            {
                //add each page to cache
                HttpRuntime.Cache.Insert(section.TitleUrlFriendly, section);
            });

            HttpRuntime.Cache.Insert("Sections", sections);
        }
    }
}