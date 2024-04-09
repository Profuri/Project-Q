using AxisConvertSystem;
using UnityEngine;

public class PictureUnit : ObjectUnit
{
    [SerializeField] private AxisType _enableAxis;

    private PictureObject _owner;
    
    private Renderer _renderer;

    private Material _enableMat;
    private Material _disableMat;

    private bool _isEnableUnit;

    public void SetPictureUnit(PictureObject owner, Material enableMat, Material disableMat)
    {
        _owner = owner;
        
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

        _isEnableUnit = false;

        if (axis == AxisType.None)
        {
            _isEnableUnit = true;
            _renderer.material = _disableMat;
            MaterialResetUp();
        }
        else if (axis == _enableAxis)
        {
            _isEnableUnit = true;
            _renderer.material = _enableMat;
            MaterialResetUp();
        }
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        if (_isEnableUnit)
        {
            ConvertedInfo.LocalPos.SetAxisElement(axis, 0f);
        }
        
        base.ApplyUnitInfo(axis);
    }

    public override void ApplyDepth()
    {
        Hide(!_isEnableUnit);
    }
}