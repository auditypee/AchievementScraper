using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace AchievementScraper.Persistence
{
    public class DatabaseHelper
    {
        public void AddAchievement(AchievementObject achievement, AchievementsDatabaseEntities context)
        {
            var dbAchievements = context.Achievements;
            
            Achievement cAchievement = new Achievement
            {
                Name = achievement.AName,
                Description = achievement.ADescription,
                Runescore = achievement.ARunescore,
                Members = achievement.AMembers,
                Link = achievement.ALink,
                Categories = CategoryAsList(achievement, context),
                Subcategories = SubcategoryAsList(achievement, context),
                QuestReqs = QuestReqAsList(achievement, context),
                SkillReqs = SkillReqAsList(achievement, context)
            };
            dbAchievements.Add(cAchievement);
        }

        private List<QuestReq> QuestReqAsList(AchievementObject achievement, AchievementsDatabaseEntities context)
        {
            var dbQuestReqs = context.QuestReqs;

            List<QuestReq> questReqList = new List<QuestReq>();
            foreach (var questReq in achievement.AQuestReqs)
            {
                if (questReq != "None")
                {
                    QuestReq cQuestReq = new QuestReq
                    {
                        Quest = questReq
                    };
                    dbQuestReqs.Add(cQuestReq);
                    questReqList.Add(cQuestReq);
                }
            }

            return questReqList;
        }

        private List<SkillReq> SkillReqAsList(AchievementObject achievement, AchievementsDatabaseEntities context)
        {
            var dbSkillReqs = context.SkillReqs;

            List<SkillReq> skillReqList = new List<SkillReq>();
            foreach (var skillReq in achievement.ASkillReqs)
            {
                if (skillReq != "None")
                {
                    SkillReq cSkillReq = new SkillReq
                    {
                        LevelSkill = skillReq
                    };

                    dbSkillReqs.Add(cSkillReq);
                    skillReqList.Add(cSkillReq);
                }
            }

            return skillReqList;
        }

        private List<Category> CategoryAsList(AchievementObject achievement, AchievementsDatabaseEntities context)
        {
            List<Category> categoryList = new List<Category>();
            foreach (var categoryStr in achievement.ACategories)
            {
                // get the Category object from the given string
                var category = (from c in context.Categories
                           where c.Name == categoryStr
                           select c).FirstOrDefault();
                
                categoryList.Add(category);
            }

            return categoryList;
        }

        private List<Subcategory> SubcategoryAsList(AchievementObject achievement, AchievementsDatabaseEntities context)
        {
            List<Subcategory> subcategoryList = new List<Subcategory>();
            foreach (var subcategoryStr in achievement.ASubcategories)
            {
                if (subcategoryStr != "No")
                {
                    // gets the Subcategory object from the given string
                    var subcategory = (from s in context.Subcategories
                                       where s.Name == subcategoryStr
                                       select s).FirstOrDefault();

                    subcategoryList.Add(subcategory);
                }
            }

            return subcategoryList;
        }
    }
}
