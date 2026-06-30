using UnityEngine;

namespace Code.Gameplay.Feature.Player.Behaviours
{
  public class LookToMouse : MonoBehaviour
  {
    [SerializeField] private Transform _rotateTransform;

    private void Update()
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 200))
      {
        Vector3 targetPosition = new Vector3(hit.point.x, _rotateTransform.position.y, hit.point.z);
        Quaternion rotation = Quaternion.LookRotation(targetPosition - _rotateTransform.position);
        _rotateTransform.rotation = Quaternion.Lerp(_rotateTransform.rotation, rotation, Time.deltaTime * 10f);
      }
    }
  }
}