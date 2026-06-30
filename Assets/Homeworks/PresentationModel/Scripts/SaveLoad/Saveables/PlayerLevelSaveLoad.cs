using Homework.SaveLoad.Data;
using Lessons.Architecture.PM;

namespace Homework.SaveLoad
{
    // Persists the player's level + experience.
    public sealed class PlayerLevelSaveLoad : ISaveLoad
    {
        private readonly PlayerLevel _playerLevel;

        public PlayerLevelSaveLoad(PlayerLevel playerLevel)
        {
            this._playerLevel = playerLevel;
        }

        public void Save(PlayerProgress progress)
        {
            progress.Player.Level = this._playerLevel.CurrentLevel;
            progress.Player.Experience = this._playerLevel.CurrentExperience;
        }

        public void Load(PlayerProgress progress)
        {
            // Restore silently (no OnLevelUp) so we don't re-trigger stat
            // progression side-effects — stat values are restored separately.
            this._playerLevel.Restore(progress.Player.Level, progress.Player.Experience);
        }
    }
}
