using InteractableSystem;
using UnityEngine.Events;
using AxisConvertSystem;
using UnityEngine;
using DG.Tweening;

public class Chapter : InteractableObject
{
    [field:SerializeField] public ChapterData Data {get; private set; }
    [SerializeField] private float _symbolRotateSpeed;

    public UnityEvent OnShowSequence;

    protected static float s_sequenceTime = 5f;

    private Transform _symbolTrm;

    public override void Awake()
    {
        base.Awake();
        _symbolTrm = transform.Find("Symbol");
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        if (_symbolTrm is not null)
        {
            _symbolTrm.eulerAngles += new Vector3(0, _symbolRotateSpeed * Time.deltaTime, 0);
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SceneControlManager.Instance.LoadScene(SceneType.Stage, 
            () =>
            { 
                StageManager.Instance.StartNewChapter(Data);
                SceneControlManager.Instance.Player.SetPosition(StageManager.Instance.CurrentStage.PlayerResetPoint);
                SceneControlManager.Instance.Player.Dissolve(0f, 0.5f);
            },
            () =>
            {
                var chapterInfoPanel = UIManager.Instance.GenerateUI("ChapterInfoPanel") as ChapterInfoPanel;
                chapterInfoPanel.SetPosition(new Vector3(0, 0));
                chapterInfoPanel.SetUp(Data.chapter);
            }
        );
    }


    public virtual void ShowingSequence(ChapterType chapterType,SaveData saveData)
    {
        gameObject.SetActive(true);

        if (saveData.IsShowSequence)
        {
            return;
        }

        if (chapterType == ChapterType.Tutorial)
        {
            Vector3 targetPos = transform.position;

            transform.position = targetPos - Vector3.up * 3f;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(targetPos,s_sequenceTime));
            sequence.AppendCallback(() =>
            {
                if (saveData.IsShowSequence == false)
                {
                    saveData.IsShowSequence = true;
                    DataManager.Instance.SaveData();
                }
            });

            OnShowSequence?.Invoke();
        }
    }
}