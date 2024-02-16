using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    private bool _isToggle;

    public override void Awake()
    {
        base.Awake();
        _isToggle = false;
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if(!_isToggle)
        {
            communicator.Converter.ConvertDimension(AxisType.None);
            StageManager.Instance.StageClear();  
            _isToggle = true;
        }
    }
}
