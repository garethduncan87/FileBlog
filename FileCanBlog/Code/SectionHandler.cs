using Duncan.FileCanDB;
using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileCanBlog.Code
{
    public class SectionHandler
    {

        public Section Load(string SectionTitle)
        {
            IFileCanDB<Section> FileCanDbSection = new FileCanDB<Section>("", "Sections", SectionTitle, StorageType.json, true);
            return FileCanDbSection.GetPacket(SectionTitle).Data;
        }

        public bool Save(Section section)
        {
            IFileCanDB<Section> FileCanDbSection = new FileCanDB<Section>("", "Sections", section.TitleUrlFriendly, StorageType.json, true);
            FileCanDbSection.InsertPacket(section, section.TitleUrlFriendly);
            return true;
        }

        public void Disable(string SectionTitle)
        {
            IFileCanDB<Section> FileCanDbSection = new FileCanDB<Section>("", "Sections", SectionTitle, StorageType.json, true);
            Section section = FileCanDbSection.GetPacket(SectionTitle).Data;
            section.Enabled = false;
            FileCanDbSection.UpdatePacket(SectionTitle, section);
        }

        public IEnumerable<Section> List(string SectionTitle, int skip, int take, out int total)
        {
            IFileCanDB<Section> FileCanDbSection = new FileCanDB<Section>("", "Sections", SectionTitle, StorageType.json, true);
            var list = FileCanDbSection.GetPackets(0, 1000);
            total = list.Count;
            return list.Select(x => x.Data).ToList();
        }
    }
}