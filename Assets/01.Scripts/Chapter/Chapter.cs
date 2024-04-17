using InteractableSystem;
using UnityEngine.Events;
using AxisConvertSystem;
using UnityEngine;

public class Chapter : InteractableObject
{
    [field:SerializeField] public ChapterData Data {get; private set; }
    
    [SerializeField] private float _symbolRotateSpeed;
    [SerializeField] private bool _canInteract = true;

    public UnityEvent onPopEvent = null;

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
        if (Data.stageCnt <= 0 || !_canInteract)
        {
            return;
        }

        SceneControlManager.Instance.LoadScene(SceneType.Stage, 
            () =>
            { 
                StageManager.Instance.StartNewChapter(Data);
                SceneControlManager.Instance.CurrentScene.initSection = StageManager.Instance.CurrentStage;
            },
            () =>
            {
                var chapterInfoPanel = UIManager.Instance.GenerateUI("ChapterInfoPanel") as ChapterInfoPanel;
                chapterInfoPanel.SetPosition(new Vector3(0, 0));
                chapterInfoPanel.SetUp(Data.chapter);
            }
        );
    }

    public virtual void ShowingSequence(ChapterType chapterType, SaveData saveData)
    {
        gameObject.SetActive(true);
    }

    public void ChangeInteract(bool canInteract)
    {
        _canInteract = canInteract;
    }

    public override void OnPop()
    {
        base.OnPop();
        onPopEvent?.Invoke();
    }
}