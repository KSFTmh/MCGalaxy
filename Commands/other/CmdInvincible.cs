/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdInvincible : Command
    {
        public override string name { get { return "invincible"; } }
        public override string shortcut { get { return "inv"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdInvincible() { }

        public override void Use(Player p, string message)
        {
        	Player who = message == "" ? p : PlayerInfo.FindOrShowMatches(p, message);
        	if (who == null) return;

            if (p != null && who.group.Permission > p.group.Permission) {
                MessageTooHighRank(p, "toggle invinciblity", true); return;
            }

            if (who.invincible)
            {
                who.invincible = false;
                if(p != null && who == p)
                {
                	Player.Message( p, "You are no longer invincible.");
                }
                else
                {
                	Player.Message(p, who.ColoredName + " %Sis no longer invincible.");
                }
                
                if (Server.cheapMessage && !who.hidden)
                    Player.SendChatFrom(who, who.ColoredName + " %Shas stopped being immortal", false);
            }
            else
            {
            	if(p != null && who == p)
                {
                	Player.Message( p, "You are now invincible.");
                }
                else
                {
            		Player.Message( p, who.ColoredName + " %Sis now invincible.");
                }
                who.invincible = true;
                if (Server.cheapMessage && !who.hidden)
                    Player.SendChatFrom(who, who.ColoredName + " %S" + Server.cheapMessageGiven, false);
            }
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/invincible [name] - Turns invincible mode on/off.");
            Player.Message(p, "If [name] is given, that player's invincibility is toggled");
            Player.Message(p, "/inv = Shortcut.");
        }
    }
}
