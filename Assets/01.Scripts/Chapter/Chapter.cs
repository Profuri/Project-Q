using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Chapter : InteractableObject
{
    [field:SerializeField] public ChapterData Data {get; private set; }
    [SerializeField] private float _symbolRotateSpeed;
    [SerializeField] private List<Transform> _moveTransformList;
    [SerializeField] private Vector3 _targetPos = Vector3.zero;
    private static float s_sequenceTime = 5f;

    private Transform _symbolTrm;

    public override void Awake()
    {
        base.Awake();
        _symbolTrm = transform.Find("Symbol");

        if(_targetPos == Vector3.zero)
        {
            _targetPos = transform.position;
        }
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


    public void ShowingSequence(ChapterType chapterType)
    {
        Debug.Log($"ChapterType: {chapterType}");
        if (chapterType == ChapterType.MAINBOARD)
        {
            Sequence sequence = DOTween.Sequence();
            foreach (Transform trm in _moveTransformList)
            {
                trm.gameObject.SetActive(true);
                trm.position = _targetPos + Vector3.up * -5f;
                sequence.Join(trm.DOMove(_targetPos, s_sequenceTime));
            }
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        if(_targetPos == Vector3.zero)
        {
            _targetPos = transform.position;
        }
            Gizmos.DrawSphere(_targetPos, 1f);
    }
#endif
}