using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class Chapter : InteractableObject
{
    [SerializeField] private ChapterData _data;
    
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        StageManager.Instance.StartNewChapter(_data);    
    }
}