using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YourRaidingBuddy.Interfaces.GUI.Extentions
{
    internal static class Extention2
    {
        /// <summary>
        /// Allows the form to be dragged on locations which have this function set.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern bool ReleaseCapture();

        /// <summary>
        /// Requirements for Combobox Enumserations
        /// </summary>
        internal class YRBCboItem
        {
            public readonly int E;
            private readonly string _s;

            public YRBCboItem(int pe, string ps) { E = pe; _s = ps; }
            public override string ToString() { return _s; }
        }



        internal static int GetComboBoxEnums(ComboBox cb)
        {
            var item = (YRBCboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }

        internal static void SetComboBoxEnums(ComboBox cb, int e)
        {
            YRBCboItem item;
            for (var i = 0; i < cb.Items.Count; i++)
            {
                item = (YRBCboItem)cb.Items[i];
                if (item.E != e) continue;
                cb.SelectedIndex = i;
                return;
            }
            item = (YRBCboItem)cb.Items[0];
          //  Logger.DiagnosticLog("Dialog Error: Combobox {0} does not have Enums({1}) in list, defaulting to Enums({2})", true, cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }

        /// <summary>
        /// This class allows you to define the location of the spreader-bar in the Property Grids.
        /// </summary>
        internal static int GetInternalLabelWidth(this PropertyGrid propertyGrid)
        {
            object gridView = GetPropertyGridView(propertyGrid);

            PropertyInfo propInfo = gridView.GetType().GetProperty("InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)propInfo.GetValue(gridView);
        }

        internal static void MoveSplitterTo(this PropertyGrid propertyGrid, int xpos)
        {
            object gridView = GetPropertyGridView(propertyGrid);

            MethodInfo methodInfo = gridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(gridView, new object[] { xpos });
        }

        private static object GetPropertyGridView(PropertyGrid propertyGrid)
        {
            MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
            return methodInfo.Invoke(propertyGrid, new object[] { });
        }
    }
}