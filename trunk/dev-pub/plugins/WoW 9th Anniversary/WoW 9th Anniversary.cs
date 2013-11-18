using System;
using System.Linq;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.Plugins;
using Styx.WoWInternals.WoWObjects;

namespace WoW_9th_Anniversary
{
    public class WowAnniversaryPlugin : HBPlugin
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public override string Name
        {
            get { return "WowAnniversaryPlugin"; }
        }

        public override string Author
        {
            get { return "nomnomnom"; }
        }

        public override Version Version
        {
            get { return new Version(1, 5); }
        }

        public static bool IsViable(WoWObject wowObject)
        {
            return (wowObject != null) && wowObject.IsValid;
        }

        public bool AnniversaryBuff()
        {
            return IsViable(Me) && !Me.Mounted && !Me.IsOnTransport && !Me.IsResting && !Me.IsDead && !Me.HasAura(132700);
        }

        public override void Pulse()
        {
            WoWItem anniversaryitem = Me.BagItems.FirstOrDefault(item => item.Entry == 90918);

            if (!AnniversaryBuff() || anniversaryitem == null) 
                return;

            anniversaryitem.Use();
            Logging.Write(Colors.LemonChiffon, "WoW 9th Anniversary buff has been applied!");
        }
    }
}
