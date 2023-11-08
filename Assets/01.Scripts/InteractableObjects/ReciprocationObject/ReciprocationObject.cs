using System;
using System.Numerics;
using InteractableSystem;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ReciprocationObject : InteractableObject
{
    [SerializeField] private Vector3 _reciprocateDir;

    [SerializeField] private float _reciprocateDistance;
    [SerializeField] private float _reciprocateSpeed;

    private Vector3 _originPos;
    private Vector3 _destPos;

    private void Awake()
    {
        _originPos = transform.position;
        _destPos = _originPos + _reciprocateDir * _reciprocateDistance;
    }

    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        var curPos = transform.position;
        var destPos = interactValue ? _destPos : _originPos;

        if (Vector3.Distance(curPos, destPos) <= 0.01f)
        {
            transform.position = destPos;
            return;
        }
        
        var lerpPos = Vector3.Lerp(curPos, destPos, _reciprocateSpeed * Time.deltaTime);
        
        transform.position = lerpPos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        var origin = transform.position;
        var dest = origin + _reciprocateDir * _reciprocateDistance;

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
