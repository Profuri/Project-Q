using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    [SerializeField] private bool _playerResetOnClear;
    private SoundEffectPlayer _soundEffectPlayer;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _soundEffectPlayer = new SoundEffectPlayer(this);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (_playerResetOnClear)
        {
            if (communicator is PlayerUnit player)
            {
                player.ReloadUnit();
            }
        }
        
        SoundManager.Instance.PlaySFX("Goal",false,_soundEffectPlayer);
        StageManager.Instance.StageClear(communicator as PlayerUnit);
        Activate(false);
    }
}
