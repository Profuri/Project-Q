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
            gameObject.SetActive(true);
            _collider.enabled = false;
            _renderer.material = _disableMat;
            return;
        }

        SetUp(_enableAxis == axis);
    }

    private void SetUp(bool enable)
    {
        gameObject.SetActive(enable);
        _collider.enabled = enable;

        if (enable)
        {
            _renderer.material = _enableMat;
        }
    }
}