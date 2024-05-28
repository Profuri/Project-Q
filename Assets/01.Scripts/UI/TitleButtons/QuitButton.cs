using AxisConvertSystem;
using InteractableSystem;

public class QuitButton : InteractableObject
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        QuitButtonCall();
    }

    public void QuitButtonCall()
    {
        GameManager.Instance.QuitGame();
    }
}