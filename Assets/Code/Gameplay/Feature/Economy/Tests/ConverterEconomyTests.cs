using System.Collections.Generic;
using NUnit.Framework;
using Otus.Converters.Core;

namespace Otus.Converters.Tests
{
    /// <summary>
    /// Pure-logic EditMode tests for the resource-converter economy. No Unity types —
    /// the whole domain is in Otus.Converters.Core and fully unit-testable.
    /// </summary>
    [TestFixture]
    public class ConverterEconomyTests
    {
        // ---------------------------------------------------------------------
        // ResourceStorage
        // ---------------------------------------------------------------------

        [Test]
        public void Storage_Add_AccumulatesAmount()
        {
            var storage = new ResourceStorage();

            storage.Add(ResourceId.Ore, 2);
            storage.Add(ResourceId.Ore, 3);

            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(5));
        }

        [Test]
        public void Storage_Add_RaisesChangedWithNewAmount()
        {
            var storage = new ResourceStorage();
            int fired = 0;
            ResourceId changed = (ResourceId)(-1);
            storage.Changed += (r, amount) => { fired++; changed = r; };

            storage.Add(ResourceId.Gold, 7);

            Assert.That(fired, Is.EqualTo(1));
            Assert.That(changed, Is.EqualTo(ResourceId.Gold));
            Assert.That(storage.Get(ResourceId.Gold), Is.EqualTo(7));
        }

        [Test]
        public void Storage_TrySpend_Insufficient_ReturnsFalseAndLeavesAmount()
        {
            var storage = new ResourceStorage();
            storage.Set(ResourceId.Ore, 1);

            bool spent = storage.TrySpend(ResourceId.Ore, 2);

            Assert.IsFalse(spent);
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(1));
        }

        [Test]
        public void Storage_TrySpend_Sufficient_Decrements()
        {
            var storage = new ResourceStorage();
            storage.Set(ResourceId.Ore, 5);

            bool spent = storage.TrySpend(ResourceId.Ore, 3);

            Assert.IsTrue(spent);
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(2));
        }

        // ---------------------------------------------------------------------
        // Converter configs (Information Expert for curves; polymorphism/OCP)
        // ---------------------------------------------------------------------

        [Test]
        public void MineConfig_Level1_HasBaseValuesAndNoInput()
        {
            var stats = new MineConfig().GetStats(1);

            Assert.That(stats.CycleSeconds, Is.EqualTo(3f));
            Assert.That(stats.OutputAmount, Is.EqualTo(1));
            Assert.That(stats.OutputResource, Is.EqualTo(ResourceId.Ore));
            Assert.IsFalse(stats.HasInput);
        }

        [Test]
        public void MineConfig_HigherLevel_ReducesCycleAndRaisesOutput()
        {
            var config = new MineConfig();
            var level1 = config.GetStats(1);
            var level3 = config.GetStats(3);

            Assert.That(level3.CycleSeconds, Is.LessThan(level1.CycleSeconds));
            Assert.That(level3.OutputAmount, Is.GreaterThan(level1.OutputAmount));
        }

        [Test]
        public void ForgeConfig_ConsumesOre_ProducesIngot()
        {
            var stats = new ForgeConfig().GetStats(1);

            Assert.IsTrue(stats.HasInput);
            Assert.That(stats.InputResource, Is.EqualTo(ResourceId.Ore));
            Assert.That(stats.InputAmount, Is.EqualTo(2));
            Assert.That(stats.OutputResource, Is.EqualTo(ResourceId.Ingot));
        }

        [Test]
        public void MintConfig_ProducesGold()
        {
            var stats = new MintConfig().GetStats(1);

            Assert.That(stats.OutputResource, Is.EqualTo(ResourceId.Gold));
            Assert.That(stats.InputResource, Is.EqualTo(ResourceId.Ingot));
        }

        [Test]
        public void Config_UpgradeCost_GrowsWithLevel()
        {
            var config = new ForgeConfig();

            Assert.That(config.GetUpgradeCost(2), Is.GreaterThan(config.GetUpgradeCost(1)));
            Assert.That(config.GetUpgradeCost(3), Is.GreaterThan(config.GetUpgradeCost(2)));
        }

        [Test]
        public void Config_MaxLevel_IsFive()
        {
            Assert.That(new MineConfig().MaxLevel, Is.EqualTo(5));
            Assert.That(new ForgeConfig().MaxLevel, Is.EqualTo(5));
            Assert.That(new MintConfig().MaxLevel, Is.EqualTo(5));
        }

        // ---------------------------------------------------------------------
        // Converter runtime (timers, input gating, tap cooldown)
        // ---------------------------------------------------------------------

        [Test]
        public void Converter_BelowCycle_DoesNotProduce()
        {
            var storage = new ResourceStorage();
            var converter = new Converter(ConverterId.Mine);

            bool produced = converter.Tick(2f, new MineConfig(), storage);

            Assert.IsFalse(produced);
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(0));
        }

        [Test]
        public void Converter_AtCycle_Produces()
        {
            var storage = new ResourceStorage();
            var converter = new Converter(ConverterId.Mine);

            bool produced = converter.Tick(3f, new MineConfig(), storage);

            Assert.IsTrue(produced);
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(1));
        }

        [Test]
        public void Converter_InsufficientInput_HoldsUntilAvailable()
        {
            var storage = new ResourceStorage();
            var converter = new Converter(ConverterId.Forge);
            var config = new ForgeConfig();

            // Cycle elapses but there is no Ore to spend.
            Assert.IsFalse(converter.Tick(5f, config, storage));
            Assert.That(storage.Get(ResourceId.Ingot), Is.EqualTo(0));

            // Provide input: the held cycle fires immediately on the next tick.
            storage.Add(ResourceId.Ore, 2);
            Assert.IsTrue(converter.Tick(0.01f, config, storage));
            Assert.That(storage.Get(ResourceId.Ingot), Is.EqualTo(1));
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(0));
        }

        [Test]
        public void Converter_Tap_GrantsBonusAndEnforcesCooldown()
        {
            var storage = new ResourceStorage();
            var converter = new Converter(ConverterId.Mine);
            var config = new MineConfig();

            Assert.IsTrue(converter.TryTap(config, storage));
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(1));
            Assert.IsFalse(converter.CanTap, "cooldown should block an immediate second tap");

            // Second tap is rejected.
            Assert.IsFalse(converter.TryTap(config, storage));

            // After the cooldown elapses (Tick also advances an auto cycle), tapping works again.
            converter.Tick(10f, config, storage);
            Assert.IsTrue(converter.CanTap);
            Assert.IsTrue(converter.TryTap(config, storage));
        }

        // ---------------------------------------------------------------------
        // ResourceConverterService
        // ---------------------------------------------------------------------

        [Test]
        public void Service_Tick_ProducesAndRaisesEvent()
        {
            var storage = new ResourceStorage();
            var service = new ResourceConverterService(storage, AllConfigs());

            int produced = 0;
            service.Produced += _ => produced++;

            service.Tick(3f); // Mine cycle elapses.

            Assert.That(produced, Is.EqualTo(1));
            Assert.That(storage.Get(ResourceId.Ore), Is.EqualTo(1));
        }

        [Test]
        public void Service_CommitLevelUp_RaisesLevelChangedAndCapsAtMax()
        {
            var service = new ResourceConverterService(new ResourceStorage(), AllConfigs());

            int changes = 0;
            service.LevelChanged += (id, level) => changes++;

            for (int i = 0; i < 10; i++)
            {
                service.CommitLevelUp(ConverterId.Mine);
            }

            Assert.That(changes, Is.EqualTo(4), "Mine starts at 1, max 5 -> 4 level-ups");
            Assert.That(service.GetLevel(ConverterId.Mine), Is.EqualTo(5));
            Assert.IsFalse(service.CanLevelUp(ConverterId.Mine));
        }

        [Test]
        public void Service_RestoreLevel_ClampsToValidRange()
        {
            var service = new ResourceConverterService(new ResourceStorage(), AllConfigs());

            service.RestoreLevel(ConverterId.Mint, 99);
            Assert.That(service.GetLevel(ConverterId.Mint), Is.EqualTo(5));

            service.RestoreLevel(ConverterId.Mint, 0);
            Assert.That(service.GetLevel(ConverterId.Mint), Is.EqualTo(1));
        }

        // ---------------------------------------------------------------------
        // UpgradeManager (Controller: Gold cost + level change)
        // ---------------------------------------------------------------------

        [Test]
        public void Upgrade_CannotAfford_ReturnsFalseAndNoChange()
        {
            var storage = new ResourceStorage(); // 0 Gold
            var service = new ResourceConverterService(storage, AllConfigs());
            var manager = new UpgradeManager(service, storage);

            Assert.IsFalse(manager.CanUpgrade(ConverterId.Mine));
            Assert.IsFalse(manager.TryUpgrade(ConverterId.Mine));
            Assert.That(service.GetLevel(ConverterId.Mine), Is.EqualTo(1));
        }

        [Test]
        public void Upgrade_WhenAffordable_SpendsGoldAndLevelsUp()
        {
            var storage = new ResourceStorage();
            storage.Set(ResourceId.Gold, 10);
            var service = new ResourceConverterService(storage, AllConfigs());
            var manager = new UpgradeManager(service, storage);

            int cost = manager.GetUpgradeCost(ConverterId.Mine); // 2 Gold
            bool upgraded = manager.TryUpgrade(ConverterId.Mine);

            Assert.IsTrue(upgraded);
            Assert.That(service.GetLevel(ConverterId.Mine), Is.EqualTo(2));
            Assert.That(storage.Get(ResourceId.Gold), Is.EqualTo(10 - cost));
        }

        [Test]
        public void Upgrade_RaisesUpgradedEventWithNewLevel()
        {
            var storage = new ResourceStorage();
            storage.Set(ResourceId.Gold, 10);
            var service = new ResourceConverterService(storage, AllConfigs());
            var manager = new UpgradeManager(service, storage);

            int newLevel = 0;
            ConverterId which = (ConverterId)(-1);
            manager.Upgraded += (id, level) => { which = id; newLevel = level; };

            manager.TryUpgrade(ConverterId.Mine);

            Assert.That(which, Is.EqualTo(ConverterId.Mine));
            Assert.That(newLevel, Is.EqualTo(2));
        }

        [Test]
        public void Upgrade_AtMaxLevel_ReturnsFalse()
        {
            var storage = new ResourceStorage();
            storage.Set(ResourceId.Gold, 1000);
            var service = new ResourceConverterService(storage, AllConfigs());
            var manager = new UpgradeManager(service, storage);

            while (service.CanLevelUp(ConverterId.Mine))
            {
                service.CommitLevelUp(ConverterId.Mine);
            }

            Assert.IsFalse(manager.CanUpgrade(ConverterId.Mine));
            Assert.IsFalse(manager.TryUpgrade(ConverterId.Mine));
        }

        // ---------------------------------------------------------------------

        private static IReadOnlyDictionary<ConverterId, ConverterConfig> AllConfigs() =>
            new Dictionary<ConverterId, ConverterConfig>
            {
                { ConverterId.Mine, new MineConfig() },
                { ConverterId.Forge, new ForgeConfig() },
                { ConverterId.Mint, new MintConfig() },
            };
    }
}
