using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class StageAxisManager : BaseManager<StageAxisManager>
{
    [SerializeField] private Transform _stageTrm;

    private Transform _axisTransform;
    private List<Transform> _mapObjects;
    
    private EAxisType _axisType;
    public EAxisType AxisType => _axisType;

    private Vector3 _stageOriginScale;

    public override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnAxisTypeChangeEvent += SetAxisType;
    }

    public override void StartManager()
    {
        _axisTransform = _stageTrm.Find("Axis");
        _mapObjects = new List<Transform>();

        for (var i = 0; i < _axisTransform.childCount; i++)
        {
            var child = _axisTransform.GetChild(i);
            
            if (child.name == "Plane")
                _stageOriginScale = child.localScale;
            
            _mapObjects.Add(child);
        }
        
        SetAxisType(EAxisType.NONE);
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    private void SetAxisType(EAxisType type)
    {
        ReturnOriginalAxis();
        _axisType = type;
        CompressAxis();
    }

    private void CompressAxis()
    {
        var axis = Vector3.zero;
        
        switch (_axisType)
        {
            case EAxisType.EXPRESSION_X:
                axis = Vector3.left * (_stageOriginScale.x / 2);
                break;
            case EAxisType.EXPRESSION_Y:
                axis = Vector3.down * (_stageOriginScale.y / 2);
                break;
            case EAxisType.EXPRESSION_Z:
                axis = Vector3.back * (_stageOriginScale.z / 2);
                break;
            default:
                axis = Vector3.zero;
                break;
        }

        _axisTransform.localPosition = axis;
        foreach (var obj in _mapObjects)
        {
            obj.localPosition -= axis;
        }
        
        
    }

    private void ReturnOriginalAxis()
    {
        var axis = _axisTransform.localPosition;
        _axisTransform.localPosition = Vector3.zero;
        foreach (var obj in _mapObjects)
        {
            obj.localPosition += axis;
        }
    }
}