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
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Commands {
    
    public sealed class CmdBrush : Command {
        public override string name { get { return "brush"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") {
                Player.Message(p, "Your current brush is: " + p.BrushName); return;
            }
            string[] args = message.Split(trimChars, 2);           
            string brush = FindBrush(args[0]);
            
            if (brush == null) {
                if (message.CaselessStarts("help")) {
                    HandleHelp(p, args);
                } else {
                    Player.Message(p, "No brush found with name \"" + args[0] + "\".");
                    Player.Message(p, "Available brushes: " + AvailableBrushes);
                }
            } else {
                Player.Message(p, "Set your brush to: " + brush);
                p.BrushName = brush;
                p.DefaultBrushArgs = args.Length > 1 ? args[1] : "";
            }
        }
        
        void HandleHelp(Player p, string[] args) {
            if (args.Length != 2) { Help(p); return; }
            
            string brush = FindBrush(args[1]);
            if (brush == null) {
                Player.Message(p, "No brush found with name \"" + args[1] + "\".");
                Player.Message(p, "Available brushes: " + AvailableBrushes);
            } else {
                string[] help = Brush.BrushesHelp[brush];
                foreach (string line in help)
                    Player.Message(p, line);
            }
        }
        
        internal static string FindBrush(string message) {
            foreach (var brush in Brush.Brushes) {
                if (brush.Key.CaselessEq(message))
                    return brush.Key;
            }
            return null;
        }
        
        internal static string AvailableBrushes {
            get { return string.Join( ", ", Brush.Brushes.Keys); }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/brush [name] <default brush args>");
            Player.Message(p, "%HSets your current brush to the brush with the given name.");
            Player.Message(p, "%T/brush help [name]");
            Player.Message(p, "%HOutputs the help for the brush with the given name.");
            Player.Message(p, "Available brushes: " + AvailableBrushes);
            Player.Message(p, "%HThe default brush simply takes one argument specifying the block to draw with. " +
                               "If no arguments are given, your currently held block is used instead.");
        }
    }
}
