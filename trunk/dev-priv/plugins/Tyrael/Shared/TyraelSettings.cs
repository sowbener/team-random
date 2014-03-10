using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using System.Windows.Forms;

namespace Tyrael.Shared
{
    public sealed class TyraelSettings : Settings
    {
        private static TyraelSettings _instance;

        public static TyraelSettings Instance
        {
            get { return _instance ?? (_instance = new TyraelSettings()); }
        }

        public TyraelSettings()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/Tyrael/Tyrael-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, Tyrael.Revision)))
        {
        }

        #region UI Settings
        [Setting, DefaultValue(false)]
        public bool CheckAutoUpdate { get; set; }

        [Setting, DefaultValue(true)]
        public bool CheckChatOutput { get; set; }

        [Setting, DefaultValue(true)]
        public bool CheckClickToMove { get; set; }

        [Setting, DefaultValue(false)]
        public bool CheckHealingMode { get; set; }

        [Setting, DefaultValue(false)]
        public bool CheckPluginPulsing { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseSoftLock { get; set; }

        [Setting, DefaultValue(TyraelUtilities.SvnUrl.Release)]
        public TyraelUtilities.SvnUrl SvnUrl { get; set; }

        [Setting, DefaultValue(Keys.X)]
        public Keys PauseKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKeys.Alt)]
        public ModifierKeys ModKeyChoice { get; set; }
        #endregion

        #region Manual Settings
        [Setting, DefaultValue(false)]
        public bool CheckRaidWarningOutput { get; set; }

        [Setting, DefaultValue(0)]
        public int CurrentRevision { get; set; }

        [Setting, DefaultValue("")]
        public string LastStatCounted { get; set; }
        #endregion
    }
}
