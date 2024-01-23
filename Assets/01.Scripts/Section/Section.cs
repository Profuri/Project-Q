using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : PoolableMono
{
    [SerializeField] private Vector3 _enterPoint;
    public Vector3 EnterPoint => _enterPoint;
    
    [SerializeField] private Vector3 _exitPoint;
    public Vector3 ExitPoint => _exitPoint;

    [SerializeField] private Vector3 _playerResetPoint;
    public Vector3 PlayerResetPoint => _playerResetPoint;

    [SerializeField] private SectionData _sectionData;

    private List<BridgeObject> _bridgeObjects;

    public Vector3 CenterPosition { get; private set; }

    public virtual void Awake()
    {
        _bridgeObjects = new List<BridgeObject>();
    }

    public void Generate(Vector3 position)
    {
        transform.position = position - Vector3.up * 5;
        CenterPosition = position;
        StartCoroutine(SectionMoveRoutine(CenterPosition));
    }

    public void Disappear()
    {
        StartCoroutine(SectionMoveRoutine(CenterPosition - Vector3.up * 5, () =>
        {
            PoolManager.Instance.Push(this);
        }));
    }
    
    public void ConnectOtherSection(Section other)
    {
        var dir = (ExitPoint - other.EnterPoint).normalized;

        if (dir.x > dir.z)
        {
            dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
        }
        else
        {
            dir = new Vector3(0, 0, Mathf.Sign(dir.z));
        }
        
        var exitPoint = CenterPosition + ExitPoint;
        var enterPoint = exitPoint + (dir * _sectionData.sectionIntervalDistance);
        var nextStageCenter = enterPoint - 
                              (new Vector3(other.EnterPoint.x, 0, other.EnterPoint.z).normalized * other.EnterPoint.magnitude);
        
        GenerateBridge(exitPoint, enterPoint);
        other.Generate(nextStageCenter);
    }
    
    private void GenerateBridge(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(BridgeGenerateRoutine(startPoint, endPoint));
    }
    
    public void RemoveBridge()
    {
        StartCoroutine(BridgeRemoveRoutine());
    }
    
    private IEnumerator SectionMoveRoutine(Vector3 dest, Action callBack = null)
    {
        while (true)
        {
            var pos = transform.position;
            var lerp = Vector3.Lerp(pos, dest, 0.1f);
            transform.position = lerp;
            
            if (Vector3.Distance(pos, dest) <= 0.01f)
            {
                break;
            }

            yield return null;
        }

        transform.position = dest;
        callBack?.Invoke();
    }
    
    private IEnumerator BridgeGenerateRoutine(Vector3 startPoint, Vector3 endPoint)
    {
        var waitSecond = new WaitForSeconds(_sectionData.bridgeGenerateDelay);
        
        var bridgeSize = _sectionData.bridgeWidth + _sectionData.bridgeIntervalDistance;
        var bridgeCount = (endPoint - startPoint).magnitude / bridgeSize;

        var bridgeDir = (endPoint - startPoint).normalized;
        var bridgeRotation = Quaternion.LookRotation(bridgeDir);

        for (var i = 1; i <= bridgeCount; i++)
        {
            var bridge = PoolManager.Instance.Pop("Bridge") as BridgeObject;
            var bridgePos = startPoint + (bridgeDir * (i * bridgeSize) - bridgeDir * (bridgeSize / 2f));
            
            bridge.SetWidth(_sectionData.bridgeWidth);
            bridge.Generate(bridgePos, bridgeRotation);
            _bridgeObjects.Add(bridge);

            yield return waitSecond;
        }
    }
    
    private IEnumerator BridgeRemoveRoutine()
    {
        var waitSecond = new WaitForSeconds(_sectionData.bridgeGenerateDelay);
        
        foreach (var bridge in _bridgeObjects)
        {
            bridge.Remove();
            yield return waitSecond;
        }
        
        _bridgeObjects.Clear();
    }
    
    public override void Init()
    {
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(_enterPoint, 0.3f);
        
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(_exitPoint, 0.3f);
        
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(_playerResetPoint, 0.3f);
    }
#endif
}