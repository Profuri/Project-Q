using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using UnityEngine;
using AxisConvertSystem;

[RequireComponent(typeof(LineRenderer))]
public class LaserLauncherObject : ObjectUnit
{
    [SerializeField] private Transform _shotPointTrm;
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private float _laserDistance;
    public float RemainDistance { get; private set; }
    
    [Header("Breath Setting")] 
    [SerializeField] private bool _isBreath;
    
    [SerializeField] private float _refreshDelay;
    [SerializeField] private float _launchTime;

    private LineRenderer _laserRenderer;
    private Queue<LaserInfo> _laserInfos;

    private bool _isActiveLaser;

    private SoundEffectPlayer _soundEffectPlayer;

    public override void Awake()
    {
        base.Awake();
        _laserRenderer = GetComponent<LineRenderer>();
        _laserInfos = new Queue<LaserInfo>();
        _soundEffectPlayer = new SoundEffectPlayer(this);
        DisableLaser();
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        
        if (_isBreath)
        {
            StartCoroutine(LaunchRoutine());
        }
        else
        {
            EnableLaser();
        }
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        
        bool isClear = StageManager.Instance.IsClear;
        if(isClear)
        {
            DisableLaser();
            return;
        }
        if (_isActiveLaser)
        {
            Launch();
        }
    }
    
    private IEnumerator LaunchRoutine()
    {
        var waitForRefresh = new WaitForSeconds(_refreshDelay);
        var waitForLaunch = new WaitForSeconds(_launchTime);
        
        while (true)
        {
            yield return waitForRefresh;
            EnableLaser();
            yield return waitForLaunch;
            DisableLaser();
        }
    }

    private void EnableLaser()
    {
        _isActiveLaser = true;
        _laserRenderer.enabled = true;
        SoundManager.Instance.PlaySFX("Laser",true,_soundEffectPlayer);
    }


    private void DisableLaser()
    {
        _isActiveLaser = false;
        _laserRenderer.enabled = false;
        _soundEffectPlayer.Stop();
    }

    private void Launch()
    {
        var cnt = _laserInfos.Count * 2;
        _laserRenderer.positionCount = cnt;
        RemainDistance = _laserDistance;
        AddLaser(new LaserInfo{ origin = Collider.bounds.center + transform.forward * _shotPointTrm.localPosition.z, power = transform.forward * _laserDistance });
        
        for (var i = 0; i < cnt; i += 2)
        {
            var info = _laserInfos.Peek();
            _laserRenderer.SetPosition(i, info.origin);
            
            if (ObstacleCheck(info, out var hit) && (hit.collider.excludeLayers & LayerMask.GetMask("Laser")) == 0)
            {
                _laserRenderer.SetPosition(i + 1, hit.point);
                InteractOther(hit, info, true);
            }
            else
            {
                _laserRenderer.SetPosition(i + 1, info.origin + info.power);
            }
            
            _laserInfos.Dequeue();
        }
    }

    private void InteractOther(RaycastHit hit, LaserInfo lastLaser, bool interactValue)
    {
        var col = hit.collider;

        if (col.TryGetComponent<InteractableObject>(out var interactable))
        {
            if (interactable.Attribute.HasFlag(EInteractableAttribute.AFFECTED_FROM_LASER))
            {
                var rayDistance = hit.distance;
                interactable.OnInteraction(this, interactValue, hit.point, hit.normal, lastLaser.power, rayDistance);
                return;
            }
        }
        
        if (col.TryGetComponent<ObjectUnit>(out var unit))
        {
            if (!unit.staticUnit)
            {
                unit.ReloadUnit();
            }
        }
    }

    private bool ObstacleCheck(LaserInfo info, out RaycastHit hit)
    {
        return Physics.Raycast(
            info.origin,
            info.power,
            out hit,
            _laserDistance,
            _obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public void AddLaser(LaserInfo info)
    {
        _laserInfos.Enqueue(info);
    }

    public void SubtractDistance(float distance)
    {
        RemainDistance = Mathf.Clamp(RemainDistance - distance, 0f, _laserDistance);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_shotPointTrm)
        {
            Gizmos.color = Color.red;
            var col = GetComponent<Collider>();
            var forward = transform.forward;

            var origin = col.bounds.center + forward * _shotPointTrm.localPosition.z;
            var dest = origin + forward * _laserDistance;
            
            Gizmos.DrawLine(origin, dest);
        }
    }
#endif
}