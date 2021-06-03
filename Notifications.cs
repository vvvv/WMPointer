using System;

namespace WMPointer
{
    public class PointerNotification
    {
        public readonly object Sender;
        public readonly WMExtension Message;
        public readonly IntPtr HWnd;
        public readonly uint Id;
        public readonly INPUT_TYPE PointerType;
        public readonly int RawX;
        public readonly int RawY;
        public readonly MESSAGE_FLAGS MessageFlags;

        public PointerNotification(object sender, WMExtension message, IntPtr hWnd, UIntPtr wParam, IntPtr lParam)
        {
            Sender = sender;
            Message = message;
            HWnd = hWnd;
            Id = (uint)wParam & 0xFFFF;

            INPUT_TYPE pointerType = INPUT_TYPE.NONE;
            if (Pointer.GetPointerType(Id, out pointerType))
                PointerType = pointerType;

            var lParamInt = lParam.ToInt32();
            RawX = lParamInt & 0xFFFF;
            RawY = lParamInt >> 16;
            MessageFlags = (MESSAGE_FLAGS)(wParam.ToUInt32() >> 16);
        }

        public static bool IsPointerNotification(int windowMessage)
        {
            return
                (windowMessage >= (int)WMExtension.POINTERUPDATE) && (windowMessage <= (int)WMExtension.POINTERLEAVE)
                || (windowMessage == (int)WMExtension.POINTERWHEEL)
                || (windowMessage == (int)WMExtension.POINTERHWHEEL);

        }
    }
}
