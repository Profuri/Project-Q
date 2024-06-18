using Singleton;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public bool OnTutorial { get; private set; }
    
    private TutorialWindow _tutorialWindow;

    public void SetUpTutorial(TutorialInfo info)
    {
        if (OnTutorial || (_tutorialWindow != null && _tutorialWindow.IsTweening))
        {
            return;
        }
        
        _tutorialWindow = UIManager.Instance.GenerateUI("TutorialWindow") as TutorialWindow;
        _tutorialWindow.SettingTutorial(info);
        _tutorialWindow.PlayTutorial();
        
        InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, true);
        OnTutorial = true;
    }

    public void StopTutorial()
    {
        if (!OnTutorial ||( _tutorialWindow != null && _tutorialWindow.IsTweening))
        {
            return;
        }
        
        _tutorialWindow.Disappear();
        InputManager.Instance.SetEnableInputAll(true);
        OnTutorial = false;

    }

}
