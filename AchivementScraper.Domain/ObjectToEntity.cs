using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AchievementScraper.Persistence;

namespace AchievementScraper.Domain
{
    public class ObjectToEntity
    {
        public ObjectToEntity()
        {
            Scrape.BeginScraping();
            AddObjectToDatabase(Scrape.AchievementObjects);
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
