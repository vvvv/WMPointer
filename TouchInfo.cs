using System;
using System.Runtime.InteropServices;

namespace WMPointer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINTER_TOUCH_INFO
    {
        public POINTER_INFO pointerInfo;
        public TOUCH_FLAGS  touchFlags;
        public TOUCH_MASK   touchMask;
        public int rcContactLeft;
        public int rcContactTop;
        public int rcContactRight;
        public int rcContactBottom;
        public int rcContactRawLeft;
        public int rcContactRawTop;
        public int rcContactRawRight;
        public int rcContactRawBottom;
        public uint orientation;
        public uint pressure;
    }

    [Flags]
    public enum TOUCH_FLAGS
    {
        TOUCH_FLAG_NONE = 0x00000000
    }

    [Flags]
    public enum TOUCH_MASK
    {
        NONE = 0x00000000,
        CONTACTAREA = 0x00000001,
        ORIENTATION = 0x00000002,
        PRESSURE = 0x00000004,
    }

    public static partial class Win32
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerTouchInfo(uint pointerID, out POINTER_TOUCH_INFO touchInfo);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerTouchInfoHistory(uint pointerID, ref int entriesCount, IntPtr zero);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerTouchInfoHistory(uint pointerID, ref int entriesCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_TOUCH_INFO[] touchInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameTouchInfo(uint pointerID, ref int pointerCount, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameTouchInfo(uint pointerID, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_TOUCH_INFO[] touchInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameTouchInfoHistory(uint pointerID, ref int entriesCount, ref int pointerCount, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameTouchInfoHistory(uint pointerID, ref int entriesCount, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_TOUCH_INFO[] touchInfo);
    }
}
