using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class PlayButton : InteractableObject
{
    [SerializeField] private ChapterData _tutorialChapterData;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        PlayButtonCall();
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