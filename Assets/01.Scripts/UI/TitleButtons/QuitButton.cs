using AxisConvertSystem;
using InteractableSystem;

public class QuitButton : InteractableObject
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        // 나중에 연출 추가하던지 하자
        GameManager.Instance.QuitGame();
    }
}