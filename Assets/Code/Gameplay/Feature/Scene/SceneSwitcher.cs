using System;
using Code.Infrastructure.Services.SceneLoad;
using UnityEngine;
using VContainer;

namespace Code.Gameplay.Feature.Scene
{
  public class SceneSwitcher : MonoBehaviour
  {
    [SerializeField] private string _sceneName;
    private ISceneLoader _sceneLoader;

    [Inject]
    public void Construct(ISceneLoader sceneLoader)
    {
      _sceneLoader = sceneLoader;
    }
    
    private void OnTriggerEnter(Collider other)
    {
      if(other.CompareTag("Player"))
        _sceneLoader.LoadScene(_sceneName);
    }
  }
}