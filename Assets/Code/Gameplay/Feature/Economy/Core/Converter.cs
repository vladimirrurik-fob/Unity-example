using System;

namespace Otus.Converters.Core
{
    /// <summary>
    /// Mutable runtime state for one converter: its current level and the timers that
    /// drive the automatic production cycle and the tap cooldown. Information Expert for
    /// its own timers; it does not own the resources it moves (that is
    /// <see cref="IResourceStorage"/>) nor its curves (that is
    /// <see cref="ConverterConfig"/>) — low coupling by design.
    /// </summary>
    public sealed class Converter
    {
        public ConverterId Id { get; }
        public int Level { get; private set; }

        private float _cycleTimer;
        private float _tapCooldown;

        public Converter(ConverterId id, int level = 1)
        {
            Id = id;
            Level = Math.Max(1, level);
        }

        public bool CanTap => _tapCooldown <= 0f;

        /// <summary>
        /// Advances the automatic cycle by <paramref name="dt"/> seconds. Returns true
        /// (and performs the conversion) when a cycle elapses and inputs are available.
        /// If inputs are missing, the timer holds at the threshold and retries next tick.
        /// </summary>
        public bool Tick(float dt, ConverterConfig config, IResourceStorage storage)
        {
            var stats = config.GetStats(Level);

            _cycleTimer += dt;
            if (_tapCooldown > 0f)
            {
                _tapCooldown = Math.Max(0f, _tapCooldown - dt);
            }

            if (_cycleTimer < stats.CycleSeconds)
            {
                return false;
            }

            if (Produce(stats, storage))
            {
                _cycleTimer = 0f;
                return true;
            }

            // Hold at threshold: retry as soon as inputs become available.
            _cycleTimer = stats.CycleSeconds;
            return false;
        }

        /// <summary>
        /// Manual tap: grants the tap-bonus output instantly, then starts the cooldown.
        /// Taps never consume input (the bonus is a pure reward), keeping the loop simple.
        /// </summary>
        public bool TryTap(ConverterConfig config, IResourceStorage storage)
        {
            if (!CanTap)
            {
                return false;
            }

            var stats = config.GetStats(Level);
            _tapCooldown = stats.TapCooldownSeconds;
            storage.Add(stats.OutputResource, stats.TapBonusAmount);
            return true;
        }

        public void SetLevel(int level) => Level = Math.Max(1, level);

        private bool Produce(ConverterStats stats, IResourceStorage storage)
        {
            if (stats.HasInput && !storage.TrySpend(stats.InputResource, stats.InputAmount))
            {
                return false;
            }

            storage.Add(stats.OutputResource, stats.OutputAmount);
            return true;
        }
    }
}
