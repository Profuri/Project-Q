using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public bool IsTutorialViewing { get; set; } = false;
    private TutorialCanvas _currentCanvas;

    public void StartTutorial(TutorialSO tutorialSO)
    {
        _currentCanvas = SceneControlManager.Instance.AddObject("TutorialCanvas") as TutorialCanvas;
        _currentCanvas.ShowTutorial(tutorialSO);
    }

    public void StopTutorial()
    {
        SceneControlManager.Instance.DeleteObject(_currentCanvas);
    }

}
