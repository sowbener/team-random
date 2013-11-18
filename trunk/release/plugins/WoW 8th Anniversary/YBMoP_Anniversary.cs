using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using Styx.Plugins;
using Styx.WoWInternals.WoWObjects;
using Styx;
using Styx.WoWInternals;
using Styx.Common;
using System.Windows.Media;

namespace WowAnniversary
{
    public class WowAnniversaryPlugin : HBPlugin
    {
        public override string Name { get { return "WowAnniversaryPlugin"; } }
        public override string Author { get { return "nomnomnom"; } }
        public override Version Version { get { return new Version(1, 4); } }
        public static LocalPlayer Me = StyxWoW.Me;

        public bool AnniversaryBuff()
        {
            if (!Me.HasAura(100951) && !Me.Mounted && !Me.IsOnTransport && !Me.IsResting && !Me.IsDead) return true;
            else return false;
        }

        public override void Pulse()
        {
            foreach (WoWItem CelebPack in StyxWoW.Me.BagItems)
            {
                if (CelebPack.Name.Contains("Celebration Package") && AnniversaryBuff())
                {
                    Lua.DoString("RunMacroText(\"/use Celebration Package\")");
                    Logging.Write(Colors.Green, "Celebration Package used.");
                }
                if (CelebPack.Name.Contains("Paquet cadeau festif") && AnniversaryBuff())
                {
                    Lua.DoString("RunMacroText(\"/use Paquet cadeau festif\")");
                    Logging.Write(Colors.Green, "Buff 8eme anniversaire de WoW appliqué");
                }
                if (CelebPack.Name.Contains("Feiertagspaket") && AnniversaryBuff())
                {
                    Lua.DoString("RunMacroText(\"/use Feiertagspaket\")");
                    Logging.Write(Colors.Green, "Feiertagspaket benutzt.");
                }
                if (CelebPack.Name.Contains("Праздничный сверток") && AnniversaryBuff())
                {
                    Lua.DoString("RunMacroText(\"/use Праздничный сверток\")");
                    Logging.Write(Colors.Green, "Праздничный сверток used.");
                }
                if (CelebPack.Name.Contains("Paquete de celebración") && AnniversaryBuff())
                {
                    Lua.DoString("RunMacroText(\"/use Paquete de celebración\")");
                    Logging.Write(Colors.Green, "Buff 8°aniversario de WoW used.");
                }
            }
        }

    }
}