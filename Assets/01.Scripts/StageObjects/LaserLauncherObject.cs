using System.Collections;
using InteractableSystem;
using UnityEngine;
using StageStructureConvertSystem;

public class LaserLauncherObject : PlatformObjectUnit
{
    [SerializeField] private Transform _shotPointTrm;
    
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private float _refreshDelay;
    [SerializeField] private float _launchTime;
    [SerializeField] private float _activeDelay;

    [SerializeField] private float _laserDistance;
    
    private LineRenderer _laserRenderer;

    private bool _isActiveLaser;

    public override void Init(StructureConverter converter)
    {
        _laserRenderer = GetComponent<LineRenderer>();
        var position = _shotPointTrm.position;
        _laserRenderer.SetPositions(new[] { position, position } );
        _laserRenderer.enabled = false;
        _isActiveLaser = false;
        base.Init(converter);
        StartCoroutine(LaunchRoutine());
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
            _laserRenderer.enabled = true;
            yield return StartCoroutine(LaserActiveRoutine(true));
            _isActiveLaser = true;
            
            yield return waitForLaunch;
            yield return StartCoroutine(LaserActiveRoutine(false));

            if (ObstacleCheck(out var hit))
            {
                InteractOther(hit.collider, false);
            }
            _laserRenderer.enabled = false;
            _isActiveLaser = false;
            var position = _shotPointTrm.position;
            _laserRenderer.SetPositions(new[] { position, position } );
        }
    }

    private IEnumerator LaserActiveRoutine(bool active)
    {
        _isActiveLaser = false;

        var percent = 0f;
        var currentTime = 0f;
        var dest = ObstacleCheck(out var hit) ? hit.point : _shotPointTrm.position + transform.forward * _laserDistance;
        var index = active ? 1 : 0;

        while (percent <= 1f)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / _activeDelay;
            var lerpPos = Vector3.Lerp(_laserRenderer.GetPosition(index), dest, percent);
            _laserRenderer.SetPosition(index, lerpPos);
            yield return null;
        }
    }

    private void Launch()
    {
        if (ObstacleCheck(out var hit))
        {
            _laserRenderer.SetPosition(1, hit.point);
            InteractOther(hit.collider, true);
        }
        else
        {
            _laserRenderer.SetPosition(1, _shotPointTrm.position + transform.forward * _laserDistance);
        }
    }

    private void InteractOther(Collider col, bool interactValue)
    {
        if (col.TryGetComponent<PlayableObjectUnit>(out var playerUnit))
        {
            playerUnit.ReloadObject();
        }

        if (col.TryGetComponent<InteractableObject>(out var interactable))
        {
            if (interactable.Attribute.HasFlag(EInteractableAttribute.AFFECTED_FROM_LASER))
            {
                interactable.OnInteraction(null, interactValue);
            }
        }
    }

    private bool ObstacleCheck(out RaycastHit hit)
    {
        return Physics.Raycast(
            _shotPointTrm.position,
            transform.forward,
            out hit,
            _laserDistance,
            _obstacleMask
        );
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = ObstacleCheck(out var hit) ? Color.green : Color.red;
        
        if (_shotPointTrm == null)
        {
            return;
        }

        var position = _shotPointTrm.position;
        Gizmos.DrawLine(position, position + transform.forward * _laserDistance);
    }
#endif
}