using System;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator _animator;

    private readonly int _activeHash = Animator.StringToHash("active");
    private readonly int _movementHash = Animator.StringToHash("movement");
    private readonly int _isGroundHash = Animator.StringToHash("isGround");
    private readonly int _isHoldHash = Animator.StringToHash("isHold");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void UnActive()
    {
        _animator.SetBool(_activeHash, false);
    }
    
    private void Active()
    {
        _animator.SetBool(_activeHash, true);
    }

    public void Movement(bool value)
    {
        _animator.SetBool(_movementHash, value);
    }

    public void IsGround(bool value)
    {
        _animator.SetBool(_isGroundHash, value);
    }

    public void IsHold(bool value)
    {
        _animator.SetBool(_isHoldHash, value);
    }
    
    // Animation event
    public void AutoActiveCall()
    {
        Active();
    }
}