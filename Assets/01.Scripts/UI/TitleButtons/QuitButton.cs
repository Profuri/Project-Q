using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class QuitButton : InteractableObject
{
    [SerializeField] private UIButton3D _button3D;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        QuitButtonCall();
    }
    
    public override void OnDetectedEnter(ObjectUnit communicator = null)
    {
        base.OnDetectedEnter(communicator);
        if (_button3D != null)
        {
            _button3D.OnHoverHandle();
        }
    }

    public override void OnDetectedLeave(ObjectUnit communicator = null)
    {
        base.OnDetectedLeave(communicator);
        if (_button3D != null)
        {
            _button3D.OnHoverCancelHandle();
        }
    }

    public void QuitButtonCall()
    {
        GameManager.Instance.QuitGame();
    }
}