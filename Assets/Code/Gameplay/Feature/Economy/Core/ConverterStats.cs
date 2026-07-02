namespace Otus.Converters.Core
{
    /// <summary>
    /// Immutable snapshot of a converter's runtime parameters at a given level.
    /// Produced by <see cref="ConverterConfig.GetStats"/> — the config is the
    /// Information Expert that owns these curves.
    /// </summary>
    public readonly struct ConverterStats
    {
        /// <summary>Seconds between automatic production cycles.</summary>
        public readonly float CycleSeconds;

        /// <summary>Resource consumed each cycle (only when <see cref="HasInput"/> is true).</summary>
        public readonly ResourceId InputResource;

        /// <summary>Amount of <see cref="InputResource"/> consumed each cycle.</summary>
        public readonly int InputAmount;

        /// <summary>Whether this converter consumes an input resource.</summary>
        public readonly bool HasInput;

        /// <summary>Resource produced each cycle / on tap.</summary>
        public readonly ResourceId OutputResource;

        /// <summary>Output amount produced each cycle.</summary>
        public readonly int OutputAmount;

        /// <summary>Bonus output granted by a manual tap.</summary>
        public readonly int TapBonusAmount;

        /// <summary>Cooldown (seconds) after a tap before the next tap is allowed.</summary>
        public readonly float TapCooldownSeconds;

        public ConverterStats(
            float cycleSeconds,
            ResourceId outputResource,
            int outputAmount,
            int tapBonusAmount,
            float tapCooldownSeconds,
            ResourceId inputResource = default,
            int inputAmount = 0,
            bool hasInput = false)
        {
            CycleSeconds = cycleSeconds;
            OutputResource = outputResource;
            OutputAmount = outputAmount;
            TapBonusAmount = tapBonusAmount;
            TapCooldownSeconds = tapCooldownSeconds;
            InputResource = inputResource;
            InputAmount = inputAmount;
            HasInput = hasInput;
        }
    }
}
