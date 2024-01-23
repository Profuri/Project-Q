using UnityEngine;
using StageStructureConvertSystem;

[RequireComponent(typeof(StructureConverter), typeof(StageCollisionChecker))]
public class Stage : Section
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    public ChapterType Chapter => _chapter;
    [SerializeField] private int _stageOrder;
    public int stageOrder => _stageOrder;

    public bool ActiveStage { get; private set; }

    public StructureConverter Converter { get; private set; }

    public override void Awake()
    {
        base.Awake();
        Converter = GetComponent<StructureConverter>();
    }

    public void InitStage()
    {
        Converter.Init();
    }

    public void StageEnterEvent(PlayerController player)
    {
        Converter.SetConvertable(true);
        ActiveStage = true;
        
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
        ActiveStage = false;
        
        CameraManager.Instance.ChangeVCamController(VirtualCamType.PLAYER);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetPlayer(player);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetCurrentCam();
    }

    public override void Init()
    {
        ActiveStage = false;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter}_Stage_{_stageOrder}";
    }
#endif
}
