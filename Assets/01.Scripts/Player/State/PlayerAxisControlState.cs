using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

public class PlayerAxisControlState : PlayerBaseState
{
    private AxisType _controllingAxis;
    
    public PlayerAxisControlState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        
        if (!Player.Converter.Convertable)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }

        _controllingAxis = AxisType.None;

        if (Player.Converter.AxisType == AxisType.None)
        {
            Player.InputReader.OnAxisControlEvent += AxisControlHandle;
            Player.InputReader.OnClickEvent += SelectAxisHandle;
            
            VolumeManager.Instance.SetAxisControlVolume(true, 0.2f);
            ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(true);
        }
        else
        {
            SelectAxisHandle();
        }
    }

    public override void UpdateState()
    {
        CalcCurrentControlAxis();
        LightManager.Instance.SetAxisLight(_controllingAxis);
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.InputReader.OnAxisControlEvent -= AxisControlHandle;
        Player.InputReader.OnClickEvent -= SelectAxisHandle;
        VolumeManager.Instance.SetAxisControlVolume(false, 0.2f);
        LightManager.Instance.SetAxisLight(AxisType.None);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(false);
    }
    
    private void CalcCurrentControlAxis()
    {
        var vCam = CameraManager.Instance.ActiveVCam;
        var camYValue = vCam.GetAxisYValue();
        var camXValue = vCam.GetAxisXValue();

        if (camYValue >= 45f)
        {
            _controllingAxis = AxisType.Y;
        }
        else 
        {
            if (camXValue >= -45f)
            {
                _controllingAxis = AxisType.Z;
            }
            else if (camXValue >= -90f)
            {
                _controllingAxis = AxisType.X;
            }
        } 
    }

    private void SelectAxisHandle()
    {
        Controller.ChangeState(typeof(PlayerIdleState));
        
        // block input

        Player.Converter.ConvertDimension(_controllingAxis, () =>
        {
            if (_controllingAxis != AxisType.None && !SafeConvertCheck(out var obstructiveUnits))
            {
                Player.Rigidbody.isKinematic = true;
                ShowObstructiveUnits(obstructiveUnits);
            }
        });
    }

    private void ShowObstructiveUnits(List<ObjectUnit> obstructiveUnits)
    {
        var seq = DOTween.Sequence();
        
        foreach (var unit in obstructiveUnits)
        {
            seq.Join(unit.transform.DOShakePosition(0.25f, 0.5f, 20));
        }

        seq.OnComplete(() =>
        {
            Player.Converter.ConvertDimension(AxisType.None);
            Player.Rigidbody.isKinematic = false;
        });
    }

    private bool SafeConvertCheck(out List<ObjectUnit> obstructiveUnits)
    {
        var center = Player.Collider.bounds.center;
        var radius = ((CapsuleCollider)Player.Collider).radius;
        var height = ((CapsuleCollider)Player.Collider).height;

        var p1 = center + Vector3.up * height / 2f;
        var p2 = center - Vector3.up * height / 2f;

        var cols = new Collider[10];
        Physics.OverlapCapsuleNonAlloc(p1, p2, radius, cols, Player.Data.obstructiveMask);

        obstructiveUnits = cols.Where(col => col is not null && col != Player.StandingUnit?.Collider)
            .Select(col => col.GetComponent<ObjectUnit>()).ToList();
        return obstructiveUnits.Count <= 0;
    }
}