using System;
using System.Collections;
using UnityEngine;
using StageStructureConvertSystem;

[RequireComponent(typeof(StructureConverter), typeof(StageCollisionChecker))]
public class Stage : PoolableMono
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    public ChapterType Chapter => _chapter;
    [SerializeField] private int _stageOrder;
    public int stageOrder => _stageOrder;

    [Header("Stage Setting")]
    [SerializeField] private Vector3 _stageEnterPoint;
    public Vector3 StageEnterPoint => _stageEnterPoint;
    
    [SerializeField] private Vector3 _stageExitPoint;
    public Vector3 StageExitPoint => _stageExitPoint;

    [SerializeField] private Vector3 _playerResetPoint;
    public Vector3 PlayerResetPoint => _playerResetPoint;

    public Vector3 CenterPosition { get; private set; }
    public bool ActiveStage { get; private set; }

    public StructureConverter Converter { get; private set; }

    private void Awake()
    {
        Converter = GetComponent<StructureConverter>();
    }

    public void GenerateStage(Vector3 position)
    {
        transform.position = position - Vector3.up * 5;
        CenterPosition = position;
        StartCoroutine(StageMoveRoutine(CenterPosition));
    }

    public void DisappearStage()
    {
        StartCoroutine(StageMoveRoutine(CenterPosition - Vector3.up * 5, () =>
        {
            PoolManager.Instance.Push(this);
        }));
    }

    public void EnableStage()
    {
        ActiveStage = true;
        Converter.Init();
    }
    
    public void DisableStage()
    {
        ActiveStage = false;
    }

    private IEnumerator StageMoveRoutine(Vector3 dest, Action CallBack = null)
    {
        while (true)
        {
            var pos = transform.position;
            var lerp = Vector3.Lerp(pos, dest, 0.1f);
            transform.position = lerp;
            
            if (Vector3.Distance(pos, dest) <= 0.01f)
            {
                break;
            }

            yield return null;
        }

        transform.position = dest;
        CallBack?.Invoke();
    }
    
    public override void Init()
    {
        DisableStage();
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter}_Stage_{_stageOrder}";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(_stageEnterPoint, 0.5f);
        
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(_stageExitPoint, 0.5f);
        
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(_playerResetPoint, 0.5f);
    }
#endif
}
