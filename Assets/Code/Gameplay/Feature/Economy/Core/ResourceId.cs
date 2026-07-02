namespace Otus.Converters.Core
{
    /// <summary>
    /// Identifies a resource in the economy. Order is deliberate: Ore (seed) is
    /// produced by the Mine, refined into Ingot by the Forge, and minted into Gold
    /// (the upgrade currency) by the Mint.
    /// </summary>
    public enum ResourceId
    {
        Ore = 0,
        Ingot = 1,
        Gold = 2,
    }
}
