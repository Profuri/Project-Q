using InteractableSystem;
using UnityEngine.Events;
using AxisConvertSystem;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Chapter : InteractableObject
{
    [field:SerializeField] public ChapterData Data {get; private set; }
    [SerializeField] private float _symbolRotateSpeed;

    public UnityEvent OnShowSequence;

    private Transform[] _moveTransforms;

    protected static float s_sequenceTime = 5f;

    private Transform _symbolTrm;

    public override void Awake()
    {
        base.Awake();
        _symbolTrm = transform.Find("Symbol");

        _moveTransforms = new Transform[transform.childCount];
        _moveTransforms = GetComponentsInChildren<Transform>();
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        if (_symbolTrm is not null)
        {
            _symbolTrm.Rotate(0, _symbolRotateSpeed, 0, Space.World);
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SceneControlManager.Instance.LoadScene(SceneType.Stage, () =>
        {
            StageManager.Instance.StartNewChapter(Data);
        });
    }


    public virtual void ShowingSequence(ChapterType chapterType,SaveData saveData)
    {
        gameObject.SetActive(true);

        if (saveData.IsClearTutorial)
        {
            return;
        }

        if (chapterType == ChapterType.MAINBOARD)
        {
            Vector3 targetPos = transform.position;

            transform.position = targetPos - Vector3.up * 3f;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(targetPos,s_sequenceTime));

            OnShowSequence?.Invoke();
        }
    }
}