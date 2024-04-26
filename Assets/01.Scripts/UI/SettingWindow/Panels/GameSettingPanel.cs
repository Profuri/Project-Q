public class GameSettingPanel : WindowPanel
{
    public void ResetGameData()
    {
        DataManager.Instance.ResetData();
        SceneControlManager.Instance.LoadScene(SceneType.Title);
    }
}