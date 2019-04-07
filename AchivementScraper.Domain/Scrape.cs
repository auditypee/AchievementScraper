using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using AchievementScraper.Persistence;

namespace AchievementScraper.Domain
{
    public class Scrape
    {
        private static readonly string LOAURL = @"https://runescape.wiki/w/List_of_achievements";
        private static readonly string RSWIKIURL = @"https://runescape.wiki";
        private static readonly List<string> skillsList = new List<string>
        {
            "Attack", "Strength", "Defence", "Ranged", "Prayer", "Magic", "Constitution", "Crafting", "Mining", "Smithing", "Fishing",
            "Cooking", "Firemaking", "Woodcutting", "Runecrafting", "Dungeoneering", "Fletching", "Agility", "Herblore", "Thieving", "Slayer",
            "Farming", "Construction", "Hunter", "Summoning", "Divination", "Invention"
        };

        public static List<AchievementObject> AchievementObjects { get; set; }

        public static void BeginScraping()
        {
            AchievementObjects = GetDataAsObjects();
        }

        private static List<AchievementObject> GetDataAsObjects()
        {
            List<AchievementObject> achievements = new List<AchievementObject>();

            using (WebClient wb = new WebClient())
            {
                // download the webpage as a string
                string listofachievementsString = wb.DownloadString(LOAURL);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(listofachievementsString);

                achievements = GetData(htmlDoc);
                // removes the first in the list because it is unnecessary
                achievements.RemoveAt(0);
            }

            return achievements;
        }

        public static List<AchievementObject> GetData(HtmlDocument htmlDoc)
        {
            List<AchievementObject> tableData = new List<AchievementObject>();

            // select the entire table that contains the achievements data
            var loaTable = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='mw-content-text']/div/table[1]/tbody");
            // collects all the rows from the table
            var loaRows = loaTable.SelectNodes(".//tr");

            foreach (var row in loaRows)
            {
                AchievementObject achievementData = GetAchievementRow(row);

                tableData.Add(achievementData);

                string line = AchObjToString(achievementData);
                System.Diagnostics.Debug.Write(line);
                // limited to 2 requests/second
                System.Threading.Thread.Sleep(500);
            }

            return tableData;
        }

        public static AchievementObject GetAchievementRow(HtmlNode row)
        {
            AchievementObject achievement = new AchievementObject();
            int index = 0;
            // parses through each column to initialize the AchievementObject
            foreach (var column in row.ChildNodes)
            {
                if (!string.IsNullOrWhiteSpace(column.InnerText))
                {
                    switch (index)
                    {
                        // Name
                        case 0:
                            achievement.AName = column.InnerText;
                            break;
                        // Members
                        case 1:
                            achievement.AMembers = column.InnerText;
                            break;
                        // Description
                        case 2:
                            achievement.ADescription = column.InnerText;
                            break;
                        // Category
                        case 3:
                            string categoryStr = Regex.Replace(column.InnerHtml, @"\<br\>", "|");

                            achievement.ACategories = categoryStr.Split('|').ToList();
                            break;
                        // Subcategory
                        case 4:
                            string subcategoryStr = Regex.Replace(column.InnerHtml, @"\<br\>", "|");

                            achievement.ASubcategories = subcategoryStr.Split('|').ToList();
                            break;
                        // Runescore
                        case 5:
                            achievement.ARunescore = int.Parse(column.InnerText);
                            break;
                    }
                    index++;
                }
            }
            // Link
            achievement.ALink = row.SelectSingleNode(".//a").Attributes["href"].Value;
            achievement.ASkillReqs = new List<string>();
            achievement.AQuestReqs = new List<string>();
            // Skill and Quest Reqs
            achievement = GetRequirements(achievement);
            
            return achievement;
        }
        
        // used for debugging purposes
        private static string AchObjToString(AchievementObject achievement)
        {
            string nam = achievement.AName;
            string des = achievement.ADescription;
            int run = achievement.ARunescore;
            string mem = achievement.AMembers;
            string lin = achievement.ALink;
            string cat = "";
            string sub = "";
            string que = "";
            string ski = "";
            foreach (var c in achievement.ACategories)
                cat += c + ", ";
            foreach (var s in achievement.ASubcategories)
                sub += s + ", ";
            foreach (var q in achievement.AQuestReqs)
                que += q + ", ";
            foreach (var s in achievement.ASkillReqs)
                ski += s + ", ";


            string line = string.Format(
                "Name: {0}  |Description: {1}  |Runescore: {2}  |Members: {3}  " +
                "|Link: {4}  |Categories: {5}  |Subcategories: {6}  |Quest Requirements: {7}  " +
                "|Skill Requirements: {8}\n",
                nam, des, run, mem, lin, cat, sub, que, ski
            );

            return line;
        }

        private static List<string> SplitString(string str)
        {
            List<string> split = str.Split('|')
                .Where(s => !string.IsNullOrEmpty(s)).ToList();

            return split;
        }
        
        // goes through each link and gets the requirements
        public static AchievementObject GetRequirements(AchievementObject achievement)
        {
            using (WebClient wb = new WebClient())
            {
                string achievementPageString = wb.DownloadString(RSWIKIURL + achievement.ALink);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(achievementPageString);

                var infoTable = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"infobox-achievement\"]/tbody");

                if (infoTable != null)
                {
                    var reqNode = infoTable.SelectSingleNode(".//td[@class='qc-active']");
                    // has requirements table
                    if (reqNode != null)
                    {
                        string data = Regex.Replace(reqNode.InnerText, @"\r|\n|\t", "|");

                        achievement = SkillQuestReq(achievement, data);
                    }
                }
                
            }
            // if empty requirements table
            achievement = IfSkillQuestIsEmpty(achievement);

            return achievement;
        }
        
        private static AchievementObject SkillQuestReq(AchievementObject achievement, string data)
        {
            var split = SplitString(data);
            // goes through each split string and adds it to the respective requirement
            foreach (var s in split)
            {
                string trimmedString = s.Trim().Replace("  ", " ");
                if (skillsList.Any(word => trimmedString.Contains(word)))
                    achievement.ASkillReqs.Add(trimmedString);
                else
                    achievement.AQuestReqs.Add(trimmedString);
            }
            
            return achievement;
        }

        private static AchievementObject IfSkillQuestIsEmpty(AchievementObject achievement)
        {
            if (!achievement.AQuestReqs.Any())
                achievement.AQuestReqs.Add("None");
            if (!achievement.ASkillReqs.Any())
                achievement.ASkillReqs.Add("None");

            return achievement;
        }
    }
}
