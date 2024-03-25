using UnityEngine;

public class PauseWindow : UIComponent
{
     private SettingWindow _settingWindow;
     
     public void Resume()
     {
          if (_settingWindow is not null && _settingWindow.poolOut)
          {
               _settingWindow.Disappear();
          }
          GameManager.Instance.Resume();
     }

     public void GenerateSettingPopup()
     {
          if (_settingWindow is null || !_settingWindow.poolOut)
          {
               _settingWindow = UIManager.Instance.GenerateUI("PauseSettingWindow") as SettingWindow;
               _settingWindow.transform.localPosition = new Vector3(5.05f, 1.2f, 7.48f);
               _settingWindow.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
          }
     }

     public void GoChapterSelectScene()
     {
          Resume();
          SceneControlManager.Instance.LoadScene(SceneType.Chapter);
     }

     public void GoTitleScene()
     {
          Resume();
          SceneControlManager.Instance.LoadScene(SceneType.Title);
     }
}