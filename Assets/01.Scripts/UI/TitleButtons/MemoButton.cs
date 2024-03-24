using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class MemoButton : InteractableObject
{
    [SerializeField] private Transform _titleCanvasTrm;
    private MemoWindow _memoWindow;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (_memoWindow is null || !_memoWindow.poolOut)
        {
            _memoWindow = UIManager.Instance.GenerateUI("MemoWindow", _titleCanvasTrm) as MemoWindow;
            _memoWindow.transform.localPosition = new Vector3(0.52f, 0f, 6.56f);
            _memoWindow.transform.localRotation = Quaternion.identity;
        }
    }
}