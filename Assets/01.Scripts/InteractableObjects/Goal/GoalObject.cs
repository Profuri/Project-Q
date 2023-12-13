using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    private bool _isToggle;

    private void OnEnable()
    {
        _isToggle = false;
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        if(!_isToggle)
        {
            StageManager.Instance.StageClear();  
            _isToggle = true;
        }
    }
}
