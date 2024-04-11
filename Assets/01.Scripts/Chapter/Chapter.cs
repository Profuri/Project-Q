using InteractableSystem;
using UnityEngine.Events;
using AxisConvertSystem;
using UnityEngine;
using DG.Tweening;

public class Chapter : InteractableObject
{
    [field:SerializeField] public ChapterData Data {get; private set; }
    [SerializeField] private float _symbolRotateSpeed;

    [SerializeField] private bool _canInteract = true;

    public UnityEvent OnShowSequence;

    protected static float s_sequenceTime = 5f;

    private Transform _symbolTrm;

    private ParticleSystem _dustParticle;

    public override void Awake()
    {
        base.Awake();
        _symbolTrm = transform.Find("Symbol");
        _dustParticle = transform.Find("ChapterDustParticle")?.GetComponent<ParticleSystem>();
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

    public void PlayDustParticle()
    {
        _dustParticle.Play();
    }

    public void StopDustParticle()
    {
        _dustParticle.Stop();
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

    public void ChangeInteract(bool canInteract)
    {
        _canInteract = canInteract;
    }
}