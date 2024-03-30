using AxisConvertSystem;
using InteractableSystem;

public class PlayButton : InteractableObject 
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SceneControlManager.Instance.LoadScene(SceneType.Chapter);  
    }
}