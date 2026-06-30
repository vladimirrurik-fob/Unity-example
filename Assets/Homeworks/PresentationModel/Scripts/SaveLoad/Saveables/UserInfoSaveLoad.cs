using Homework.SaveLoad.Data;
using Lessons.Architecture.PM;

namespace Homework.SaveLoad
{
    // Persists the player's name + description (the Sprite icon is an asset
    // reference and is intentionally not serialized to a flat file).
    public sealed class UserInfoSaveLoad : ISaveLoad
    {
        private readonly UserInfo _userInfo;

        public UserInfoSaveLoad(UserInfo userInfo)
        {
            this._userInfo = userInfo;
        }

        public void Save(PlayerProgress progress)
        {
            progress.Player.Name = this._userInfo.Name ?? string.Empty;
            progress.Player.Description = this._userInfo.Description ?? string.Empty;
        }

        public void Load(PlayerProgress progress)
        {
            if (!string.IsNullOrEmpty(progress.Player.Name))
            {
                this._userInfo.ChangeName(progress.Player.Name);
            }

            if (!string.IsNullOrEmpty(progress.Player.Description))
            {
                this._userInfo.ChangeDescription(progress.Player.Description);
            }
        }
    }
}
