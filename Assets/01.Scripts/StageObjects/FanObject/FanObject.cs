using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class FanObject : InteractableObject
{
    [Header("Air Settings")] 
    [SerializeField] private FanAirAxisType _airAxis;
    [SerializeField] private float _collisionSize;
    [SerializeField] private float _airMaxHeight;
    [SerializeField] private float _airPower;
    [SerializeField] private LayerMask _effectedMask;
    
    [Header("Fan Settings")]
    [SerializeField] private float _fanMaxSpeed;
    [SerializeField] private float _fanAccelerator;
    [SerializeField] private float _fanDecelerator;

    private Transform _fanTrm;
    private ParticleSystem _airParticle;
    
    private float _currentFanSpeed;

    private bool _enabled;

    private List<ObjectUnit> _affectedUnits;

    public override void Awake()
    {
        base.Awake();

        var model = transform.Find("Model");
        _fanTrm = model.Find("Fan");
        _airParticle = _fanTrm.Find("AirParticle").GetComponent<ParticleSystem>();

        _affectedUnits = new List<ObjectUnit>();
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        
        RotateFan();
        FloatingOther();
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        EnableFan();
    }

    private void EnableFan()
    {
        _enabled = true;
        _airParticle.Play();
    }

    private void ReleaseFan()
    {
        _enabled = false;
        _airParticle.Stop();
    }
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (interactValue)
        {
            if (_enabled)
            {
                return;
            }
            
            EnableFan();
        }
        else
        {
            ReleaseFan();
        }
    }

    private void FloatingOther()
    {
        if (!_enabled)
        {
            return;
        }

        if (!CheckCollision(out var hits, out var size))
        {
            foreach (var unit in _affectedUnits)
            {
                unit.StopImmediately(true);
            }
            return;
        }
        
        _affectedUnits.Clear();
        
        for (var i = 0; i < size; i++)
        {
            var hit = hits[i];

            if (hit.collider.TryGetComponent(out ObjectUnit unit))
            {
                if (!unit.staticUnit)
                {
                    var airVelocity = unit.Rigidbody.velocity;
                    if (_airAxis is FanAirAxisType.X or FanAirAxisType.Y or FanAirAxisType.Z)
                    {
                        airVelocity.SetAxisElement((AxisType)((int)(_airAxis - 1) % 3 + 1), _airPower);
                    }
                    else
                    {
                        airVelocity.SetAxisElement((AxisType)((int)(_airAxis - 1) % 3 + 1), -_airPower);
                    }
                    unit.SetVelocity(airVelocity, false);   
                    _affectedUnits.Add(unit);
                }
            }
        }
    }

    private bool CheckCollision(out RaycastHit[] hits, out int size)
    {
        hits = new RaycastHit[10];
        var colSize = Vector3.one * _collisionSize;
        colSize.SetAxisElement((AxisType)((int)(_airAxis - 1) % 3 + 1), 0.1f);
        size = Physics.BoxCastNonAlloc(
            transform.position,
            colSize / 2f,
            GetAirDir(),
            hits, 
            transform.rotation,
            _airMaxHeight,
            _effectedMask
        );
        return size > 0;
    }

    private void RotateFan()
    {
        _currentFanSpeed += (_enabled ? _fanAccelerator : -_fanDecelerator) * Time.deltaTime;
        _currentFanSpeed = Mathf.Clamp(_currentFanSpeed, 0, _fanMaxSpeed);
        _fanTrm.Rotate(new Vector3(0, _currentFanSpeed * Time.deltaTime, 0));
    }

    private Vector3 GetAirDir()
    {
        if (_airAxis is FanAirAxisType.X or FanAirAxisType.Y or FanAirAxisType.Z)
        {
            return Vector3ExtensionMethod.GetAxisDir((AxisType)_airAxis);
        }
        else
        {
            return -Vector3ExtensionMethod.GetAxisDir((AxisType)((int)(_airAxis - 1) % 3 + 1));
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var center = transform.position + GetAirDir() * _airMaxHeight;
        var colSize = Vector3.one * _collisionSize;
        colSize.SetAxisElement((AxisType)((int)(_airAxis - 1) % 3 + 1), 0.1f);
        Gizmos.DrawWireCube(center, colSize);
    }
    
    private void OnValidate()
    {
        _airParticle = transform.Find("Model/Fan/AirParticle").GetComponent<ParticleSystem>();
        var particleMainSetting = _airParticle.main;
        particleMainSetting.startLifetime = _airMaxHeight / 10f;

        transform.eulerAngles = GetAxisRot();
    }

    private Vector3 GetAxisRot()
    {
        return _airAxis switch
        {
            FanAirAxisType.X => new Vector3(0, 0, -90),
            FanAirAxisType.Y => new Vector3(0, 0, 0),
            FanAirAxisType.Z => new Vector3(-90, 0, 0),
            FanAirAxisType.ReverseX => new Vector3(0, 0, 90),
            FanAirAxisType.ReverseY => new Vector3(180, 0, 0),
            FanAirAxisType.ReverseZ => new Vector3(90, 0, 0),
            _ => Vector3.zero // 기본값 처리
        };
    }

#endif
}
