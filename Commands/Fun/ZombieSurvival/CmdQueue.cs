/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdQueue : Command
    {
        public override string name { get { return "queue"; } }
        public override string shortcut { get { return "qz"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
		public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }      
        public CmdQueue() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }
            string value = args[1];
            
            if (args[0].CaselessEq("zombie")) {
                Player who = PlayerInfo.FindOrShowMatches(p, value);
                if (who == null) return;
                
                Player.Message(p, value + " was queued.");
                Server.zombie.QueuedZombie = value;
                if (Server.zombie.CurLevel != null)
                    Server.zombie.CurLevel.ChatLevel(who.ColoredName + " %Swas queued as the next zombie.");
            } else if (args[0].CaselessEq("level")) {
                if (LevelInfo.ExistsOffline(value)) {
                    Player.Message(p, value + " was queued.");
                    Server.zombie.QueuedLevel = value.ToLower();
                    if (Server.zombie.CurLevel != null)
                        Server.zombie.CurLevel.ChatLevel(value + " was queued as the next map.");
                } else {
                    Player.Message(p, "Level \"" + value + "\" was not found.");
                }
            } else {
            	Help(p);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "/queue zombie [name] - Next round [name] will be infected");
            Player.Message(p, "/queue level [name] - Next round [name] will be the round loaded");
        }
    }
}
