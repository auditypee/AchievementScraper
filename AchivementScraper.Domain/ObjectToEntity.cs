using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AchievementScraper.Persistence;

namespace AchivementScraper.Domain
{
    public class ObjectToEntity
    {
        public ObjectToEntity()
        {
            Scrape scraper = new Scrape();
            AddObjectToDatabase(scraper.AchievementObjects);
        }

        public void AddObjectToDatabase(List<AchievementObject> achievementObjects)
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (var context = new AchievementsDatabaseEntities())
            {
                foreach (var achievementObject in achievementObjects)
                    dbHelper.AddAchievement(achievementObject, context);
                context.SaveChanges();
            }
        }
    }
}
