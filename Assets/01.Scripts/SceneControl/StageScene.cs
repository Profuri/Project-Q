public class StageScene : Scene
{
    public override void OnPush()
    {
        StageManager.Instance.ReleaseChapter();
        base.OnPush();
    }
}