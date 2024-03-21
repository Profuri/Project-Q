using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIAnimation : ScriptableObject
{
    public float duration;
    public string targetTrmPath;
    public bool joinPrevAnimation;

    protected Transform targetTrm;

    public void SetTargetTrm(Transform componentTrm)
    {
        targetTrm = componentTrm.Find(targetTrmPath);
        if (targetTrm is null)
        {
            Debug.LogError($"[UIAnimation] transform path is invalid ( {targetTrmPath} )");
        }
    }
    
    public virtual void Init() { }
    public virtual Tween GetTween() { return null; }
}