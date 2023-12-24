using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Stage : PoolableMono
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    [SerializeField] private int _order;

    [Header("Stage Setting")]
    [SerializeField] private Vector3 _stageEnterPoint;
    public Vector3 StageEnterPoint => _stageEnterPoint;
    
    [SerializeField] private Vector3 _stageExitPoint;
    public Vector3 StageExitPoint => _stageExitPoint;

    [SerializeField] private Vector3 _playerResetPoint;
    public Vector3 PlayerResetPoint => _playerResetPoint;

    public Vector3 CenterPosition { get; private set; }
    private bool _activeStage;

    public void GenerateStage(Vector3 position)
    {
        transform.position = position - Vector3.up * 5;
        CenterPosition = position;
        StartCoroutine(StageMoveRoutine(CenterPosition));
    }

    public void DisappearStage()
    {
        _activeStage = false;
        StartCoroutine(StageMoveRoutine(CenterPosition - Vector3.up * 5));
    }

    private IEnumerator StageMoveRoutine(Vector3 dest)
    {
        while (true)
        {
            var pos = transform.position;
            var lerp = Vector3.Lerp(pos, dest, 0.3f);
            transform.position = lerp;
            
            if (Vector3.Distance(pos, dest) <= 0.01f)
            {
                break;
            }

            yield return null;
        }

        transform.position = dest;
    }
    
    public override void Init()
    {
        _activeStage = false;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter}_Stage_{_order}";
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
