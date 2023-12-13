using InteractableSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    private bool _isToggle;

    private void OnEnable()
    {
        _isToggle = false;
    }

    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        if(!_isToggle)
        {
            StageManager.Instance.StageClear();  
            _isToggle = true;
        }
    }
}
