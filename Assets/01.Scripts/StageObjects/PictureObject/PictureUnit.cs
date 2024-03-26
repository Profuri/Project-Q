using AxisConvertSystem;
using UnityEngine;

public class PictureUnit : ObjectUnit
{
    [SerializeField] private AxisType _enableAxis;
    private Collider _collider;
    private Renderer _renderer;

    private Material _enableMat;
    private Material _disableMat;

    public void Init(Material enableMat, Material disableMat)
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        _collider.enabled = false;

        _enableMat = enableMat;
        _disableMat = disableMat;
    }

    public void ChangeAxis(AxisType axis)
    {
        if (axis == AxisType.None)
        {
            _renderer.material = _disableMat;
        }
        else if (axis == _enableAxis)
        {
            _renderer.material = _enableMat;
        }
        Activate(_enableAxis == axis || axis == AxisType.None);
    }
}