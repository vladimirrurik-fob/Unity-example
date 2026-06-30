using System;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Feature.Player
{
  //  ПЛОХОЙ ПРИМЕР (GOD OBJECT):
  // Делает всё: движение, анимация, поворот к мыши, здоровье, сейв, стрельба.
  public class BadPlayerController : MonoBehaviour
  {
    [Header("Movement")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _speed = 5;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Look To Mouse")]
    [SerializeField] private Transform _rotateTransform;

    [Header("Health")]
    [SerializeField] private int _maxHealth = 100;
    private int _health = 5;

    [Header("Shooting")]
    [SerializeField] private Transform _firePoint;
    // [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _fireRate = 0.3f;

    [SerializeField] private TextMeshProUGUI _healthText;

    public event Action<int> OnHealthChanged;

    private bool _isMove;
    private float _nextShotTime;

    private void Start()
    {
      // сразу и инициализация здоровья
      OnHealthChanged?.Invoke(_health);
    }

    private void Update()
    {
      // HandleMovement();     // движение + флаг IsMove
      HandleAnimation();    // анимации
      // HandleLookToMouse();  // поворот к мыши
      HandleShooting();     // стрельба
      //Health
      //Обработка UI
    }

    // // ------------------------
    // // ДВИЖЕНИЕ
    // // ------------------------
    // private void HandleMovement()
    // {
    //   float x = Input.GetAxisRaw("Horizontal");
    //   float z = Input.GetAxisRaw("Vertical");
    //
    //   Vector3 inputMove = _playerTransform.right * x + _playerTransform.forward * z;
    //
    //   if (inputMove.x != 0 || inputMove.z != 0)
    //     _isMove = true;
    //   else
    //     _isMove = false;
    //
    //   _characterController.Move(inputMove * _speed * Time.deltaTime);
    // }

    // ------------------------
    // АНИМАЦИЯ
    // ------------------------
    private void HandleAnimation()
    {
      if (_animator != null)
        _animator.SetBool("IsMove", _isMove);
    }

    // ------------------------
    // ПОВОРОТ К МЫШИ
    // ------------------------
    private void HandleLookToMouse()
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit, 200))
      {
        Vector3 targetPosition = new Vector3(
          hit.point.x,
          _rotateTransform.position.y,
          hit.point.z);

        Quaternion rotation = Quaternion.LookRotation(targetPosition - _rotateTransform.position);
        _rotateTransform.rotation = Quaternion.Lerp(
          _rotateTransform.rotation,
          rotation,
          Time.deltaTime * 10f);
      }
    }

    // ------------------------
    // СТРЕЛЬБА
    // ------------------------
    private void HandleShooting()
    {
      if (Input.GetMouseButton(0))
      {
        if (Time.time > _nextShotTime)
        {
          Shoot();
          _nextShotTime = Time.time + _fireRate;
        }
      }
    }

    private void Shoot()
    {
      // Bullet bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
      // Vector3 direction = _firePoint.forward;
      // bullet.SetDirection(direction);
    }

    // ------------------------
    // ЗДОРОВЬЕ
    // ------------------------
    public void TakeDamage(int damage)
    {
      _health -= damage;
      _health = Mathf.Clamp(_health, 0, _maxHealth);
      OnHealthChanged?.Invoke(_health);
    }
  }
}
