using Code.Infrastructure.Services._Input;
using Code.Infrastructure.Services.Audio;
using UnityEngine;
using VContainer;

namespace Code.Gameplay.Feature.Player.Behaviours
{
  public class PlayerMovement : MonoBehaviour
  {
    [SerializeField] private IInputService _input;
    [SerializeField] private AudioService _audioService;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _speed = 5;
    private bool _isMove;
    private float _verticalVelocity;
    private IPlayerService _playerService;

    [Inject]
    public void Construct(IInputService input)
    {
      _input = input;
      // _playerService = playerService;
    }

    private void Update()
    {
      if (_input == null) return;
      
      float x = _input.GetHorizontalInput();
      float z = _input.GetVerticalInput();

      Vector3 inputMove = _playerTransform.right * x + _playerTransform.forward * z;

      if (inputMove.x != 0 || inputMove.z != 0)
        _isMove = true;
      else
        _isMove = false;
      
      bool isGrounded = _characterController.isGrounded;

      if (isGrounded && _verticalVelocity < 0) 
        _verticalVelocity = -2;
      
      _verticalVelocity += -9.81f * Time.deltaTime;
      
      inputMove.y = _verticalVelocity;

      // if (_isMove)
      //   _audioService.PlaySound(AudioId.Step);
      _characterController.Move(inputMove * _speed * Time.deltaTime);
    }
    
    public bool IsMove() 
      => _isMove;
  }
}