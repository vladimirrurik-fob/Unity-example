using System.Collections.Generic;
using UnityEngine;

namespace Homework.PresentationModel
{
    public interface IPopupManager
    {
        void Register(string key, GameObject popup);

        void Unregister(string key);

        void Show(string key);

        void Hide(string key);

        void HideAll();
    }

    public sealed class PopupManager : IPopupManager
    {
        private readonly Dictionary<string, GameObject> _popups = new();

        public void Register(string key, GameObject popup)
        {
            this._popups[key] = popup;
            if (popup != null)
            {
                popup.SetActive(false);
            }
        }

        public void Unregister(string key)
        {
            this._popups.Remove(key);
        }

        public void Show(string key)
        {
            if (this._popups.TryGetValue(key, out var popup) && popup != null)
            {
                popup.SetActive(true);
            }
        }

        public void Hide(string key)
        {
            if (this._popups.TryGetValue(key, out var popup) && popup != null)
            {
                popup.SetActive(false);
            }
        }

        public void HideAll()
        {
            foreach (var popup in this._popups.Values)
            {
                if (popup != null)
                {
                    popup.SetActive(false);
                }
            }
        }
    }
}
