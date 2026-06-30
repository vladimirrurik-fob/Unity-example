using System;
using Code.Infrastructure.Services.Analytic;
using UnityEngine;

namespace Code.Infrastructure.Services._Input
{
  public class InputService : IInputService
  {
    public event Action OnSaveBtnClick;
    public event Action OnLoadBtnClick;

    public float GetHorizontalInput() =>
      Input.GetAxisRaw("Horizontal");

    public float GetVerticalInput() =>
      Input.GetAxisRaw("Vertical");

    public void SaveBtnClick()
    {
      OnSaveBtnClick?.Invoke();
    }

    public void LoadBtnClick()
    {
      OnLoadBtnClick?.Invoke();
    }
  }
}
