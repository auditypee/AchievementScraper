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

namespace AchivementScraper.Domain
{
    public class Scrape
    {
        private readonly string LOAURL = @"https://runescape.wiki/w/List_of_achievements";
        private readonly string RSWIKIURL = @"https://runescape.wiki";
        public List<AchievementObject> AchievementObjects { get; set; }
        
        public Scrape()
        {
            AchievementObjects = GetDataAsObjects();
        }
        private List<AchievementObject> GetDataAsObjects()
        {
            List<List<string>> tableData = new List<List<string>>();

            using (WebClient wb = new WebClient())
            {
                // download the webpage as a string
                string listofachievementsString = wb.DownloadString(LOAURL);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(listofachievementsString);

                tableData = GetData(tableData, htmlDoc);
                // removes the first in the list because it is unnecessary
                tableData.RemoveAt(0);
            }
            List<AchievementObject> achievements = ConvertTableToObj(tableData);
            
            return achievements;
        }

        private List<List<string>> GetData(List<List<string>> tableData, HtmlDocument htmlDoc)
        {
            // select the entire table that contains the achievements data
            var loaTable = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='mw-content-text']/div/table[1]/tbody");
            // collects all the rows from the table
            var loaRows = loaTable.SelectNodes(".//tr");

            // parses through each row to extract the data
            foreach (var row in loaRows)
            {
                List<string> currentRowValues = new List<string>();
                // parses through each column to extract individual data
                foreach (var column in row.ChildNodes)
                {
                    // makes sure that no empty things are collected
                    if (!string.IsNullOrWhiteSpace(column.InnerText))
                    {
                        // HACK: - could possibly remove these few lines?
                        // removes carriage returns, newlines, and tabs 
                        // gets the html tag because HAP can't retain <br> from InnerText
                        string data = Regex.Replace(column.InnerHtml, @"\r|\n|\t", string.Empty);
                        // removes Name's html tags
                        data = Regex.Replace(data, @".*\>(.*)\<\/a\>", @"$1");
                        // removes <p><br></p> due to Runescore
                        data = Regex.Replace(data, @"\<p\>\<br\>\</p\>", string.Empty);
                        // separates text by br and changes it to a delimiter ':'
                        data = Regex.Replace(data, @"\<br\>", "|");


                        //string data = Regex.Replace(column.InnerText, @"\r|\n|\t", ":");
                        currentRowValues.Add(data);
                    }
                }
                // gets the link
                var link = row.SelectSingleNode(".//a").Attributes["href"].Value;
                currentRowValues.Add(link);
                tableData.Add(currentRowValues);
            }
            return tableData;
        }


        private List<AchievementObject> ConvertTableToObj(List<List<string>> tableData)
        {
            int i = 0;
            List<AchievementObject> achievements = new List<AchievementObject>();
            foreach (var tableRow in tableData)
            {
                AchievementObject achievement = InitAchievementObj(tableRow);
                achievement = GetRequirementsAsObj(achievement);
                
                achievements.Add(achievement);

                string line = AchObjToString(achievement);
                //Console.WriteLine(line);
                System.Diagnostics.Debug.Write(i++ + " " + line);
                System.Threading.Thread.Sleep(500);
            }

            return achievements;
        }

        private string AchObjToString(AchievementObject achievement)
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

        private AchievementObject InitAchievementObj(List<string> tableRow)
        {
            string name = tableRow.ElementAt(0);
            string members = tableRow.ElementAt(1);
            string description = tableRow.ElementAt(2);
            string category = tableRow.ElementAt(3);
            string subCategory = tableRow.ElementAt(4);
            int runescore = int.Parse(tableRow.ElementAt(5));
            string link = tableRow.ElementAt(6);

            List<string> splitCategory = SplitString(category);
            List<string> splitSubcategory = SplitString(subCategory);
            List<string> questReqs = new List<string>();
            List<string> skillReqs = new List<string>();
            AchievementObject achievement = new AchievementObject(
                name, description, runescore, members, link, splitCategory,
                splitSubcategory, questReqs, skillReqs);
            return achievement;
        }
        
        private List<string> SplitString(string str)
        {
            List<string> split = str.Split('|')
                .Where(s => !string.IsNullOrEmpty(s)).ToList();

            return split;
        }

        // goes through each link and gets the requirements
        private AchievementObject GetRequirementsAsObj(AchievementObject achievement)
        {
            using (WebClient wb = new WebClient())
            {
                string achievementPageString = wb.DownloadString(RSWIKIURL + achievement.ALink);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(achievementPageString);

                var infoTable = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"infobox-achievement\"]/tbody");
                var reqNode = infoTable.SelectSingleNode(".//td[@class='qc-active']");
                // has requirements table
                if (reqNode != null)
                {
                    string data = Regex.Replace(reqNode.InnerText, @"\r|\n|\t", "|");

                    achievement = SkillQuestReq(achievement, data);
                }
            }
            achievement = IfSkillQuestIsEmpty(achievement);

            return achievement;
        }

        private AchievementObject SkillQuestReq(AchievementObject achievement, string data)
        {
            var split = SplitString(data);
            // goes through each split string and adds it to the respective requirement
            foreach (var s in split)
            {
                // matches skill requirements
                if (Regex.IsMatch(s, @"\d+\s+\w+"))
                {
                    // TODO: - need to work on adding "boostable" level requirement
                    var rmB = Regex.Replace(s, @"\ \[B\]", string.Empty);
                    achievement.ASkillReqs.Add(rmB);
                }
                else
                    achievement.AQuestReqs.Add(s);
            }
            
            return achievement;
        }

        private AchievementObject IfSkillQuestIsEmpty(AchievementObject achievement)
        {
            if (!achievement.AQuestReqs.Any())
                achievement.AQuestReqs.Add("None");
            if (!achievement.ASkillReqs.Any())
                achievement.ASkillReqs.Add("None");

            return achievement;
        }
    }
}
