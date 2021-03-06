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
using System;
using System.Globalization;
namespace MCGalaxy.Commands
{
    public sealed class CmdTake : Command
    {
        public override string name { get { return "take"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }        
        public CmdTake() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }

            string taker = null, takerRaw = null;
            if (p == null) { takerRaw = "(console)"; taker = "(console)"; }
            else { takerRaw = p.color + p.name; taker = p.ColoredName; }

            int amount = 0;
            bool all = args[1].CaselessEq("all");
            if (!all && !int.TryParse(args[1], out amount)) {
                Player.Message(p, "Amount must be an integer."); return;
            }
            if (amount < 0) { Player.Message(p, "%cYou can't take negative %3" + Server.moneys); return; }
            
            int matches = 1;
            Player who = PlayerInfo.FindOrShowMatches(p, args[0], out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "%cYou can't take %3" + Server.moneys + "%c from yourself"); return; }
            
            Economy.EcoStats ecos;
            if (who == null) {
                OfflinePlayer off = PlayerInfo.FindOffline(args[0]);
                if (off == null) { Player.Message(p, "The player \"&a" + args[0] + "%S\" was not found at all."); return; }
                ecos = Economy.RetrieveEcoStats(args[0]);
                Take(all, ref ecos, ref amount);
                Player.GlobalMessage(taker + " %Stook %f" + amount + " %3" + Server.moneys + " %Sfrom " + off.color + off.name + "%f(offline)");
            } else {
                ecos = Economy.RetrieveEcoStats(who.name);
                ecos.money = who.money;
                Take(all, ref ecos, ref amount);
                who.money = ecos.money;
                who.OnMoneyChanged();
                Player.GlobalMessage(taker + " %Stook %f" + amount + " %3" + Server.moneys + " %Sfrom " + who.prefix + who.name);
            }
            ecos.fine = "%f" + amount + " %3" + Server.moneys + " by " + takerRaw + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateEcoStats(ecos);
        }
        
        static void Take(bool all, ref Economy.EcoStats ecos, ref int amount) {
            if (all || ecos.money < amount) {
                amount = ecos.money;
                ecos.money = 0;
            } else {
                ecos.money -= amount;
            }
        }
        
        public override void Help(Player p){
            Player.Message(p, "&f/take [player] <amount> %S- Takes <amount> of " + Server.moneys + " from [player]");
            Player.Message(p, "&f/take [player] all %S- Takes all the " + Server.moneys + " from [player]");
        }
    }
}
