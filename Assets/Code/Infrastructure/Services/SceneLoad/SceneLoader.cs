using System;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.Services.SceneLoad
{
  public class SceneLoader : ISceneLoader
  {
    public void LoadScene(string sceneName)
    {
      SceneManager.LoadScene(sceneName);
    }
  }
}
