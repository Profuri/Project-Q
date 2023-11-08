using InteractableSystem;
using UnityEngine;

public class PressurePlate : InteractableObject
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;

    [SerializeField] private InteractableObject _affectedObject;
    
    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private void Awake()
    {
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
    }

    private void Update()
    {
        OnInteraction(null, CheckPressed());
    }

    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        _affectedObject?.OnInteraction(null, interactValue);
        
        var current = _pressureMainTrm.localScale.y;
        var dest = interactValue ? _minHeight : _maxHeight;

        if (Mathf.Abs(dest - current) <= 0.01f)
        {
            current = dest;
        }
        
        var scale = _pressureMainTrm.localScale;
        scale.y = Mathf.Lerp(current, dest, Time.deltaTime * _pressSpeed);
        _pressureMainTrm.localScale = scale;

    }

    private bool CheckPressed()
    {
        var checkPos = _pressureObjTrm.position
            + Vector3.up 
            * (_pressureObjTrm.localScale.y * _pressureMainTrm.localScale.y / 2 + _pressureObjTrm.localScale.y / 2);
        var checkSize = _pressureObjTrm.localScale;

        var cols = new Collider[1];
        var size = Physics.OverlapBoxNonAlloc(checkPos, checkSize / 2, cols, Quaternion.identity, _pressionorMask);

        if (size <= 0)
        {
            return false;
        }
        
        if (cols[0].TryGetComponent<InteractableObject>(out var interactable))
        {
            return interactable.Attribute.HasFlag(EInteractableAttribute.CAN_PRESS_THE_PRESSURE_PLATE);
        }

        return true;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_pressureObjTrm)
        {
            return;
        }
        
        Gizmos.color = Color.yellow;
        var checkPos = _pressureObjTrm.position
            + Vector3.up 
            * (_pressureObjTrm.localScale.y * _pressureMainTrm.localScale.y / 2 + _pressureObjTrm.localScale.y / 2);
        var checkSize = _pressureObjTrm.localScale;
        Gizmos.DrawWireCube(checkPos, checkSize);
    }
#endif
}
