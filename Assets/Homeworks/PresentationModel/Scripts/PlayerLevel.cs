using System;

namespace Lessons.Architecture.PM
{
    public sealed class PlayerLevel
    {
        public event Action OnLevelUp;
        public event Action<int> OnExperienceChanged;

        public int CurrentLevel { get; private set; } = 1;

        public int CurrentExperience { get; private set; }

        public int RequiredExperience => 100 * (this.CurrentLevel + 1);

        public void AddExperience(int range)
        {
            var xp = Math.Min(this.CurrentExperience + range, this.RequiredExperience);
            this.CurrentExperience = xp;
            this.OnExperienceChanged?.Invoke(xp);
        }

        public void LevelUp()
        {
            if (this.CanLevelUp())
            {
                this.CurrentExperience = 0;
                this.CurrentLevel++;
                this.OnLevelUp?.Invoke();
            }
        }

        public bool CanLevelUp()
        {
            return this.CurrentExperience == this.RequiredExperience;
        }

        // Restores saved level + experience. Deliberately does NOT raise OnLevelUp
        // (that would re-apply level-up side-effects like stat progression); it only
        // notifies that experience changed so bound views refresh.
        public void Restore(int level, int experience)
        {
            this.CurrentLevel = level;
            this.CurrentExperience = experience;
            this.OnExperienceChanged?.Invoke(experience);
        }
    }
}
