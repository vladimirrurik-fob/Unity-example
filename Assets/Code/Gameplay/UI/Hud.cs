using System;
using Code.Infrastructure.Services._Input;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class Hud : MonoBehaviour
{
   [SerializeField] private Button _saveButton;
   [SerializeField] private Button _loadButton;

   private IInputService _inputService;

   [Inject]
   public void Construct(IInputService inputService)
   {
      _inputService = inputService;

      if (_saveButton != null)
      {
         _saveButton.onClick.AddListener(ClickSave);
      }

      if (_loadButton != null)
      {
         _loadButton.onClick.AddListener(ClickLoad);
      }
   }

   private void ClickSave()
   {
      _inputService.SaveBtnClick();
   }

   private void ClickLoad()
   {
      _inputService.LoadBtnClick();
   }
}
