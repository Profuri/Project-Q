using UnityEngine;
using AxisConvertSystem;

public class Stage : Section
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    [SerializeField] private int _stageOrder;
    public int stageOrder => _stageOrder;

    public override void OnEnter(PlayerUnit player)
    {
        base.OnEnter(player);
        player.Converter.SetConvertable(true);

        if (StageManager.Instance.NextStage == this) 
        {
            StageManager.Instance.ChangeToNextStage();
        }
    }

    public override void OnExit(PlayerUnit player)
    {
        base.OnExit(player);
        player.Converter.SetConvertable(false);
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter.ToString().ToUpperFirstChar()}_Stage_{_stageOrder}";
    }
#endif
}
