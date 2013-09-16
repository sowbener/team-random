﻿using System.Reflection;
using System.Windows.Forms;

namespace DeathVader.Interfaces.GUI
{
    public static class DvPgExtention
    {
        private static object GetPropertyGridView(PropertyGrid propertyGrid)
        { 
            MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
            return methodInfo.Invoke(propertyGrid, new object[] {});
        }

        public static int GetInternalLabelWidth(this PropertyGrid propertyGrid)
        {
            object gridView = GetPropertyGridView(propertyGrid);

            PropertyInfo propInfo = gridView.GetType().GetProperty("InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)propInfo.GetValue(gridView);
        }

        public static void MoveSplitterTo(this PropertyGrid propertyGrid, int xpos)
        {
            object gridView = GetPropertyGridView(propertyGrid);

            MethodInfo methodInfo = gridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(gridView, new object[] { xpos });
        }
    }
}