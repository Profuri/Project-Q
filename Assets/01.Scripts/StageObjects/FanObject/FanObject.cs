using System;
using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
        var model = transform.Find("Model");
        _fanTrm = model.Find("Fan");
        _airParticle = _fanTrm.Find("AirParticle").GetComponent<ParticleSystem>();

        _affectedUnits = new List<ObjectUnit>();
        
        base.Awake();
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        
        RotateFan();
        CheckAffectedUnit();
    }

    public void LateUpdate()
    {
        FloatingUnits();
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        EnableFan();
    }

    public override void Activate(bool active)
    {
        base.Activate(active);
        if (active)
        {
            if (_enabled)
            {
                EnableFan();
            }
            else
            {
                ReleaseFan();
            }
        }
        else
        {
            var prev = _enabled;
            ReleaseFan();
            _enabled = prev;
        }
    }

    protected override void Hide(bool hide)
    {
        if (!hide)
        {
            if (_enabled)
            {
                EnableFan();
            }
            else
            {
                ReleaseFan();
            }
        }
        else
        {
            var prev = _enabled;
            ReleaseFan();
            _enabled = prev;
        }
        base.Hide(hide);
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

    private void CheckAffectedUnit()
    {
        if (!_enabled)
        {
            return;
        }

        if (!CheckCollision(out var cols))
        {
            foreach (var unit in _affectedUnits)
            {
                unit.StopImmediately(false);
                unit.useGravity = true;
            }
            _affectedUnits.Clear();
            return;
        }

        foreach (var col in cols)
        {
            if (col.TryGetComponent(out ObjectUnit unit))
            {
                if (_affectedUnits.Contains(unit))
                {
                    continue;
                }
                
                if (!unit.staticUnit)
                {
                    _affectedUnits.Add(unit);
                }
            }
        }
    }

    private void FloatingUnits()
    {
        foreach (var unit in _affectedUnits)
        {
            var velocity = unit.Rigidbody.velocity;
            
            if (Mathf.Abs(velocity.y) >= 0.01f)
            {
                velocity.y -= Mathf.Sign(velocity.y) * 
                              GameManager.Instance.CoreData.gravityScale * 
                              Mathf.Sqrt(Mathf.Abs(velocity.y)) * Time.deltaTime;
            }
            else
            {
                velocity.y = 0f;
            }

            var airPower = GetAirDir() * _airPower;
            velocity.SetAxisElement(GetAirNormalAxis(), airPower.GetAxisElement(GetAirNormalAxis()));


            unit.SetGravity(false);
            unit.SetVelocity(velocity, false);
        }
    }

    private bool CheckCollision(out Collider[] cols)
    {
        var center = transform.position + GetAirDir() * (_airMaxHeight / 2f);
        var colSize = Vector3.one * _collisionSize;
        colSize.SetAxisElement(GetAirNormalAxis(), _airMaxHeight);

        cols = Physics.OverlapBox(center, colSize / 2f, Quaternion.identity, _effectedMask);
        
        return cols.Length > 0;
    }

    private void RotateFan()
    {
        _currentFanSpeed += (_enabled ? _fanAccelerator : -_fanDecelerator) * Time.deltaTime;
        _currentFanSpeed = Mathf.Clamp(_currentFanSpeed, 0, _fanMaxSpeed);
        _fanTrm.Rotate(new Vector3(0, _currentFanSpeed * Time.deltaTime, 0));
    }

    private Vector3 GetAirDir()
    {
        if ((int)_airAxis >= 4)
        {
            return -Vector3ExtensionMethod.GetAxisDir(GetAirNormalAxis());
        }
        else
        {
            return Vector3ExtensionMethod.GetAxisDir(GetAirNormalAxis());
        }
    }

    private AxisType GetAirNormalAxis()
    {
        return (AxisType)((int)(_airAxis - 1) % 3 + 1);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var center = transform.position + GetAirDir() * (_airMaxHeight / 2f);
        var colSize = Vector3.one * _collisionSize;
        colSize.SetAxisElement(GetAirNormalAxis(), _airMaxHeight);
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
