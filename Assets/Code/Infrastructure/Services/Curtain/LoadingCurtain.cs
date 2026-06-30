using UnityEngine;

namespace Code.Infrastructure.Services.Curtain
{
   public sealed class LoadingCurtain : ILoadingCurtain
   {
      public void Show()
      {
         Debug.Log("[Curtain] Show");
      }

      public void Hide()
      {
         Debug.Log("[Curtain] Hide");
      }
   }
}
