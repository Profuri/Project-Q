using InteractableSystem;
using AxisConvertSystem;

public class GoalObject : InteractableObject
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        StageManager.Instance.StageClear(communicator as PlayerUnit);
        Activate(false);
    }
}
