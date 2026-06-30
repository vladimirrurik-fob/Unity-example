using UnityEngine;
using VContainer;

namespace Homework.SaveLoad
{
    // Humble adapter: the only MonoBehaviour in the save/load subsystem. It does
    // nothing but forward Unity lifecycle callbacks to the plain-C# save service.
    // Add it to any root GameObject in the scene; VContainer injects the service.
    public sealed class AppQuitSaver : MonoBehaviour
    {
        private ISaveLoadService _saveLoadService;

        [Inject]
        public void Construct(ISaveLoadService saveLoadService)
        {
            this._saveLoadService = saveLoadService;
        }

        private void OnApplicationQuit()
        {
            this._saveLoadService?.Save();
        }

        // Covers editor Stop and mobile backgrounding.
        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                this._saveLoadService?.Save();
            }
        }
    }
}
