using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementScraper.Persistence
{
    public class AchievementObject
    {
        public string AName { get; set; }
        public string ADescription { get; set; }
        public int ARunescore { get; set; }
        public string AMembers { get; set; }
        public string ALink { get; set; }
        public List<string> ACategories { get; set; }
        public List<string> ASubcategories { get; set; }
        public List<string> AQuestReqs { get; set; }
        public List<string> ASkillReqs { get; set; }

        public AchievementObject()
        {

        }

        public AchievementObject(string name, string description, int runescore, 
            string members, string link, List<string> categories, List<string> subcategories, 
            List<string> questReqs, List<string> skillReqs)
        {
            AName = name;
            ADescription = description;
            ARunescore = runescore;
            AMembers = members;
            ALink = link;
            ACategories = categories;
            ASubcategories = subcategories;
            AQuestReqs = questReqs;
            ASkillReqs = skillReqs;
        }
    }
}
