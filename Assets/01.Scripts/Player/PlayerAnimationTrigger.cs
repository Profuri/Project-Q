using System;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private PlayerUnit _playerUnit;

    private void Awake()
    {
        _playerUnit = transform.parent.GetComponent<PlayerUnit>();
    }

    public void AnimationTrigger(string key)
    {
        _playerUnit.AnimationTrigger(key);
    }
}