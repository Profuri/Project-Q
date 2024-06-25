using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class MemoButton : InteractableObject
{
    [SerializeField] private Transform _titleCanvasTrm;
    private MemoWindow _memoWindow;
    
    [SerializeField] private UIButton3D _button3D;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        GenerateMemoWindow();
    }
    
    public override void OnDetectedEnter(ObjectUnit communicator = null)
    {
        base.OnDetectedEnter(communicator);
        if (_button3D != null)
        {
            _button3D.OnHoverHandle();
        }
    }

    public override void OnDetectedLeave(ObjectUnit communicator = null)
    {
        base.OnDetectedLeave(communicator);
        if (_button3D != null)
        {
            _button3D.OnHoverCancelHandle();
        }
    }

    public void GenerateMemoWindow(bool isSound = true)
    {
        if (_memoWindow is null || !_memoWindow.poolOut)
        {
            _memoWindow = UIManager.Instance.GenerateUI("MemoWindow", _titleCanvasTrm) as MemoWindow;
            _memoWindow.transform.localPosition = new Vector3(0.77f, 0f, 6.36f);
            _memoWindow.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            if(isSound)
            {
                SoundManager.Instance.PlaySFX("PanelAppear", false);
            }
        }
    }
}