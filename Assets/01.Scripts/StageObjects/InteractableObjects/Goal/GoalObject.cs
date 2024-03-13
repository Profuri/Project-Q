using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        communicator.Converter.ConvertDimension(AxisType.None);
        StageManager.Instance.StageClear(communicator as PlayerUnit);
        Activate(false);
    }
}
