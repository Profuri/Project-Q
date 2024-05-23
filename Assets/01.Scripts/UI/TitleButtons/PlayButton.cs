using AxisConvertSystem;
using InteractableSystem;

public class PlayButton : InteractableObject 
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        PlayButtonCall();
    }
    
    public void PlayButtonCall()
    {
        SceneControlManager.Instance.LoadScene(SceneType.Chapter);
    }
}