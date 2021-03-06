/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    
    public class CmdTColor : Command {
        
        public override string name { get { return "tcolor"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdTColor() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            
            Player who = PlayerInfo.FindOrShowMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.group.Permission > p.group.Permission) {
                Player.Message(p, "Cannot change the title color of someone of greater rank"); return;
            }
            
            ParameterisedQuery query = ParameterisedQuery.Create();
            if (args.Length == 1) {                
                Player.SendChatFrom(who, who.ColoredName + " %Shad their title color removed.", false);
                who.titlecolor = "";
                
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET title_color = '' WHERE Name = @Name");
            } else  {
                string color = Colors.Parse(args[1]);
                if (color == "") { Player.Message(p, "There is no color \"" + args[1] + "\"."); return; }
                else if (color == who.titlecolor) { Player.Message(p, who.DisplayName + " already has that title color."); return; }
                Player.SendChatFrom(who, who.ColoredName + " %Shad their title color changed to " + color + Colors.Name(color) + "%S.", false);
                who.titlecolor = color;
                
                query.AddParam("@Color", Colors.Name(color));
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET title_color = @Color WHERE Name = @Name");                
            }
            who.SetPrefix();
        }

        public override void Help(Player p) {
            Player.Message(p, "/tcolor <player> [color] - Gives <player> the title color of [color].");
            Player.Message(p, "If no [color] is specified, title color is removed.");
            HelpColors(p);
        }
        
        protected void HelpColors(Player p) {
            Player.Message(p, "&0black &1navy &2green &3teal &4maroon &5purple &6gold &7silver");
            Player.Message(p, "&8gray &9blue &alime &baqua &cred &dpink &eyellow &fwhite");
            Player.Message(p, "To see a list of all colors, use /help colors.");
        }
    }
    
    public sealed class CmdXTColor : CmdTColor {
        
        public override string name { get { return "xtcolor"; } }
        public override string shortcut { get { return ""; } }
        public CmdXTColor() { }

        public override void Use(Player p, string message) {
            if (message != "") message = " " + message;
            base.Use(p, p.name + message);
        }

        public override void Help(Player p) {
            Player.Message(p, "/xtcolor [color] - Gives you the title color of [color].");
            Player.Message(p, "If no [color] is specified, title color is removed.");
            HelpColors(p);
        }
    }
}
