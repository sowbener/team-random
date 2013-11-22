using System.IO;
using System.Windows.Forms;
using Styx;
using Styx.Common;
using Styx.Helpers;

namespace Tyrael.Shared
{
    public sealed class TyraelSettings : Settings
    {
        private static TyraelSettings _instance;

        public static TyraelSettings Instance
        {
            get
            {
                return _instance ?? (_instance = new TyraelSettings());
            }
        }

        public TyraelSettings()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/Tyrael/Tyrael-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, Tyrael.Revision)))
        {
        }

        #region UI Settings
        [Setting, DefaultValue(true)]
        public bool ChatOutput { get; set; }

        [Setting, DefaultValue(true)]
        public bool ClickToMove { get; set; }

        [Setting, DefaultValue(false)]
        public bool HealingMode { get; set; }

        [Setting, DefaultValue(false)]
        public bool PluginPulsing { get; set; }

        [Setting, DefaultValue(false)]
        public bool ScaleTps { get; set; }

        //[Setting, DefaultValue(30)]
        //public int HonorbuddyTps { get; set; }

        //[Setting, DefaultValue(TyraelUtilities.LockState.True)]
        //public TyraelUtilities.LockState FrameLock { get; set; }

        [Setting, DefaultValue(ModifierKeys.Alt)]
        public ModifierKeys ModKeyChoice { get; set; }

        [Setting, DefaultValue(Keys.X)]
        public Keys PauseKeyChoice { get; set; }
        #endregion

        #region Manual Settings
        // You can change these values in the XML file.
        [Setting, DefaultValue(1000)]
        public int ScaleRefresh { get; set; }

        [Setting, DefaultValue(false)]
        public bool ScaleLogging { get; set; }

        [Setting, DefaultValue(false)]
        public bool TreePerformanceLogging { get; set; }

        [Setting, DefaultValue(Keys.F12)]
        public Keys TreePerformanceChoice { get; set; }

        [Setting, DefaultValue("")]
        public string LastStatCounted { get; set; }
        #endregion
    }
}
