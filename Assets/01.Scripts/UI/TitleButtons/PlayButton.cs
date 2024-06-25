using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class PlayButton : InteractableObject
{
    [SerializeField] private ChapterData _tutorialChapterData;
    [SerializeField] private UIButton3D _button3D;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        PlayButtonCall();
    }

    public override void OnDetectedEnter(ObjectUnit communicator = null)
    {
        base.OnDetectedEnter(communicator);
        _button3D.OnHoverHandle();
    }

    public override void OnDetectedLeave(ObjectUnit communicator = null)
    {
        base.OnDetectedLeave(communicator);
        _button3D.OnHoverCancelHandle();
    }
    
    public void PlayButtonCall()
    {
        if (DataManager.sSaveData.ChapterClearDictionary[ChapterType.Tutorial])
        {
            SceneControlManager.Instance.LoadScene(SceneType.Chapter);
        }
        else
        {
            SceneControlManager.Instance.LoadScene(SceneType.Stage, 
                () =>
                { 
                    StageManager.Instance.StartNewChapter(_tutorialChapterData);
                    SceneControlManager.Instance.CurrentScene.initSection = StageManager.Instance.CurrentStage;
                },
                () =>
                {
                    var chapterInfoPanel = UIManager.Instance.GenerateUI("ChapterInfoPanel") as ChapterInfoPanel;
                    SoundManager.Instance.PlayCorrectBGM(SceneType.Stage, _tutorialChapterData.chapter == ChapterType.Cpu);
                    chapterInfoPanel.SetPosition(new Vector3(0, 0));
                    chapterInfoPanel.SetUp(_tutorialChapterData.chapter);
                }
            );
        }
    }
}