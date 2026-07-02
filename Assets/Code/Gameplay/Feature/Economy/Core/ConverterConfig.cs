using System;

namespace Otus.Converters.Core
{
    /// <summary>
    /// Defines a converter's behaviour as a pure function of its level. This is the
    /// Information Expert for the converter's stat and cost curves, and the polymorphic
    /// extension point of the system (Open/Closed): adding a new converter means adding
    /// a new subclass — no existing system code is touched, unlike the lecture's
    /// <c>if/switch</c> chain.
    /// </summary>
    public abstract class ConverterConfig
    {
        public abstract ConverterId Id { get; }

        /// <summary>Highest reachable level (levels are 1-based).</summary>
        public abstract int MaxLevel { get; }

        /// <summary>Runtime parameters at <paramref name="level"/> (1..MaxLevel).</summary>
        public abstract ConverterStats GetStats(int level);

        /// <summary>Gold cost to advance from <paramref name="level"/> to level+1.</summary>
        public abstract int GetUpgradeCost(int level);
    }

    /// <summary>
    /// Base implementation that derives all level-dependent stats from a small set of
    /// curve parameters (template-method style). Concrete converters override only the
    /// parameters — high cohesion, low duplication.
    /// </summary>
    public abstract class LeveledConverterConfig : ConverterConfig
    {
        // --- Identity ---
        protected abstract ResourceId Output { get; }
        protected virtual ResourceId Input => default;
        protected virtual int InputAmount => 0;
        protected virtual bool HasInput => false;

        // --- Bounds ---
        protected abstract int MaxLevelValue { get; }
        public override int MaxLevel => MaxLevelValue;

        // --- Base (level 1) values ---
        protected abstract float BaseCycle { get; }
        protected abstract int BaseOutput { get; }
        protected virtual int BaseTapBonus => 1;
        protected virtual float TapCooldown => 10f;

        // --- Per-level deltas ---
        protected virtual float CycleReductionPerLevel => 0.25f;
        protected virtual float MinCycle => 1f;
        protected virtual int OutputPerLevel => 1;
        protected virtual int TapBonusPerLevel => 1;

        // --- Cost curve (Gold) ---
        protected abstract int BaseCost { get; }
        protected virtual float CostMultiplier => 1.5f;

        public override ConverterStats GetStats(int level)
        {
            int step = level - 1;
            float cycle = Math.Max(MinCycle, BaseCycle - CycleReductionPerLevel * step);
            int output = BaseOutput + OutputPerLevel * step;
            int tap = BaseTapBonus + TapBonusPerLevel * step;
            return new ConverterStats(
                cycleSeconds: cycle,
                outputResource: Output,
                outputAmount: output,
                tapBonusAmount: tap,
                tapCooldownSeconds: TapCooldown,
                inputResource: Input,
                inputAmount: InputAmount,
                hasInput: HasInput);
        }

        public override int GetUpgradeCost(int level)
        {
            double cost = BaseCost * Math.Pow(CostMultiplier, level - 1);
            return Math.Max(1, (int)Math.Round(cost));
        }
    }

    public sealed class MineConfig : LeveledConverterConfig
    {
        public override ConverterId Id => ConverterId.Mine;
        protected override int MaxLevelValue => 5;
        protected override ResourceId Output => ResourceId.Ore;
        protected override float BaseCycle => 3f;
        protected override int BaseOutput => 1;
        protected override int BaseCost => 2;
    }

    public sealed class ForgeConfig : LeveledConverterConfig
    {
        public override ConverterId Id => ConverterId.Forge;
        protected override int MaxLevelValue => 5;
        protected override ResourceId Output => ResourceId.Ingot;
        protected override ResourceId Input => ResourceId.Ore;
        protected override int InputAmount => 2;
        protected override bool HasInput => true;
        protected override float BaseCycle => 5f;
        protected override int BaseOutput => 1;
        protected override int BaseCost => 3;
    }

    public sealed class MintConfig : LeveledConverterConfig
    {
        public override ConverterId Id => ConverterId.Mint;
        protected override int MaxLevelValue => 5;
        protected override ResourceId Output => ResourceId.Gold;
        protected override ResourceId Input => ResourceId.Ingot;
        protected override int InputAmount => 3;
        protected override bool HasInput => true;
        protected override float BaseCycle => 8f;
        protected override int BaseOutput => 1;
        protected override int BaseCost => 5;
    }
}
