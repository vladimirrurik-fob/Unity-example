using UnityEngine;

namespace Code.Infrastructure.Services.Audio
{
  public interface IAudioService
  {
    void PlaySound(AudioId audioId);
  }

  public class AudioService : MonoBehaviour, IAudioService
  {
    [SerializeField] private AudioSource _audioSource;
    
    public void PlaySound(AudioId audioId)
    {
      _audioSource.PlayOneShot(null, 1f);
    }
  }

  public enum AudioId
  {
    None = 0,
    Step = 1,
  }
}