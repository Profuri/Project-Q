using System.Collections;
using System.Collections.Generic;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SectionCollisionChecker))]
public class Section : PoolableMono
{
    [SerializeField] private Vector3 _enterPoint;
    [SerializeField] private Vector3 _exitPoint;
    [SerializeField] private Vector3 _playerResetPoint;
    public Vector3 PlayerResetPoint => _playerResetPoint;

    [SerializeField] private SectionData _sectionData;
    public SectionData SectionData => _sectionData;

    [SerializeField] private bool _setPlayerFollowCam = true;

    private List<BridgeObject> _bridgeObjects;
    public List<ObjectUnit> SectionUnits { get; private set; }

    public bool Active { get; set; }
    public bool Lock { get; set; }
    public Vector3 CenterPosition { get; private set; }

    public virtual void Awake()
    {
        Active = false;
        Lock = false;
        _bridgeObjects = new List<BridgeObject>();
        SectionUnits = new List<ObjectUnit>();
        ReloadSectionUnits();
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.FixedUpdateUnit();
            }
        }
    }

    public void Update()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.UpdateUnit();
            }
        }
    }

    public void Generate(Vector3 position, bool moveRoutine = true)
    {
        ReloadSectionUnits();
        CenterPosition = position;
        transform.position = CenterPosition;
        if (moveRoutine)
        {
            Dissolve(true, 1.5f);
            transform.position = position - Vector3.up * _sectionData.sectionYDepth;
            transform.DOMove(CenterPosition, 3f);
        }
    }

    public void Disappear()
    {
        ReloadSectionUnits();
        Dissolve(false, 1.5f);
        transform.DOMove(CenterPosition - Vector3.up * _sectionData.sectionYDepth, 1.5f)
            .OnComplete(() =>
            {
                PoolManager.Instance.Push(this);
                Active = false;
            });
    }

    public void ReloadSectionUnits()
    {
        SectionUnits.Clear();
        transform.GetComponentsInChildren(SectionUnits);
    }

    public virtual void OnEnter(PlayerUnit player)
    {
        Active = true;
        Lock = true;
        player.SetSection(this);
        CameraManager.Instance.ChangeVCamController(VirtualCamType.SECTION);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).SetPlayer(_setPlayerFollowCam ? player : null);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(AxisType.None);
    }

    public virtual void OnExit(PlayerUnit player)
    {
        CameraManager.Instance.ChangeVCamController(VirtualCamType.PLAYER);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetPlayer(_setPlayerFollowCam ? player : null);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetCurrentCam();
    }
    
    public void ConnectOtherSection(Section other)
    {
        var dir = (_exitPoint - other._enterPoint).normalized;

        if (dir.x > dir.z)
        {
            dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
        }
        else
        {
            dir = new Vector3(0, 0, Mathf.Sign(dir.z));
        }
        
        var exitPoint = CenterPosition + _exitPoint;
        var enterPoint = exitPoint + (dir * _sectionData.sectionIntervalDistance);
        var nextStageCenter = enterPoint - other._enterPoint.normalized * other._enterPoint.magnitude;
        
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

    private IEnumerator BridgeGenerateRoutine(Vector3 startPoint, Vector3 endPoint)
    {
        var waitSecond = new WaitForSeconds(_sectionData.bridgeGenerateDelay);
        
        var bridgeSize = _sectionData.bridgeWidth + _sectionData.bridgeIntervalDistance;
        var bridgeCount = (endPoint - startPoint).magnitude / bridgeSize;

        var bridgeDir = (endPoint - startPoint).normalized;
        var bridgeRotation = Quaternion.LookRotation(bridgeDir);

        for (var i = 1; i <= bridgeCount; i++)
        {
            var bridge = SceneControlManager.Instance.AddObject("Bridge") as BridgeObject;
            var bridgePos = startPoint + (bridgeDir * (i * bridgeSize) - bridgeDir * (bridgeSize / 2f));
            
            bridge.SetWidth(_sectionData.bridgeWidth);
            bridge.Generate(bridgePos, bridgeRotation, this);
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

    private void Dissolve(bool on, float time)
    {
        foreach (var unit in SectionUnits)
        {
            unit.Dissolve(on ? 0f : 1f, time);
        }
    }
    
    public override void OnPop()
    {
        Active = false;
        Lock = false;
    }

    public override void OnPush()
    {
        Active = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.75f);
        Gizmos.DrawSphere(_enterPoint, 0.3f);
        
        Gizmos.color = new Color(1, 0, 0, 0.75f);
        Gizmos.DrawSphere(_exitPoint, 0.3f);
        
        Gizmos.color = new Color(0, 1, 0, 0.75f);
        Gizmos.DrawSphere(_playerResetPoint, 0.3f);
    }
#endif
}