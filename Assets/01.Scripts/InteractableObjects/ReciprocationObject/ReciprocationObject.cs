using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ReciprocationObject : InteractableObject
{
    [SerializeField] private Vector3 _reciprocationDir;

    [SerializeField] private float _reciprocationDistance;
    [SerializeField] private float _reciprocationSpeed;

    private Vector3 _originPos;
    private Vector3 _destPos;

    private void OnEnable()
    {
        _originPos = transform.localPosition;
        _destPos = _originPos + _reciprocationDir * _reciprocationDistance;
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        var curPos = transform.localPosition;  
        var destPos = interactValue ? _destPos : _originPos;

        switch (StageManager.Instance.CurrentStageAxis)
        {
            case EAxisType.X:
                destPos.x = 0f;
                break;
            case EAxisType.Y:
                // destPos.y = 0f;
                break;
            case EAxisType.Z:
                destPos.z = 0f;
                break;
        }

        if (Vector3.Distance(curPos, destPos) <= 0.01f)
        {
            transform.localPosition = destPos;
            InterEnd = true;
            return;
        }
        
        var lerpPos = Vector3.Lerp(curPos, destPos, _reciprocationSpeed * Time.deltaTime);

        transform.localPosition = lerpPos;
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
