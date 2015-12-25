using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YourRaidingBuddy.Helpers;

namespace YourRaidingBuddy.Interface.Extensions
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Allows the form to be dragged on locations which have this function set.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern bool ReleaseCapture();

        /// <summary>
        /// Requirements for Combobox Enumerations
        /// </summary>
        internal class YRBCboItem
        {
            public readonly int E;
            private readonly string _s;

            public YRBCboItem(int pe, string ps) { E = pe; _s = ps; }
            public override string ToString() { return _s; }
        }

        /// <summary>
        /// Gets the ComboBox enum.
        /// </summary>
        /// <param name="cb">The cb.</param>
        /// <returns></returns>
        internal static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (YRBCboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }

        /// <summary>
        /// Sets the ComboBox enum.
        /// </summary>
        /// <param name="cb">The cb.</param>
        /// <param name="e">The e.</param>
        internal static void SetComboBoxEnum(ComboBox cb, int e)
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

            Logger.WriteDebug("ExtensionMethods: (Dialog Error) Combobox {0} does not have enum({1}) in list, defaulting to enum({2})", cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }

        /// <summary>
        /// Imports the image as icons for Chrome tabcontrol.
        /// </summary>
        /// <param name="imageList">The image list.</param>
        /// <param name="tabControl">The tab control.</param>
        /// <param name="tabPage">The tab page.</param>
        /// <param name="imageResources">The image resources.</param>
        /// <param name="indexImageList">The index image list.</param>
        internal static void ImportImage(ImageList imageList, TabControl tabControl, TabPage tabPage, Icon imageResources, int indexImageList)
        {
            // Imagelist vorbereiten
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);
            imageList.TransparentColor = Color.Black; // transparente Farbe festlegen

            // Bild hinzufügen
            imageList.Images.Add(imageResources);

            // Bild für Button festlegen
            tabControl.ImageList = imageList;

            // Index des Bitmaps in der Imagelist
            tabPage.ImageIndex = indexImageList;
        }
    }
}