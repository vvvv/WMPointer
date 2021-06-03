using System;
using System.Runtime.InteropServices;

namespace WMPointer
{
    public class WMEventArgs : EventArgs
    {
        public WMEventArgs(IntPtr hWnd, int msg, UIntPtr wParam, IntPtr lParam)
        {
            HWnd = hWnd;
            Message = msg;
            WParam = wParam;
            LParam = lParam;
        }

        public readonly IntPtr HWnd;
        public readonly int Message;
        public readonly UIntPtr WParam;
        public readonly IntPtr LParam;
        public bool Handled;
    }

    public enum INPUT_TYPE
    {
        NONE    = 0x00000000,
        POINTER = 0x00000001,
        TOUCH   = 0x00000002,
        PEN     = 0x00000003,
        MOUSE   = 0x00000004,
        TOUCHPAD= 0x00000005
    }

    [Flags]
    public enum PDC_FLAGS
    {
        PDC_ARRIVAL         = 0x001,
        PDC_REMOVAL         = 0x002,
        PDC_ORIENTATION_0   = 0x004,
        PDC_ORIENTATION_90  = 0x008,
        PDC_ORIENTATION_180 = 0x010,
        PDC_ORIENTATION_270 = 0x020,
        PDC_MODE_DEFAULT    = 0x040,
        PDC_MODE_CENTERED   = 0x080,
        PDC_MAPPING_CHANGE  = 0x100,
        PDC_RESOLUTION      = 0x200,
        PDC_ORIGIN          = 0x400,
        PDC_MODE_ASPECTRATIOPRESERVED = 0x800
    }

    public enum WMPointerMessage
    {
        POINTERDEVICECHANGE = 0x0238,
        POINTERDEVICEINRANGE = 0x239,
        POINTERDEVICEOUTOFRANGE = 0x23A,

        NCPOINTERUPDATE = 0x0241,           //577
        NCPOINTERDOWN = 0x0242,             //578
        NCPOINTERUP = 0x0243,               //579

        POINTERUPDATE = 0x0245,             //581
        POINTERDOWN = 0x0246,               //582
        POINTERUP = 0x0247,                 //583
        
        POINTERENTER = 0x0249,              //585
        POINTERLEAVE = 0x024A,              //586
                         
        POINTERACTIVATE = 0x024B,           //587
        POINTERCAPTURECHANGED = 0x024C,     //588

        TOUCHHITTESTING = 0x024D,           //589
        POINTERWHEEL = 0x024E,              //590
        POINTERHWHEEL = 0x024F,             //591
        
        DM_POINTERHITTEST = 0x0250
    }

    public static partial class Win32
    {
        public static uint ToId(UIntPtr wParam) => (uint)wParam & 0xFFFF;

        public static MESSAGE_FLAGS ToMessageFlags(UIntPtr wParam) => (MESSAGE_FLAGS)(wParam.ToUInt32() >> 16);

        public static int ToRawX(IntPtr lParam) => lParam.ToInt32() & 0xFFFF;

        public static int ToRawY(IntPtr lParam) => lParam.ToInt32() >> 16;

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int EnableMouseInPointer(bool enable);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerCursorId(uint pointerID, out uint cursorId);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerType(uint pointerID, out INPUT_TYPE pointerType);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern void GetUnpredictedMessagePos();

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool IsMouseInPointerEnabled();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SkipPointerFrameMessages(uint pointerID);
    }
}