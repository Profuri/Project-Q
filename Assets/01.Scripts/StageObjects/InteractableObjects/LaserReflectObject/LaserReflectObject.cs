using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LaserReflectObject : InteractableObject
{
    private const float RotateValue = 45f;

    private MeshRenderer _reflectPanel;
    private MeshRenderer _tempReflectPanel;

    private float _currentRotate;
    private bool _isControlRotate;

    public override void Awake()
    {
        base.Awake();

        _reflectPanel = GetComponent<MeshRenderer>();
        _tempReflectPanel = transform.parent.Find("TempLaserReflectPlate").GetComponent<MeshRenderer>();
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        ConvertedInfo.LocalRot = UnitInfo.LocalRot;
        ConvertedInfo.LocalScale = UnitInfo.LocalScale;
        base.ApplyUnitInfo(axis);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if(communicator is PlayerUnit)
        {
            if (_isControlRotate)
            {
                return;
            }

            _isControlRotate = true;
            _reflectPanel.enabled = false;
            _tempReflectPanel.enabled = true;

            _tempReflectPanel.transform.localRotation = _reflectPanel.transform.localRotation;
            _currentRotate = _tempReflectPanel.transform.localEulerAngles.y;
            _tempReflectPanel.transform.DOScale((Converter.AxisType == AxisType.None ? OriginUnitInfo.LocalScale : ConvertedInfo.LocalScale) * 1.25f, 0.25f);
            _tempReflectPanel.transform.DOLocalMoveY(0.15f, 0.25f);
            
            InputManager.Instance.UIInputReader.OnLeftArrowClickEvent += RotateLeftHandle;
            InputManager.Instance.UIInputReader.OnRightArrowClickEvent += RotateRightHandle;
            InputManager.Instance.UIInputReader.OnEnterClickEvent += EnterRotateEditHandle;
            InputManager.Instance.PlayerInputReader.OnInteractionEvent += EnterRotateEditHandle;
            
            InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, true);
        }
        else if(communicator is LaserLauncherObject laser)
        {
            var point   = (Vector3)param[0];
            var normal  = (Vector3)param[1];
            var lastDir = ((Vector3)param[2]).normalized;
            var rayDistance = (float)param[3];
            
            var dir = Vector3.Reflect(lastDir, normal).normalized;
            laser.SubtractDistance(rayDistance);

            laser.AddLaser(new LaserInfo { origin = point + dir.normalized, power = dir * laser.RemainDistance });
        }
    }

    private void RotateRightHandle()
    {
        StartSafeCoroutine("ReflectPanelRotateSequence", RotateSequence(0.25f, -RotateValue));
    }

    private void RotateLeftHandle()
    {
        StartSafeCoroutine("ReflectPanelRotateSequence", RotateSequence(0.25f, RotateValue));
    }

    private void EnterRotateEditHandle()
    {
        _tempReflectPanel.transform.DOLocalMoveY(0f, 0.25f);
        _tempReflectPanel.transform.DOScale(Converter.AxisType == AxisType.None ? OriginUnitInfo.LocalScale : ConvertedInfo.LocalScale, 0.25f)
            .OnComplete(() =>
            {
                _reflectPanel.enabled = true;
                _tempReflectPanel.enabled = false;
            });

        _reflectPanel.transform.localRotation = Quaternion.Euler(Vector3.up * _currentRotate);
        UnitInfo.LocalRot = _reflectPanel.transform.localRotation;
        
        InputManager.Instance.UIInputReader.OnLeftArrowClickEvent -= RotateLeftHandle;
        InputManager.Instance.UIInputReader.OnRightArrowClickEvent -= RotateRightHandle;
        InputManager.Instance.UIInputReader.OnEnterClickEvent -= EnterRotateEditHandle;
        InputManager.Instance.PlayerInputReader.OnInteractionEvent -= EnterRotateEditHandle;
        
        InputManager.Instance.SetEnableInputAll(true);

        _isControlRotate = false;
    }

    private IEnumerator RotateSequence(float rotateTime, float rotateValue)
    {
        float timer = 0f;
        float percent = timer / rotateTime;

        _currentRotate += rotateValue;
        
        Quaternion originRotation = _tempReflectPanel.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(Vector3.up * _currentRotate);
        
        while (percent < 1f)
        {
            timer += Time.deltaTime;
            percent = timer / rotateTime;

            _tempReflectPanel.transform.rotation = Quaternion.Lerp(originRotation, targetRotation, percent);
            yield return null;
        }
    }
}