using NUnit.Framework;
using AchievementScraper.Persistence;
using AchievementScraper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AchievementScraper.Domain.Tests
{
    [TestFixture]
    public class ScrapeTests
    {
        [Test]
        public void Check_Empty_Requirements()
        {

        }

        [Test]
        public void Check_1Skill_1Quest_Requirements()
        {
            AchievementObject aCleanSlate = new AchievementObject
            {
                AName = "A Clean Slate",
                ADescription = "Cleanse the Corrupted Seren Stone with at least one crystal.",
                ARunescore = 15,
                AMembers = "Yes",
                ALink = "/w/A_Clean_Slate",
                ACategories = new List<string>
                {
                    "Exploration"
                },
                ASubcategories = new List<string>
                {
                    "Tirannwn"
                },
                AQuestReqs = new List<string>(),
                ASkillReqs = new List<string>()
            };

            aCleanSlate = Scrape.GetRequirements(aCleanSlate);

            Assert.Contains("Plague's End", aCleanSlate.AQuestReqs);
            Assert.Contains("75 Prayer", aCleanSlate.ASkillReqs);
        }

        [Test]
        public void Check_3Skill_Requirements()
        {
            AchievementObject aBridgeNotFar = new AchievementObject
            {
                AName = "A Bridge Not Far",
                ADescription = "Cross the River Lum using a crossbow.",
                ARunescore = 10,
                AMembers = "Yes",
                ALink = "/w/A_Bridge_Not_Far",
                ACategories = new List<string>
                {
                    "Exploration"
                },
                ASubcategories = new List<string>
                {
                    "Desert"
                },
                AQuestReqs = new List<string>(),
                ASkillReqs = new List<string>()
            };

            aBridgeNotFar = Scrape.GetRequirements(aBridgeNotFar);

            Assert.Contains("8 Agility", aBridgeNotFar.ASkillReqs);
            Assert.Contains("19 Strength", aBridgeNotFar.ASkillReqs);
            Assert.Contains("37 Ranged", aBridgeNotFar.ASkillReqs);
        }

        [Test]
        public void GetAch_A_Mini_Shipment()
        {
            string aMiniShipmentRow = @"<tr class=''><td><a href='/w/A_Mini_Shipment' title='A Mini Shipment'>A Mini Shipment</a></td><td>Yes</td><td>Loot the trunk in the Temple of Aminishi.</td><td>Combat<br>Completionist</td><td>Elite Dungeons<br>Master Quest Cape</td><td>5<p><br></p></td></tr>";
            HtmlNode achNode = HtmlNode.CreateNode(aMiniShipmentRow);

            AchievementObject achievementActual = Scrape.GetAchievementRow(achNode);
            AchievementObject achievementExpected = new AchievementObject
            {
                AName = "A Mini Shipment",
                AMembers = "Yes",
                ADescription = "Loot the trunk in the Temple of Aminishi.",
                ACategories = new List<string>
                {
                    "Combat",
                    "Completionist"
                },
                ASubcategories = new List<string>
                {
                    "Elite Dungeons",
                    "Master Quest Cape"
                },
                ARunescore = 5,
                AQuestReqs = new List<string>
                {
                    "Curse of the Black Stone (partial)"
                },
                ASkillReqs = new List<string>
                {
                    "None"
                },
                ALink = "/w/A_Mini_Shipment"
            };

            AssertingAchObj(achievementExpected, achievementActual);
        }

        [Test]
        public void GetAch_Kill_Kril_Vol_2_VI()
        {
            string killKrilVol2VI = @"<tr class=''><td><a href='/w/Kill_K%27ril_Vol_2_VI' title='Kill K''ril Vol 2 VI'>Kill K'ril Vol 2 VI</a></td><td>Yes</td><td>Defeat K'ril Tsutsaroth in hard mode. (X/100)</td><td>Combat</td><td>Boss Kills</td><td>5<p><br></p></td></tr>";
            HtmlNode achNode = HtmlNode.CreateNode(killKrilVol2VI);

            AchievementObject achievementActual = Scrape.GetAchievementRow(achNode);
            AchievementObject achievementExpected = new AchievementObject
            {
                AName = "Kill K'ril Vol 2 VI",
                AMembers = "Yes",
                ADescription = "Defeat K'ril Tsutsaroth in hard mode. (X/100)",
                ACategories = new List<string>
                {
                    "Combat"
                },
                ASubcategories = new List<string>
                {
                    "Boss Kills"
                },
                ARunescore = 5,
                AQuestReqs = new List<string>
                {
                    "Troll Stronghold (partial)"
                },
                ASkillReqs = new List<string>
                {
                    "60 Strength or 60 Agility",
                    "70 Constitution"
                },
                ALink = "/w/Kill_K%27ril_Vol_2_VI"
            };

            AssertingAchObj(achievementExpected, achievementActual);
        }

        [Test]
        public void GetAch_Great_Responsibility()
        {
            string greatResponsibility = @"<tr class=''><td><a href='/w/Great_Responsibility' title='Great Responsibility'>Great Responsibility</a></td><td>Yes</td><td>Have over 490,000 charge<sup class='noprint fact'>[<span class='fact-text' title='The preceding quote has been reproduced verbatim and is not a transcription error.'>sic</span>]</sup> in a jumbo generator in the Invention Guild.</td><td>Skills</td><td>Invention</td><td>20<p><br></p></td></tr>";
            HtmlNode achNode = HtmlNode.CreateNode(greatResponsibility);

            AchievementObject achievementActual = Scrape.GetAchievementRow(achNode);
            AchievementObject achievementExpected = new AchievementObject
            {
                AName = "Great Responsibility",
                AMembers = "Yes",
                ADescription = "Have over 490,000 charge[sic] in a jumbo generator in the Invention Guild.",
                ACategories = new List<string>
                {
                    "Skills"
                },
                ASubcategories = new List<string>
                {
                    "Invention"
                },
                ARunescore = 20,
                ALink = "/w/Great_Responsibility",
                AQuestReqs = new List<string>
                {
                    "None"
                },
                ASkillReqs = new List<string>
                {
                    "102 Invention"
                }
            };

            AssertingAchObj(achievementExpected, achievementActual);
        }

        [Test]
        public void GetAch_Pick_All()
        {
            string pickAll = @"<tr class=''><td><a href='/w/Pick_All' title='Pick All'>Pick All</a></td><td>Yes</td><td>Use a lockpick to open ALL of New Varrock's treasure chests.</td><td>Exploration</td><td>New Varrock</td><td>15<p><br></p></td></tr>";
            HtmlNode achNode = HtmlNode.CreateNode(pickAll);

            AchievementObject achievementActual = Scrape.GetAchievementRow(achNode);
            AchievementObject achievementExpected = new AchievementObject
            {
                AName = "Pick All",
                AMembers = "Yes",
                ADescription = "Use a lockpick to open ALL of New Varrock's treasure chests.",
                ACategories = new List<string>
                {
                    "Exploration"
                },
                ASubcategories = new List<string>
                {
                    "New Varrock"
                },
                ARunescore = 15,
                ALink = "/w/Pick_All",
                AQuestReqs = new List<string>
                {
                    "Dimension of Disaster: Shield of Arrav (partial)",
                    "Dimension of Disaster: Demon Slayer (partial)"
                },
                ASkillReqs = new List<string>
                {
                    "70 Thieving"
                }
            };

            AssertingAchObj(achievementExpected, achievementActual);
        }

        [Test]
        public void GetAch_No_Smoke_Without_Pyre()
        {
            string noSmokeWithoutPyre = @"<tr class=''><td><a href='/w/No_Smoke_Without_Pyre' title='No Smoke Without Pyre'>No Smoke Without Pyre</a></td><td>Yes</td><td>Make a pyre ship from magic logs.</td><td>Exploration</td><td>Fremennik</td><td>25<p><br></p></td></tr>";
            HtmlNode achNode = HtmlNode.CreateNode(noSmokeWithoutPyre);

            AchievementObject achievementActual = Scrape.GetAchievementRow(achNode);
            AchievementObject achievementExpected = new AchievementObject
            {
                AName = "No Smoke Without Pyre",
                AMembers = "Yes",
                ADescription = "Make a pyre ship from magic logs.",
                ACategories = new List<string>{
                    "Exploration"
                },
                ASubcategories = new List<string>
                {
                    "Fremennik"
                },
                ARunescore = 25,
                ALink = "/w/No_Smoke_Without_Pyre",
                AQuestReqs = new List<string>
                {
                    "Completed Firemaking section of Barbarian Training"
                },
                ASkillReqs = new List<string>
                {
                    "85 Crafting",
                    "85 Firemaking"
                }

            };

            AssertingAchObj(achievementExpected, achievementActual);
        }

        public void AssertingAchObj(AchievementObject achievementExpected, AchievementObject achievementActual)
        {
            Assert.AreEqual(achievementExpected.AName, achievementActual.AName);
            Assert.AreEqual(achievementExpected.ADescription, achievementActual.ADescription);
            Assert.AreEqual(achievementExpected.AMembers, achievementActual.AMembers);
            Assert.AreEqual(achievementExpected.ALink, achievementActual.ALink);
            Assert.AreEqual(achievementExpected.ARunescore, achievementActual.ARunescore);
            CollectionAssert.AreEqual(achievementExpected.ACategories, achievementActual.ACategories);
            CollectionAssert.AreEqual(achievementExpected.ASubcategories, achievementActual.ASubcategories);
            CollectionAssert.AreEqual(achievementExpected.AQuestReqs, achievementActual.AQuestReqs);
            CollectionAssert.AreEqual(achievementExpected.ASkillReqs, achievementActual.ASkillReqs);
        }
    }
}