using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class Chapter : InteractableObject
{
    [SerializeField] private ChapterData _data;
    
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        SceneControlManager.Instance.LoadScene(SceneType.Stage, () =>
        {
            StageManager.Instance.StartNewChapter(_data);
            PoolManager.Instance.Pop("Player");
        });
    }
}