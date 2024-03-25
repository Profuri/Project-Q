using AxisConvertSystem;
using InteractableSystem;

public class PlayButton : InteractableObject 
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        // 나중에 연출 넣기
        SceneControlManager.Instance.LoadScene(SceneType.Chapter);  
    }
}