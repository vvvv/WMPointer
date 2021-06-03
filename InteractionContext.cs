using System;
using System.Runtime.InteropServices;

//https://www.codeproject.com/articles/607234/using-windows-interaction-con

namespace WMPointer.InteractionContext
{
    [StructLayout(LayoutKind.Explicit)]
    public struct INTERACTION_CONTEXT_OUTPUT
    {
        [FieldOffset(0)]
        public INTERACTION Interaction;
        [FieldOffset(4)]
        public INTERACTION_FLAGS InteractionFlags;
        [FieldOffset(8)]
        public INPUT_TYPE InputType;
        [FieldOffset(12)]
        public float X;
        [FieldOffset(16)]
        public float Y;
        [FieldOffset(20)]
        [MarshalAs(UnmanagedType.Struct)]
        public INTERACTION_ARGUMENTS_MANIPULATION Manipulation;
        [FieldOffset(20)]
        [MarshalAs(UnmanagedType.Struct)]
        public INTERACTION_ARGUMENTS_TAP Tap;
        [FieldOffset(20)]
        [MarshalAs(UnmanagedType.Struct)]
        public INTERACTION_ARGUMENTS_CROSS_SLIDE CrossSlide;
    }

    public enum INTERACTION
    {
        NONE            = 0x00000000,
        MANIPULATION    = 0x00000001,
        TAP             = 0x00000002,
        SECONDARY_TAP   = 0x00000003,
        HOLD            = 0x00000004,
        DRAG            = 0x00000005,
        CROSS_SLIDE     = 0x00000006
    }

    [Flags]
    public enum INTERACTION_FLAGS : uint
    {
        NONE = 0x00000000,
        BEGIN = 0x00000001,
        END = 0x00000002,
        CANCEL = 0x00000004,
        INERTIA = 0x00000008,
        MAX = 0xffffffff
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERACTION_ARGUMENTS_MANIPULATION
    {
        [MarshalAs(UnmanagedType.Struct)]
        public MANIPULATION_TRANSFORM Delta;
        [MarshalAs(UnmanagedType.Struct)]
        public MANIPULATION_TRANSFORM Cumulative;
        [MarshalAs(UnmanagedType.Struct)]
        public MANIPULATION_VELOCITY Velocity;
        public MANIPULATION_RAILS_STATE RailsState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MANIPULATION_TRANSFORM
    {
        public float TranslationX;
        public float TranslationY;
        public float Scale;
        public float Expansion;
        public float Rotation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MANIPULATION_VELOCITY
    {
        public float VelocityX;
        public float VelocityY;
        public float VelocityExpansion;
        public float VelocityAngular;
    }

    public enum MANIPULATION_RAILS_STATE
    {
        UNDECIDED   = 0x00000000,
        FREE        = 0x00000001,
        RAILED      = 0x00000002,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERACTION_ARGUMENTS_TAP
    {
        public int Count;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERACTION_ARGUMENTS_CROSS_SLIDE
    {
        public CROSS_SLIDE_FLAGS Flags;
    }

    [Flags]
    public enum CROSS_SLIDE_FLAGS
    {
        NONE = 0x00000000,
        SELECT = 0x00000001,
        SPEED_BUMP = 0x00000002,
        REARRANGE = 0x00000004
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERACTION_CONTEXT_CONFIGURATION
    {
        public INTERACTION Interaction;
        public INTERACTION_CONFIGURATION_FLAGS Enable;

        public INTERACTION_CONTEXT_CONFIGURATION(INTERACTION interaction, INTERACTION_CONFIGURATION_FLAGS enable)
        {
            Interaction = interaction;
            Enable = enable;
        }
    }

    [Flags]
    public enum INTERACTION_CONFIGURATION_FLAGS
    {
        NONE                            = 0x00000000,
        MANIPULATION                    = 0x00000001,
        MANIPULATION_TRANSLATION_X      = 0x00000002,
        MANIPULATION_TRANSLATION_Y      = 0x00000004,
        MANIPULATION_ROTATION           = 0x00000008,
        MANIPULATION_SCALING            = 0x00000010,
        MANIPULATION_TRANSLATION_INERTIA = 0x00000020,
        MANIPULATION_ROTATION_INERTIA   = 0x00000040,
        MANIPULATION_SCALING_INERTIA    = 0x00000080,
        MANIPULATION_RAILS_X            = 0x00000100,
        MANIPULATION_RAILS_Y            = 0x00000200,
        MANIPULATION_EXACT              = 0x00000400,
        CROSS_SLIDE                     = 0x00000001,
        CROSS_SLIDE_HORIZONTAL          = 0x00000002,
        CROSS_SLIDE_SELECT              = 0x00000004,
        CROSS_SLIDE_SPEED_BUMP          = 0x00000008,
        CROSS_SLIDE_REARRANGE           = 0x00000010,
        CROSS_SLIDE_EXACT               = 0x00000020,
        TAP                             = 0x00000001,
        TAP_DOUBLE                      = 0x00000002,
        SECONDARY_TAP                   = 0x00000001,
        HOLD                            = 0x00000001,
        HOLD_MOUSE                      = 0x00000002,
        DRAG                            = 0x00000001
    }

    public enum INTERACTION_STATE : uint
    {
        IDLE                = 0x00000000,
        IN_INTERACTION      = 0x00000001,
        POSSIBLE_DOUBLE_TAP = 0x00000002,
        STATE_MAX           = 0xffffffff
    }

    public enum INERTIA_PARAMETER
    {
        TRANSLATION_DECELERATION = 0x00000001,
        TRANSLATION_DISPLACEMENT = 0x00000002,
        ROTATION_DECELERATION    = 0x00000003,
        ROTATION_ANGLE           = 0x00000004,
        EXPANSION_DECELERATION   = 0x00000005,
        EXPANSION_EXPANSION      = 0x00000006
    }

    //[StructLayout(LayoutKind.Sequential)]
    //internal struct CROSS_SLIDE_PARAMETER
    //{
    //    public CROSS_SLIDE_THRESHOLD Threshold;
    //    public float Distance;
    //}

    //internal enum CROSS_SLIDE_THRESHOLD
    //{
    //    SELECT_START = 0x00000000,
    //    SPEED_BUMP_START = 0x00000001,
    //    SPEED_BUMP_END = 0x00000002,
    //    REARRANGE_START = 0x00000003,
    //    COUNT = 0x00000004
    //}

    public static class Win32
    {
        //setup
        [DllImport("ninput.dll", SetLastError = true)]
        public static extern int CreateInteractionContext(out IntPtr interactionContext);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern void StopInteractionContext(IntPtr interactionContext);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern void ResetInteractionContext(IntPtr interactionContext);

        [DllImport("ninput.dll", SetLastError = true)]
        public static extern int DestroyInteractionContext(IntPtr interactionContext);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void INTERACTION_CONTEXT_OUTPUT_CALLBACK(IntPtr clientData, IntPtr output);

        [DllImport("ninput.dll")]
        public static extern void RegisterOutputCallbackInteractionContext(IntPtr interactionContext, IntPtr callbackFunc, IntPtr clientData);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern void GetInteractionConfigurationInteractionContext(IntPtr interactionContext, int configurationCount, [MarshalAs(UnmanagedType.LPArray), In, Out] INTERACTION_CONTEXT_CONFIGURATION[] configuration);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern void SetInteractionConfigurationInteractionContext(IntPtr interactionContext, int configurationCount, [MarshalAs(UnmanagedType.LPArray), In] INTERACTION_CONTEXT_CONFIGURATION[] configuration);

        //using
        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern int AddPointerInteractionContext(IntPtr interactionContext, uint pointerId);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern int RemovePointerInteractionContext(IntPtr interactionContext, uint pointerId);
        

        [DllImport("ninput.dll", PreserveSig = false)]
        internal static extern void GetStateInteractionContext(IntPtr interactionContext, IntPtr pointerInfo, out INTERACTION_STATE state);

        [DllImport("ninput.dll")]
        public static extern IntPtr ProcessPointerFramesInteractionContext(IntPtr interactionContext, int entriesCount, int pointerCount, [MarshalAs(UnmanagedType.LPArray), In] POINTER_INFO[] pointerInfo);

        //inertia
        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern int GetInertiaParameterInteractionContext(IntPtr interactionContext, INERTIA_PARAMETER inertiaParameter, out float value);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern int SetInertiaParameterInteractionContext(IntPtr interactionContext, INERTIA_PARAMETER inertiaParameter, float value);

        [DllImport("ninput.dll", PreserveSig = false)]
        public static extern void ProcessInertiaInteractionContext(IntPtr interactionContext);
    }
}
