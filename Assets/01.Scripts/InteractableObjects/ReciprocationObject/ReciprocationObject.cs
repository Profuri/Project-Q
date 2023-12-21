using System;
using System.Numerics;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class ReciprocationObject : InteractableObject
{
    [SerializeField] private Vector3 _reciprocationDir;

    [SerializeField] private float _reciprocationDistance;
    [SerializeField] private float _reciprocationSpeed;

    private Vector3 _originPos;
    private Vector3 _destPos;

    [SerializeField] private StructureConverter _converter;

    private void OnEnable()
    {
        _originPos = transform.position;
        _destPos = _originPos + _reciprocationDir * _reciprocationDistance;
        if(_converter == null)
        {
            _converter = transform.GetComponentInParent<StructureConverter>();
            if(_converter == null)
            {
                Debug.LogError("Set Converter to this Reciprocation Object");
            }
        }
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        var curPos = transform.position;  
        var destPos = interactValue ? _destPos : _originPos;

        switch (_converter.AxisType)
        {
            case EAxisType.X:
                destPos.x = 0f;
                break;
            case EAxisType.Y:
                destPos.y = 0f;
                break;
            case EAxisType.Z:
                destPos.z = 0f;
                break;
        }

        if (Vector3.Distance(curPos, destPos) <= 0.01f)
        {
            transform.position = destPos;
            InterEnd = true;
            return;
        }
        
        var lerpPos = Vector3.Lerp(curPos, destPos, _reciprocationSpeed * Time.deltaTime);
        
        transform.position = lerpPos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        var origin = transform.position;
        var dest = origin + _reciprocationDir * _reciprocationDistance;

        if (Application.isPlaying)
        {
            origin = _originPos;
            dest = _destPos;
        }

        var size = transform.localScale;

        Gizmos.DrawLine(origin, dest);
        Gizmos.DrawWireCube(origin, size);
        Gizmos.DrawWireCube(dest, size);
    }
#endif
}
