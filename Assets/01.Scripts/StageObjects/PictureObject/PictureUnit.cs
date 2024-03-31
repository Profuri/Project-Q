using AxisConvertSystem;
using UnityEngine;

public class PictureUnit : ObjectUnit
{
    [SerializeField] private AxisType _enableAxis;
    private Renderer _renderer;

    private Material _enableMat;
    private Material _disableMat;

    private bool _isEnableUnit;

    public void Init(Material enableMat, Material disableMat)
    {
        _renderer = GetComponent<Renderer>();

        _enableMat = enableMat;
        _disableMat = disableMat;
    }

    public void ChangeAxis(AxisType axis)
    {
        if (!activeUnit)
        {
            return;
        }

        if (axis == AxisType.None)
        {
            _renderer.material = _disableMat;
            MaterialResetUp();
        }
        else if (axis == _enableAxis)
        {
            _renderer.material = _enableMat;
            MaterialResetUp();
        }

        _isEnableUnit = _enableAxis == axis || axis == AxisType.None;
    }

    public override void DepthSetting()
    {
        base.DepthSetting();

        if (!activeUnit)
        {
            return;
        }

        Hide(!_isEnableUnit);
    }
}