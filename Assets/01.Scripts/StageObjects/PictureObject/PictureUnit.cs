using AxisConvertSystem;
using UnityEngine;

public class PictureUnit : MonoBehaviour
{
    [SerializeField] private AxisType _enableAxis;
    private Collider _collider;

    public void Init()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void ChangeAxis(AxisType axis)
    {
        if (axis == AxisType.None)
        {
            gameObject.SetActive(true);
            _collider.enabled = false;
            return;
        }

        SetUp(_enableAxis == axis);
    }

    private void SetUp(bool enable)
    {
        gameObject.SetActive(enable);
        _collider.enabled = enable;
    }
}