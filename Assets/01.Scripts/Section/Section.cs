using System;
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

    private List<BridgeObject> _bridgeObjects;
    public List<ObjectUnit> SectionUnits { get; private set; }
    private List<Material> _sectionMaterials;

    private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");
    private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");

    public bool Active { get; private set; }
    public bool Lock { get; set; }
    public Vector3 CenterPosition { get; private set; }

    public virtual void Awake()
    {
        Active = false;
        Lock = false;
        _bridgeObjects = new List<BridgeObject>();
        SectionUnits = new List<ObjectUnit>();
        transform.GetComponentsInChildren(SectionUnits);
        
        _sectionMaterials = new List<Material>();
        var renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if(!renderer.enabled)
            {
                continue;
            }
            
            foreach (var material in renderer.materials) 
            {
                _sectionMaterials.Add(material);    
            }
        }
        
        DOTween.Init(true, true, LogBehaviour.Verbose). SetCapacity(2000, 100);
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

    public void LateUpdate()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.LateUpdateUnit();
            }
        }
    }

    public void Generate(Vector3 position, bool moveRoutine = true)
    {
        CenterPosition = position;
        transform.position = CenterPosition;
        if (moveRoutine)
        {
            Dissolve(true, 2.5f);
            transform.position = position - Vector3.up * _sectionData.sectionYDepth;
            transform.DOMove(CenterPosition, 3f);
        }
    }

    public void Disappear()
    {
        Dissolve(false, 2.5f);
        transform.DOMove(CenterPosition - Vector3.up * _sectionData.sectionYDepth, 3f)
            .OnComplete(() =>
            {
                PoolManager.Instance.Push(this);
                Active = false;
            });
    }

    public virtual void OnEnter(PlayerUnit player)
    {
        Active = true;
        Lock = true;
        player.SetSection(this);
        CameraManager.Instance.ChangeVCamController(VirtualCamType.SECTION);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).SetPlayer(player);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(AxisType.None);
    }

    public virtual void OnExit(PlayerUnit player)
    {
        CameraManager.Instance.ChangeVCamController(VirtualCamType.PLAYER);
        ((PlayerCamController)CameraManager.Instance.CurrentCamController).SetPlayer(player);
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
        var value = on ? 0f : 1f;
        
        foreach (var material in _sectionMaterials)
        {
            material.SetFloat(_visibleProgressHash, Mathf.Abs(1f - value));
            material.SetFloat(_dissolveProgressHash, Mathf.Abs(1f - value));
        }

        foreach (var material in _sectionMaterials)
        {
            DOTween.To(() => material.GetFloat(_visibleProgressHash),
                progress => material.SetFloat(_visibleProgressHash, progress), value, time);
            
            DOTween.To(() => material.GetFloat(_dissolveProgressHash),
                progress => material.SetFloat(_dissolveProgressHash, progress), value, time);
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