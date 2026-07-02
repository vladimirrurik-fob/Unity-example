using System.Collections.Generic;
using Otus.Converters.Core;
using VContainer;

namespace Code.Gameplay.Feature.Economy
{
    /// <summary>
    /// Composition helper that registers the economy subsystem in a VContainer
    /// <see cref="IContainerBuilder"/> — mirrors the lecture's
    /// <c>UpgradeInstaller.Install(Container)</c> idiom. Kept as a Pure Fabrication so
    /// the gameplay composition root (<c>GameLifetimeScope</c>) stays readable and the
    /// economy can be added/removed in one line.
    /// </summary>
    public static class EconomyInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            // Information Expert for resource amounts.
            builder.Register<ResourceStorage>(Lifetime.Singleton).AsImplementedInterfaces();

            // The converter catalog (data) the services depend on. Default catalog is
            // code-defined; a ScriptableObject-authored catalog could be swapped in here
            // (SDD) without touching the services.
            builder.RegisterInstance<IReadOnlyDictionary<ConverterId, ConverterConfig>>(DefaultCatalog());

            // Pure-fabrication controller over the converters + the upgrade controller.
            builder.Register<ResourceConverterService>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<UpgradeManager>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Save/load adapter — collected by SaveLoadRegistry as ISaveLoad.
            builder.Register<EconomySaveLoad>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        /// <summary>The default converter set: Mine (Ore seed) -> Forge (Ingot) -> Mint (Gold).</summary>
        public static IReadOnlyDictionary<ConverterId, ConverterConfig> DefaultCatalog() =>
            new Dictionary<ConverterId, ConverterConfig>
            {
                { ConverterId.Mine, new MineConfig() },
                { ConverterId.Forge, new ForgeConfig() },
                { ConverterId.Mint, new MintConfig() },
            };
    }
}
