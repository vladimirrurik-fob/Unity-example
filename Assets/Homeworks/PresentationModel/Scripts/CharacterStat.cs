using System;

namespace Lessons.Architecture.PM
{
    public sealed class CharacterStat
    {
        public event Action<int> OnValueChanged;

        public string Name { get; private set; }

        public int Value { get; private set; }

        public CharacterStat(string name, int value = 0)
        {
            this.Name = name;
            this.Value = value;
        }

        public void ChangeValue(int value)
        {
            this.Value = value;
            this.OnValueChanged?.Invoke(value);
        }
    }
}
