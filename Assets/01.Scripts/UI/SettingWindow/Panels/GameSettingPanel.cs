public class GameSettingPanel : WindowPanel
{
    public void ResetGameData()
    {
        if (GameManager.Instance.InPause)
        {
            GameManager.Instance.Resume(false);
        }
        
        SceneControlManager.Instance.LoadScene(SceneType.Title, DataManager.Instance.ResetData);
    }
}