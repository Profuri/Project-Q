using AxisConvertSystem;
using UnityEngine;

public class PictureUnit : ObjectUnit, IPassable
{
    [SerializeField] private AxisType _enableAxis;

    private PictureObject _owner;
    
    private Renderer _renderer;

    private Material _enableMat;
    private Material _disableMat;
    
    public bool PassableAfterAxis { get; set; }

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
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        if (!PassableAfterAxis)
        {
            ConvertedInfo.LocalPos.SetAxisElement(axis, 0f);
        }
        
        base.ApplyUnitInfo(axis);
    }

    public override void ApplyDepth()
    {
        Hide(Converter.AxisType == AxisType.None && PassableAfterAxis);
    }

    public void PassableCheck(AxisType axis)
    {
        if (axis == AxisType.None || axis != _enableAxis)
        {
            PassableAfterAxis = true;
        }
        else
        {
            PassableAfterAxis = false;
        }
    }
}