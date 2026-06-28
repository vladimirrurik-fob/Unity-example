using System;
using UniRx;
using Lessons.Architecture.PM;

namespace Homework.PresentationModel
{
    public sealed class CharacterStatPresentationModel : ICharacterStatPresentationModel, IDisposable
    {
        public IReadOnlyReactiveProperty<string> Name => this._name;

        public IReadOnlyReactiveProperty<string> Value => this._value;

        private readonly ReactiveProperty<string> _name = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _value = new ReactiveProperty<string>();

        private readonly CharacterStat _stat;

        public CharacterStatPresentationModel(CharacterStat stat)
        {
            this._stat = stat;
            this._stat.OnValueChanged += this.OnValueChanged;
            this.Refresh();
        }

        public void Dispose()
        {
            this._stat.OnValueChanged -= this.OnValueChanged;
        }

        private void OnValueChanged(int value) => this.Refresh();

        private void Refresh()
        {
            this._name.Value = this._stat.Name;
            this._value.Value = this._stat.Value.ToString();
        }
    }
}
