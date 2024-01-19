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

    public void InitStage()
    {
        SetActive(true);
        Converter.Init();
    }

    public void SetActive(bool value)
    {
        ActiveStage = value;
    }

    public void StageEnterEvent(PlayerController player)
    {
        Converter.SetConvertable(true);
        CameraManager.Instance.ChangeVCamController(VirtualCamType.STAGE);
        ((StageCamController)CameraManager.Instance.CurrentCamController).SetStage(this);
        ((StageCamController)CameraManager.Instance.CurrentCamController).ChangeStageCamera(EAxisType.NONE);
        
        if (StageManager.Instance.NextStage == this) 
        {
            StageManager.Instance.ChangeToNextStage(player);
        }
    }

    public void StageExitEvent(PlayerController player)
    {
        Converter.SetConvertable(false);
        
        CameraManager.Instance.ChangeVCamController(VirtualCamType.PLAYER);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetPlayer(player);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetCurrentCam();
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
        SetActive(false);
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
