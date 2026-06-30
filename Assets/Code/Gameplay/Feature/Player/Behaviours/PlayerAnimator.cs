using System;
using UnityEngine;

namespace Code.Gameplay.Feature.Player.Behaviours
{
  public class PlayerAnimator : MonoBehaviour
  {
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerMovement _playerMovement;

    private void Awake()
    {
      
    }

    private void Start()
    {
      
    }

    private void Update()
    {
      _animator.SetBool("IsMove", _playerMovement.IsMove());
    }
  }
}