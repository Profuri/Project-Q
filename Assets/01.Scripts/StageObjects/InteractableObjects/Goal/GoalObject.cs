using InteractableSystem;
using AxisConvertSystem;

public class GoalObject : InteractableObject
{
    private SoundEffectPlayer _soundEffectPlayer;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _soundEffectPlayer = new SoundEffectPlayer(this);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SoundManager.Instance.PlaySFX("Goal",false,_soundEffectPlayer);
        StageManager.Instance.StageClear(communicator as PlayerUnit);
        Activate(false);
    }
}
