/*
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
using MCGalaxy.Bots;

namespace MCGalaxy.Commands {
    
    public class CmdModel : Command {
        
        public override string name { get { return "model"; } }
        public override string shortcut { get { return "setmodel"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") {
                if (p == null) {
                    Player.Message(p, "Console must provide a player or bot name."); return;
                }
                message = p.name;
            }
            
            Player who = p;
            PlayerBot pBot = null;
            bool isBot = message.CaselessStarts("bot ");
            string[] args = message.Split(trimChars, isBot ? 3 : 2);
            string model = null;

            if (isBot && args.Length > 2) {
                pBot = PlayerBot.FindOrShowMatches(p, args[1]);
                if (pBot == null) return;
                model = args[2];
            } else if (args.Length > 1) {
                isBot = false;
                who = PlayerInfo.FindOrShowMatches(p, args[0]);
                if (who == null) return;
                model = args.Length >= 2 ? args[1] : "humanoid";
            } else {
                isBot = false;
                who = p;
                if (who == null) { Player.Message(p, "Console must provide a player name."); return; }
                model = message;
            }
            model = model.ToLower();

            if (isBot) {
                pBot.model = model;
                Entities.UpdateModel(pBot.id, model, pBot.level, null);
                Player.GlobalMessage("Bot " + pBot.name + "'s %Smodel was changed to a &c" + model);
                BotsFile.UpdateBot(pBot);
            } else {
                who.model = model;
                Entities.UpdateModel(who.id, model, who.level, who);
                if (p != who)
                    Player.GlobalMessage(who.ColoredName + "'s %Smodel was changed to a &c" + model);
                
                Server.Models.DeleteStartsWith(who.name + " ");
                if (model != "humanoid")
                    Server.Models.Append(who.name + " " + model);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "/model [name] [model] - Sets the model of that player.");
            Player.Message(p, "/model bot [name] [model] - Sets the model of that bot.");
            HelpModels(p);
        }
        
        protected void HelpModels(Player p) {
            Player.Message(p, "Available models: Chibi, Chicken, Creeper, Giant, Humanoid, Pig, Sheep, Spider, Skeleton, Zombie.");
            Player.Message(p, "To set a block model, use a block ID for the model name.");
            Player.Message(p, "For setting scaling models, put \"|[scale]\" after the model name (not supported by all clients).");
        }
    }
    
    public class CmdXModel : CmdModel {
        
        public override string name { get { return "xmodel"; } }
        public override string shortcut { get { return "xm"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); }
            string model = message == "" ? "humanoid" : message;
            base.Use(p, p.name + " " + model);
        }

        public override void Help(Player p) {
            Player.Message(p, "/xm [model] - Sets your own model.");
            HelpModels(p);
        }
    }
}
