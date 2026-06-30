namespace Code.Infrastructure.Services.Curtain
{
   /// <summary>
   /// Loading curtain abstraction shown while the bootstrap warms up and a scene
   /// is being loaded. Minimal console implementation here (a full UI prefab can
   /// drop in without touching the states).
   /// </summary>
   public interface ILoadingCurtain
   {
      void Show();

      void Hide();
   }
}
