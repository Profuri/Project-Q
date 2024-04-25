using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class MemoButton : InteractableObject
{
    [SerializeField] private Transform _titleCanvasTrm;
    private MemoWindow _memoWindow;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        GenerateMemoWindow();
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
                SoundManager.Instance.PlaySFX("PanelPopup", false);
            }
        }
    }
}