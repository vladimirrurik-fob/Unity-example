using System;

namespace Code.Infrastructure.Services._Input
{
  public interface IInputService
  {
    event Action OnSaveBtnClick;
    event Action OnLoadBtnClick;
    float GetHorizontalInput();
    float GetVerticalInput();
    void SaveBtnClick();
    void LoadBtnClick();
  }
}
