using Singleton;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public bool OnTutorial { get; private set; }
    
    private TutorialWindow _tutorialWindow;

    public void SetUpTutorial(TutorialInfo info)
    {
        if (OnTutorial)
        {
            return;
        }
        
        _tutorialWindow = UIManager.Instance.GenerateUI("TutorialWindow") as TutorialWindow;
        // _tutorialWindow.SettingTutorial(info);
        // _tutorialWindow.PlayTutorial();
        
        InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, false);
        OnTutorial = true;
    }

    public void StopTutorial()
    {
        if (!OnTutorial)
        {
            return;
        }
        
        _tutorialWindow.Disappear();
        InputManager.Instance.SetEnableInputAll(true);
        OnTutorial = false;
    }

}
