using InteractableSystem;
using UnityEngine;

public class PressurePlate : MonoBehaviour, IInteractable
{
    public Transform GetTransform => transform;
    public EInteractType InteractType => EInteractType.INTERACT_SELF;

    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;

    [SerializeField] private PressureAffectedObject _affectedObject;
    
    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private void Awake()
    {
        _affectedObject = null;
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
    }

    private void Update()
    {
        OnInteraction(null, CheckPressed());
    }

    public void OnInteraction(PlayerController player, bool interactValue)
    {
        var current = _pressureMainTrm.localScale.y;
        var dest = interactValue ? _minHeight : _maxHeight;

        if (dest.ToString() != current.ToString() && Mathf.Abs(dest - current) <= 0.01f)
        {
            current = dest;
            _affectedObject?.OnInteraction(null, interactValue);
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
        var cols = Physics.OverlapBox(checkPos, checkSize / 2, Quaternion.identity, _pressionorMask);

        return cols.Length > 0;
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
