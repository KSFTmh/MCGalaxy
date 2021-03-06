/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy {
    
    /// <summary> Manages the awards the server has, and which players have which awards. </summary>
    public static class Awards {
        
        public struct PlayerAward { public string Name; public List<string> Awards; }
        
        public class Award { public string Name, Description; }
        
        /// <summary> List of all awards the server has. </summary>
        public static List<Award> AwardsList = new List<Award>();

        /// <summary> List of all players who have awards. </summary>
        public static List<PlayerAward> PlayerAwards = new List<PlayerAward>();
        

        #region I/O
        
        public static void Load() {
            if (!File.Exists("text/awardsList.txt")) {
                using (StreamWriter SW = File.CreateText("text/awardsList.txt")) {
                    SW.WriteLine("#This is a full list of awards. The server will load these and they can be awarded as you please");
                    SW.WriteLine("#Format is:");
                    SW.WriteLine("# AwardName : Description of award goes after the colon");
                    SW.WriteLine();
                    SW.WriteLine("Gotta start somewhere : Built your first house");
                    SW.WriteLine("Climbing the ladder : Earned a rank advancement");
                    SW.WriteLine("Do you live here? : Joined the server a huge bunch of times");
                }
            }

            AwardsList = new List<Award>();
            PropertiesFile.Read("text/awardsList.txt", AwardsListLineProcessor, ':');
            PlayerAwards = new List<PlayerAward>();
            PropertiesFile.Read("text/playerAwards.txt", PlayerAwardsLineProcessor, ':');
            Save();
        }
        
        static void AwardsListLineProcessor(string key, string value) {
            if (value == "") return;
            Award award = new Award();
            award.Name = key;
            award.Description = value;
            AwardsList.Add(award);
        }
        
        static void PlayerAwardsLineProcessor(string key, string value) {
            if (value == "") return;
            PlayerAward pl;
            pl.Name = key.ToLower();
            pl.Awards = new List<string>();
            
            if (value.IndexOf(',') != -1)
                foreach (string award in value.Split(','))
                    pl.Awards.Add(award);
            else if (value != "")
                pl.Awards.Add(value);
            PlayerAwards.Add(pl);
        }

        public static void Save() {
            using (StreamWriter SW = File.CreateText("text/awardsList.txt"))  {
                SW.WriteLine("# This is a full list of awards. The server will load these and they can be awarded as you please");
                SW.WriteLine("# Format is:");
                SW.WriteLine("# AwardName : Description of award goes after the colon");
                SW.WriteLine();
                foreach (Award award in AwardsList)
                    SW.WriteLine(award.Name + " : " + award.Description);
            }
            
            using (StreamWriter SW = File.CreateText("text/playerAwards.txt")) {
                foreach (PlayerAward pA in PlayerAwards)
                    SW.WriteLine(pA.Name.ToLower() + " : " + string.Join(",", pA.Awards.ToArray()));
            }
        }
        #endregion
        
        
        #region Player awards
        
        /// <summary> Adds the given award to that player's list of awards. </summary>
        public static bool GiveAward(string playerName, string name) {
            foreach (PlayerAward pl in PlayerAwards) {
                if (!pl.Name.CaselessEq(playerName)) continue;
                
                foreach (string award in pl.Awards) {
                    if (award.CaselessEq(name)) return false;
                }
                pl.Awards.Add(name);
                return true;
            }
            PlayerAward newPl;
            newPl.Name = playerName;
            newPl.Awards = new List<string>();
            newPl.Awards.Add(name);
            PlayerAwards.Add(newPl);
            return true;
        }
        
        /// <summary> Removes the given award from that player's list of awards. </summary>
        public static bool TakeAward(string playerName, string name) {
            foreach (PlayerAward pl in PlayerAwards) {
                if (!pl.Name.CaselessEq(playerName)) continue;
                
                for (int i = 0; i < pl.Awards.Count; i++) {
                    if (!pl.Awards[i].CaselessEq(name)) continue;
                    pl.Awards.RemoveAt(i); 
                    return true;
                }
                return false;
            }
            return false;
        }
        
        /// <summary> Returns the percentage of all the awards that the given player has. </summary>
        public static string AwardAmount(string playerName) {
            int numAwards = AwardsList.Count;
            foreach (PlayerAward pl in PlayerAwards) {
                if (!pl.Name.CaselessEq(playerName)) continue;
                double percentage = Math.Round(((double)pl.Awards.Count / numAwards) * 100, 2);
                return "&f" + pl.Awards.Count + "/" + numAwards + " (" + percentage + "%)" + Server.DefaultColor;
            }
            return "&f0/" + numAwards + " (0%)" + Server.DefaultColor;
        }
        
        /// <summary> Finds the list of awards that the given player has. </summary>
        public static List<string> GetPlayerAwards(string name) {
            foreach (PlayerAward pl in PlayerAwards)
                if (pl.Name.CaselessEq(name)) return pl.Awards;
            return new List<string>();
        }
        #endregion
        
        
        #region Awards management
        
        /// <summary> Adds a new award with the given name. </summary>
        public static bool Add(string name, string desc) {
            if (Exists(name)) return false;

            Award award = new Award();
            award.Name = name;
            award.Description = desc;
            AwardsList.Add(award);
            return true;
        }
        
        /// <summary> Removes the award with the given name. </summary>
        public static bool Remove(string name) {
            foreach (Award award in AwardsList) {
                if (!award.Name.CaselessEq(name)) continue;
                AwardsList.Remove(award);
                return true;
            }
            return false;
        }
        
        /// <summary> Whether an award with that name exists. </summary>
        public static bool Exists(string name) {
            foreach (Award award in AwardsList)
                if (award.Name.CaselessEq(name)) return true;
            return false;
        }
        
        /// <summary> Whether an award with that name exists. </summary>
        public static string Find(string name) {
            foreach (Award award in AwardsList)
                if (award.Name.CaselessEq(name)) return award.Name;
            return null;
        }
        
        /// <summary> Gets the description of the award matching the given name, 
        /// or an empty string if no matching award was found. </summary>
        public static string GetDescription(string name) {
            foreach (Award award in AwardsList)
                if (award.Name.CaselessEq(name)) return award.Description;
            return "";
        }
        #endregion
    }
}
