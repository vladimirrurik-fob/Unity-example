using Otus.Converters.Core;
using UnityEngine;

namespace Code.Gameplay.Feature.Economy
{
    /// <summary>
    /// Bridge from Unity's per-frame loop to the pure <see cref="IConverterService"/>.
    /// The domain logic has no Unity dependency, so this MonoBehaviour is the only place
    /// that feeds <c>Time.deltaTime</c> in — keeping the Core assembly engine-free and
    /// unit-testable.
    /// <para>
    /// Driven from <c>Update</c> (rather than VContainer's ITickable) so converter
    /// ticking does not depend on VContainer player-loop settings — deterministic and
    /// self-contained. Constructed by <see cref="EconomyBootstrap"/>.
    /// </para>
    /// </summary>
    public sealed class EconomyTickable : MonoBehaviour
    {
        private IConverterService _converters;

        public void Construct(IConverterService converters) => _converters = converters;

        private void Update()
        {
            if (_converters != null)
            {
                _converters.Tick(Time.deltaTime);
            }
        }
    }
}
