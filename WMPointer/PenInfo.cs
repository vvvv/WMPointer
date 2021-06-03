using System;
using System.Runtime.InteropServices;

namespace WMPointer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINTER_PEN_INFO
    {
        public POINTER_INFO pointerInfo;
        public PEN_FLAGS penFlags;
        public PEN_MASK penMask;
        public uint pressure;
        public uint rotation;
        public int tiltX;
        public int tiltY;
    }

    [Flags]
    public enum PEN_FLAGS
    {
        PEN_FLAG_NONE = 0x00000000,
        PEN_FLAG_BARREL = 0x00000001,
        PEN_FLAG_INVERTED = 0x00000002,
        PEN_FLAG_ERASER = 0x00000004,
    }

    [Flags]
    public enum PEN_MASK
    {
        PEN_MASK_NONE = 0x00000000,
        PEN_MASK_PRESSURE = 0x00000001,
        PEN_MASK_ROTATION = 0x00000002,
        PEN_MASK_TILT_X = 0x00000004,
        PEN_MASK_TILT_Y = 0x00000008
    }

    public static partial class Win32
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerPenInfo(uint pointerID, out POINTER_PEN_INFO penInfo);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerPenInfoHistory(uint pointerID, ref int entriesCount, IntPtr zero);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerPenInfoHistory(uint pointerID, ref int entriesCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_PEN_INFO[] penInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFramePenInfo(uint pointerID, ref int pointerCount, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFramePenInfo(uint pointerID, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_INFO[] penInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFramePenInfoHistory(uint pointerID, ref int entriesCount, ref int pointerCount, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFramePenInfoHistory(uint pointerID, ref int entriesCount, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_INFO[] penInfo);
    }
}
