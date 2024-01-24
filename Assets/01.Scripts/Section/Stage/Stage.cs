using UnityEngine;
using StageStructureConvertSystem;

[RequireComponent(typeof(StructureConverter))]
public class Stage : Section
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    [SerializeField] private int _stageOrder;
    public int stageOrder => _stageOrder;

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

    public override void OnEnter(PlayerController player)
    {
        base.OnEnter(player);
        Converter.SetConvertable(true);

        if (StageManager.Instance.NextStage == this) 
        {
            StageManager.Instance.ChangeToNextStage(player);
        }
    }

    public override void OnExit(PlayerController player)
    {
        base.OnExit(player);
        Converter.SetConvertable(false);
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter}_Stage_{_stageOrder}";
    }
#endif
}
