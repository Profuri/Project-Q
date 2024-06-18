using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class ReciprocationObject : InteractableObject
{
    [SerializeField] private Vector3 _reciprocationDir;

    [SerializeField] private float _reciprocationDistance;
    [SerializeField] private float _reciprocationSpeed;

    [SerializeField] private LayerMask _checkCollisionLayer;

    private Vector3 _originPos;
    private Vector3 _destPos;
    
    public Vector3 MovingVector { get; private set; }

    public override void Awake()
    {
        base.Awake();
        _originPos = transform.localPosition;
        _destPos = _originPos + _reciprocationDir * _reciprocationDistance;
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var curPos = transform.localPosition;  
        var destPos = interactValue ? _destPos : _originPos;
        var destPosOnConvert = destPos;

        if (!subUnit)
        {
            var layerDepth = (float)compressLayer * Vector3ExtensionMethod.GetAxisDir(Converter.AxisType).GetAxisElement(Converter.AxisType);
            destPosOnConvert.SetAxisElement(Converter.AxisType, layerDepth);
        }

        if (Vector3.Distance(curPos, destPosOnConvert) <= 0.01f)
        {
            MovingVector = Vector3.zero;
            transform.localPosition = destPosOnConvert;
            return;
        }

        var lerpPos = Vector3.Lerp(curPos, destPosOnConvert, _reciprocationSpeed * Time.deltaTime);
        MovingVector = lerpPos - curPos;

        transform.localPosition = lerpPos;

        float yDiff = transform.localPosition.y - lerpPos.y;

        if(yDiff > 0.01f)
        {
            CheckCollision();
        }
    }

    private void CheckCollision()
    {
        Vector3 center = Collider.bounds.center;
        Vector3 halfExtents = Collider.bounds.size * 0.5f;
        Quaternion quaternion = transform.rotation;
        float maxDistance = Collider.bounds.size.y + 0.1f;

        RaycastHit[] cols = Physics.BoxCastAll(center,halfExtents,Vector3.down,quaternion,maxDistance,_checkCollisionLayer);

        foreach(RaycastHit hit in cols)
        {
            if(hit.collider.TryGetComponent(out PlayerUnit playerUnit))
            {
                playerUnit.ReloadUnit();
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        var col = GetComponent<Collider>();

        var origin = transform.position;
        var dest = origin + _reciprocationDir * _reciprocationDistance;

        var size = col.bounds.size;

        Gizmos.DrawLine(origin, dest);
        Gizmos.DrawWireCube(origin, size);
        Gizmos.DrawWireCube(dest, size);
    }
#endif
}
