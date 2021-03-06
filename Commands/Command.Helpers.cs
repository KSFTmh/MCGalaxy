﻿/*
    Copyright 2015 MCGalaxy

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
using System.Text;

namespace MCGalaxy {
    
    public abstract partial class Command {

        const CommandEnable bothFlags = CommandEnable.Lava | CommandEnable.Zombie;
        public static string GetDisabledReason(CommandEnable enable) {
            if (enable == CommandEnable.Always) return null;
            if (enable == CommandEnable.Economy && !Economy.Enabled)
                return "economy is disabled.";
            
            if (enable == bothFlags && !(Server.zombie.Running || Server.lava.active))
                return "neither zombie nor lava survival is running.";
            if (enable == CommandEnable.Zombie && !Server.zombie.Running)
                return "zombie survival is not running.";
            if (enable == CommandEnable.Lava)
                return "lava survival is not running.";
            return null;
        }
        
        protected static void RevertAndClearState(Player p, ushort x, ushort y, ushort z) {
            p.ClearBlockchange();
            p.RevertBlock(x, y, z);
        }
        
        protected void MessageInGameOnly(Player p) {
            Player.Message(p, "/" + name + " can only be used in-game.");
        }
        
        protected bool CheckAdditionalPerm(Player p, int num = 1) {
            return p == null || (int)p.group.Permission >= CommandOtherPerms.GetPerm(this, num);
        }
        
        protected void MessageNeedPerms(Player p, string action, int num = 1) {
            int perm = CommandOtherPerms.GetPerm(this, num);
            MessageNeedMinPerm(p, action, perm);
        }
        
        protected void MessageNeedMinPerm(Player p, string action, int perm) {
            Group grp = Group.findPermInt(perm);
            if (grp == null)
                Player.Message(p, "Only ranks with permissions greater than &a" + perm + "%Scan " + action);
            else
                Player.Message(p, "Only " + grp.ColoredName + "%S+ can " + action);
        }
        
        protected void MessageTooHighRank(Player p, string action, bool canAffectOwnRank) {
            MessageTooHighRank(p, action, p.group, canAffectOwnRank);
        }
        
        protected void MessageTooHighRank(Player p, string action, Group grp, bool canAffectGroup) {
            if (canAffectGroup)
                Player.Message(p, "Can only " + action + " players ranked " + grp.ColoredName + " %Sor below");
            else
                Player.Message(p, "Can only " + action + " players ranked below " + grp.ColoredName);
        }
        
        internal void MessageCannotUse(Player p) {
            var perms = GrpCommands.allowedCommands.Find(C => C.commandName == name);
            if (perms.disallow.Contains(p.group.Permission)) {
                Player.Message(p, "Your rank cannot use /%T" + name); return;
            }
            
            StringBuilder builder = new StringBuilder("Only ");            
            if (perms.allow.Count > 0) {
                foreach (LevelPermission perm in perms.allow) {
            		Group grp = Group.findPermInt((int)perm);
                    if (grp == null) continue;
                    builder.Append(grp.ColoredName).Append("%S, ");
                }
            	if (builder.Length > "Only ".Length) {
                    builder.Remove(builder.Length - 2, 2);
                    builder.Append(", and ");
            	}
            }
            
            Group minGrp = Group.findPermInt((int)perms.lowestRank);
            if (minGrp == null)
                builder.Append("ranks with permissions greater than &a" + (int)perms.lowestRank + "%S");
            else
                builder.Append(minGrp.ColoredName + "%S+");
            builder.Append(" can use %T/" + name);
            Player.Message(p, builder.ToString());
        }
    }
    
    public sealed class CommandTypes {
        public const string Building = "build";
        public const string Chat = "chat";
        public const string Economy = "economy";
        public const string Games = "game";
        public const string Information = "information";
        public const string Moderation = "mod";
        public const string Other = "other";
        public const string World = "world";
    }
}
