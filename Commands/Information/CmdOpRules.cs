/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdOpRules : Command
    {
        public override string name { get { return "oprules"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdOpRules() { }

        public override void Use(Player p, string message)
        {
            if (!File.Exists("text/oprules.txt")) {
                CP437Writer.WriteAllText("text/oprules.txt", "No oprules entered yet!");
            }
            List<string> oprules = CP437Reader.ReadAllLines("text/oprules.txt");

            Player who = p;
            if (message != "") {
                who = PlayerInfo.FindOrShowMatches(p, message);
                if (who == null) return;
                if (p != null && p.group.Permission < who.group.Permission) {
                    MessageTooHighRank(p, "send /oprules", false); return;
                }
            }

            Player.Message(who, "Server OPRules:");
            foreach (string s in oprules)
                Player.Message(who, s);
        }

        public override void Help(Player p) {
            Player.Message(p, "/oprules [player]- Displays server oprules to a player");
        }
    }
}
