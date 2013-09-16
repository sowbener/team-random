using System;
using Styx.Plugins;
using System.Linq;
using System.Threading;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx;
using Styx.CommonBot;
using System.Diagnostics;

namespace MrFetcher
{
    public class MrFetcher : HBPlugin
    {
	 int teller=0;

        public override void Pulse()
        {
		if (!SpellManager.GlobalCooldown && Styx.StyxWoW.Me.IsAlive && !Styx.StyxWoW.Me.Combat && SpellManager.CanCast("Fetch")){
        WoWUnit ret = (from unit in ObjectManager.GetObjectsOfType<WoWUnit>(true, true)
                       orderby unit.Distance ascending
                       where unit.Lootable
                       where unit.IsDead
					   where unit.InLineOfSight
                       where unit.Distance < 50
                       select unit).FirstOrDefault();
					   
			if (ret != null){

			    ret.Target();
				WoWMovement.MoveStop();
			if (SpellManager.CanCast("Fetch")) {
			 SpellManager.Cast("Fetch");  //Lua.DoString("RunMacroText(\"/use Loot-A-Rang\");");
			
			
			
			Stopwatch timer = new Stopwatch();
			timer.Reset();
			timer.Start();
			while (!Styx.StyxWoW.Me.Combat && ret.Lootable && timer.ElapsedMilliseconds < 2500){  //StyxWoW.Me.Pet.Location.Distance(ret.Location) > 5
			}
			}

		   
		   
		   
        }
		}
		}

		

        public override string Name { get { return "MrFetcher"; } }

        public override string Author { get { return "maybe"; } }

        public override Version Version { get { return new Version(1,0,0,0);} }
    }
}