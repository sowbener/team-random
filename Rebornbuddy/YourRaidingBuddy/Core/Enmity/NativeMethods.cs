// This file has been cleaned up to use with YRB, taken from https://github.com/xtuaok/ACT_EnmityPlugin

using System;
using System.Runtime.InteropServices;

namespace YourRaidingBuddy.Helpers
{
    static class NativeMethods
    {
        // ReadProcessMemory
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);
    }
}
