using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SectionCollisionChecker))]
public class Section : PoolableMono
{
    [SerializeField] private Vector3 _enterPoint;
    [SerializeField] private Vector3 _exitPoint;
    [SerializeField] private Vector3 _playerResetPoint;
    [SerializeField] private Vector3 _playerResetPointInClear;
    
    public Vector3 PlayerResetPoint => _playerResetPoint;
    public Vector3 PlayerResetPointInClear => _playerResetPointInClear;
    
    [SerializeField] private SectionData _sectionData;
    public SectionData SectionData => _sectionData;

    [SerializeField] private bool _setPlayerFollowCam = true;

    private List<BridgeObject> _bridgeObjects;
    public List<ObjectUnit> SectionUnits { get; private set; }
 
    public bool Active { get; set; }
    public bool Lock { get; set; }
    public Vector3 CenterPosition { get; private set; }

    public event Action OnEnterSectionEvent = null;
    public event Action OnExitSectionEvent = null;

    public virtual void Awake()
    {
        Active = false;
        Lock = false;
        _bridgeObjects = new List<BridgeObject>();
        SectionUnits = new List<ObjectUnit>();
    }

    protected virtual void FixedUpdate()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.FixedUpdateUnit();
            }
        }
    }

    protected virtual void Update()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.UpdateUnit();
            }
        }
    }

    public void Generate(Vector3 position, bool playDissolve = true, bool moveRoutine = true, float dissolveTime = 1.5f, Action Callback = null)
    {
        ReloadSectionUnits();
        CenterPosition = position;
        transform.position = CenterPosition;

        if (playDissolve)
        {
            Dissolve(true, dissolveTime);
            Callback?.Invoke();
        }

        if (moveRoutine)
        {
            transform.position = position - Vector3.up * _sectionData.sectionYDepth;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(CenterPosition, 3f));
            seq.AppendCallback(() => Callback?.Invoke());
        }
    }

    public virtual void Disappear(float dissolveTime = 1.5f, Action Callback = null)
    {
        ReloadSectionUnits();
        Dissolve(false, dissolveTime);
        transform.DOMove(CenterPosition - Vector3.up * _sectionData.sectionYDepth, 1.5f)
            .OnComplete(() =>
            {
                Active = false;
                Callback?.Invoke();
                SceneControlManager.Instance.DeleteObject(this);
            });
    }

    public void ReloadSectionUnits(bool includeInactiveUnit = true)
    {
        SectionUnits.Clear();
        SectionUnits = transform.GetComponentsInChildren<ObjectUnit>(includeInactiveUnit).ToList();
    }

    public virtual void OnEnter(PlayerUnit player)
    {
        Active = true;
        Lock = true;
        player.SetSection(this);
        CameraManager.Instance.ChangeVCamController(VirtualCamType.SECTION);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).SetPlayer(_setPlayerFollowCam ? player : null);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(AxisType.None);
        
        OnEnterSectionEvent?.Invoke();
    }

    public virtual void OnExit(PlayerUnit player)
    {
        CameraManager.Instance.ChangeVCamController(VirtualCamType.PLAYER);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetPlayer(_setPlayerFollowCam ? player : null);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetCurrentCam();
        
        OnExitSectionEvent?.Invoke();
    }
    
    public void ConnectOtherSection(Section other)
    {
        var dir = (_exitPoint - other._enterPoint).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
        }
        else
        {
            dir = new Vector3(0, 0, Mathf.Sign(dir.z));
        }
        
        var currentExitPoint = CenterPosition + _exitPoint;
        var nextEnterPoint = currentExitPoint + dir * _sectionData.sectionIntervalDistance;
        var nextStageCenter = nextEnterPoint - other._enterPoint.normalized * other._enterPoint.magnitude;
        
        GenerateBridge(currentExitPoint, nextEnterPoint);
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
            bridge.SetColliderSize(_sectionData.bridgeIntervalDistance);
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

    protected void Dissolve(bool on, float time)
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
        ReloadSectionUnits();
    }

    public override void OnPush()
    {
        Active = false;
        SectionUnits.Clear();

        OnEnterSectionEvent = null;
        OnExitSectionEvent = null;
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
        
        Gizmos.color = new Color(1, 1, 0, 0.75f);
        Gizmos.DrawSphere(_playerResetPointInClear, 0.3f);
    }
#endif
}