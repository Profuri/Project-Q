using InputControl;
using ModuleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateOtherObjectModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _rotatePower;

    private RotationObject _selectedObj;

    private Vector2 _rotateValue;

    public override void Init(Transform root)
    {
        base.Init(root);

        _selectedObj = null;
        _rotateValue = Vector2.zero;
        _inputReader.OnRotateEvent += OnRotateEvent;
    }
    private void OnRotateEvent(Vector2 value)
    {
        value.x *= _rotatePower;
        value.y *= _rotatePower;
        
        _rotateValue = value;
    }

    public override void FixedUpdateModule()
    {
        
    }

    public override void UpdateModule()
    {
        if (_selectedObj == null) return;

        RotateSelectedObj();
    }

    private void RotateSelectedObj()
    {
        EAxisType curAxisType = GameManager.Instance.CurAxisType;
        switch (curAxisType)
        {
            case EAxisType.X:
                //x가 회전돌아야 함
                _selectedObj.transform.Rotate(_rotateValue.x, 0, 0, Space.Self);
                break;
            case EAxisType.Y:
                //y가 회전돌아야 함
                _selectedObj.transform.Rotate(0, _rotateValue.x, 0, Space.Self);
                break;
            case EAxisType.Z:
                //z가 회전돌아야 함
                _selectedObj.transform.Rotate(0, 0, _rotateValue.x, Space.Self);
                break;
            case EAxisType.NONE:
                //상관없이 돌아야 함
                _selectedObj.transform.Rotate(_rotateValue.y, _rotateValue.x, 0, Space.Self);
                break;
        }
    }

    public void SetInteractObject(RotationObject rotationObject)
    {
        _selectedObj = rotationObject;
    }

    public void UnSetObject()
    {
        _selectedObj = null; 
    }
}
