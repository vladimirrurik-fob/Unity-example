using System;
using UniRx;

namespace Homework.PresentationModel
{
    public interface ICharacterStatPresentationModel : IDisposable
    {
        IReadOnlyReactiveProperty<string> Name { get; }

        IReadOnlyReactiveProperty<string> Value { get; }
    }
}
