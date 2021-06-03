using System;
using WMPointer.InteractionContext;

namespace WMPointer
{
    [Flags]
    public enum ManipulationFlags
    {
        TranslationX = 0x00000002,
        TranslationY = 0x00000004,
        Rotation = 0x00000008,
        Scaling = 0x00000010,
        All = 31,
        TranslationInertia = 0x00000020,
        RotationInertia = 0x00000040,
        ScalingInertia = 0x00000080,
        AllInertia = 255,
        RailsX = 0x00000100,
        RailsY = 0x00000200,
        Exact = 0x00000400,
    }

    public class InteractionConfiguration
    {
        public static INTERACTION_CONFIGURATION_FLAGS ManipulationAll = (INTERACTION_CONFIGURATION_FLAGS)31;
        public static INTERACTION_CONFIGURATION_FLAGS ManipulationAllInertia = (INTERACTION_CONFIGURATION_FLAGS)255;

        public INTERACTION_CONTEXT_CONFIGURATION[] Configuration { get; private set; }
        public INTERACTION_CONFIGURATION_FLAGS Manipulation
        {
            get { return Configuration[0].Enable; }
            set { Configuration[0].Enable = value; }
        }

        public InteractionConfiguration()
        {
            Configuration = new INTERACTION_CONTEXT_CONFIGURATION[]
            {
                new INTERACTION_CONTEXT_CONFIGURATION(INTERACTION.MANIPULATION, ManipulationAllInertia),
                new INTERACTION_CONTEXT_CONFIGURATION(INTERACTION.TAP, INTERACTION_CONFIGURATION_FLAGS.TAP | INTERACTION_CONFIGURATION_FLAGS.TAP_DOUBLE),
                new INTERACTION_CONTEXT_CONFIGURATION(INTERACTION.SECONDARY_TAP, INTERACTION_CONFIGURATION_FLAGS.SECONDARY_TAP)
            };
        }
    }
}
