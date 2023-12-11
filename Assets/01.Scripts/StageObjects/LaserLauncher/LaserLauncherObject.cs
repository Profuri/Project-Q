using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using SingularityGroup.HotReload;
using UnityEngine;
using StageStructureConvertSystem;

[RequireComponent(typeof(LineRenderer))]
public class LaserLauncherObject : StructureObjectUnitBase
{
    [SerializeField] private Transform _shotPointTrm;
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private float _laserDistance;
    
    [Header("Breath Setting")] 
    [SerializeField] private bool _isBreath;
    
    [SerializeField] private float _refreshDelay;
    [SerializeField] private float _launchTime;
    [SerializeField] private float _activeDelay;

    private LineRenderer _laserRenderer;
    private Queue<LaserInfo> _laserInfos;

    private bool _isActiveLaser;

    public override void Init(StructureConverter converter)
    {
        _laserRenderer = GetComponent<LineRenderer>();
        _laserInfos = new Queue<LaserInfo>();
        
        DisableLaser();
        
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

    public void Update()
    {
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

            // if (ObstacleCheck(, out var hit))
            // {
            //     InteractOther(hit, false);
            // }
            //
            // var position = _shotPointTrm.position;
            // _laserRenderer.SetPositions(new[] { position, position } );
        }
    }

    private void EnableLaser()
    {
        _isActiveLaser = true;
        _laserRenderer.enabled = true;
    }

    private void DisableLaser()
    {
        _isActiveLaser = false;
        _laserRenderer.enabled = false;
    }

    #region 이전 코드

    // private IEnumerator LaserEnableRoutine()
    // {
    //     _isActiveLaser = false;
    //     _laserRenderer.enabled = true;
    //     _laserInfos.Clear();
    //     _laserInfos.Add(new LaserInfo{ origin = _shotPointTrm.position, dir = transform.forward });
    //     
    //     var percent = 0f;
    //     var currentTime = 0f;
    //     var dest = ObstacleCheck(out var hit) ? hit.point : _shotPointTrm.position + transform.forward * _laserDistance;
    //     var index = 1;
    //
    //     while (percent <= 1f)
    //     {
    //         currentTime += Time.deltaTime;
    //         percent = currentTime / _activeDelay;
    //         var lerpPos = Vector3.Lerp(_laserRenderer.GetPosition(index), dest, percent);
    //         _laserRenderer.SetPosition(index, lerpPos);
    //         yield return null;
    //     }
    //
    //     _isActiveLaser = true;
    // }
    //
    // private IEnumerator LaserDisableRoutine()
    // {
    //     yield return null;
    // }

    #endregion

    private void Launch()
    {
        var cnt = _laserInfos.Count * 2;
        _laserRenderer.positionCount = cnt;
        AddLaser(new LaserInfo{ origin = _shotPointTrm.position, dir = transform.forward });

        for (var i = 0; i < cnt; i += 2)
        {
            var info = _laserInfos.Peek();
            _laserRenderer.SetPosition(i, info.origin);
            
            if (ObstacleCheck(info, out var hit))
            {
                _laserRenderer.SetPosition(i + 1, hit.point);
                InteractOther(hit, info, true);
            }
            else
            {
                _laserRenderer.SetPosition(i + 1, info.origin + info.dir * _laserDistance);
            }
            
            _laserInfos.Dequeue();
        }
    }

    private void InteractOther(RaycastHit hit, LaserInfo lastLaser, bool interactValue)
    {
        var col = hit.collider;
        
        if (col.TryGetComponent<PlayableObjectUnit>(out var playerUnit))
        {
            playerUnit.ReloadObject();
        }

        if (col.TryGetComponent<InteractableObject>(out var interactable))
        {
            if (interactable.Attribute.HasFlag(EInteractableAttribute.AFFECTED_FROM_LASER))
            {
                interactable.OnInteraction(this, interactValue, hit.point, hit.normal, lastLaser.dir);
            }
        }
    }

    private bool ObstacleCheck(LaserInfo info, out RaycastHit hit)
    {
        return Physics.Raycast(
            info.origin,
            info.dir,
            out hit,
            _laserDistance,
            _obstacleMask
        );
    }

    public void AddLaser(LaserInfo info)
    {
        _laserInfos.Enqueue(info);
    }
}