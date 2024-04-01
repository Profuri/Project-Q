using System;
using DG.Tweening;
using UnityEngine;
using System.Linq;
namespace AxisConvertSystem
{
    public class AxisConverter : MonoBehaviour
    {
        [SerializeField] private LayerMask _objectMask;
        [SerializeField, Range(0f, 1f)] private float _underOffset;
        
        public AxisType AxisType { get; private set; }

        public bool Convertable { get; private set; }

        public PlayerUnit Player { get; set; }
        private Section _section;

        private bool _cancelConvert;

        public void Init(Section section)
        {
            AxisType = AxisType.None;
            _section = section;
            SetConvertable(section is Stage);
            section.SectionUnits.ForEach(unit => unit.Init(this));
        }

        public void SetConvertable(bool convertable)
        {
            Convertable = convertable;
        }

        public void ConvertDimension(AxisType axisType, Action callback = null)
        {
            if (!Convertable || AxisType == axisType || (AxisType != AxisType.None && axisType != AxisType.None))
            {
                return;
            }

            _cancelConvert = false;
            Convertable = false;

            if(axisType == AxisType.None)
            {
                ChangeAxis(axisType);
            }

            if (CameraManager.Instance.CurrentCamController is SectionCamController)
            {
                ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(axisType, () =>
                {
                    if (!SafeConvertAxis(axisType, out var front, out var back))
                    {
                        HandleAxisConversionFailure(axisType, front.collider, back.collider);
                    }

                    if (_cancelConvert)
                    {
                        return;
                    }
                    
                    if(axisType != AxisType.None)
                    {
                        ChangeAxis(axisType);
                    }
                    
                    Convertable = true;
                    callback?.Invoke();
                });
            }
        }

        private void HandleAxisConversionFailure(AxisType axis, Collider frontCol, Collider backCol)
        {
            Player.CheckStandObject(out var hit);

            if (!(axis == AxisType.Y && frontCol is null && hit.GetComponent<Collider>() == backCol))
            {
                CancelChangeAxis(axis, frontCol, backCol);
                _cancelConvert = true;
            }
            // when y axis convert, already standing object
            else
            {
                var unit = backCol.GetComponent<ObjectUnit>();
                if (!unit.climbableUnit)
                {
                    Player.StandingUnit = unit;
                }
            }
        }

        private void CancelChangeAxis(AxisType canceledAxis, Collider frontCol, Collider backCol)
        {
            var seq = DOTween.Sequence();
        
            var reverseAxisDir = Vector3ExtensionMethod.GetAxisDir(canceledAxis) - Vector3.one;
            var shakeDir = new Vector3(Mathf.Abs(reverseAxisDir.x), Mathf.Abs(reverseAxisDir.y), Mathf.Abs(reverseAxisDir.z));

            if (frontCol != backCol && frontCol is not null)
            {
                seq.Join(frontCol.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            }

            if (backCol is not null)
            {
                seq.Join(backCol.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            }
        
            seq.OnComplete(() =>
            {
                AxisType = canceledAxis;
                Convertable = true;
                foreach (var unit in _section.SectionUnits)
                {
                    unit.RewriteUnitInfo();
                }
                Player.Converter.ConvertDimension(AxisType.None);
                InputManagerHelper.OnCancelingAxis();
            });
        }

        private void ChangeAxis(AxisType axisType)
        {
            Player.Converter.UnShowClimbableEffect();
            CameraManager.Instance.ShakeCam(1f, 0.1f);
            VolumeManager.Instance.Highlight(0.2f);
            LightManager.Instance.SetShadow(axisType == AxisType.None ? LightShadows.Soft : LightShadows.None);

            foreach (var unit in _section.SectionUnits)
            {
                unit.Convert(axisType);
            }
            foreach (var unit in _section.SectionUnits)
            {
                unit.DepthHandler.CalcDepth(axisType);
            }
            foreach (var unit in _section.SectionUnits)
            {
                unit.UnitSetting(axisType);
            }
            foreach (var unit in _section.SectionUnits)
            {
                unit.DepthSetting();
            }

            AxisType = axisType;
        }

        private bool SafeConvertAxis(AxisType axis, out RaycastHit front, out RaycastHit back)
        {
            var capsuleCol = (CapsuleCollider)Player.Collider;

            var boundsCenter = capsuleCol.bounds.center + Vector3.up * _underOffset;
            var center = capsuleCol.center;
            var dir = Vector3ExtensionMethod.GetAxisDir(axis); 

            var radius = capsuleCol.radius;
            var height = capsuleCol.height;
            
            var p1 = boundsCenter + center + Vector3.up * (-height * 0.5F);
            var p2 = p1 + Vector3.up * radius;

            var isHit1 = Physics.CapsuleCast(p1-dir, p2-dir, radius, dir, out front, Mathf.Infinity, _objectMask);
            var isHit2 = Physics.CapsuleCast(p1+dir, p2+dir, radius, -dir, out back, Mathf.Infinity, _objectMask);

            return !(isHit1 || isHit2);
        }

        public void ShowClimbableEffect()
        {
            foreach (ObjectUnit unit in _section.SectionUnits)
            {
                unit.ShowUnClimbableEffect();
            }
        }

        public void UnShowClimbableEffect()
        {
            foreach (ObjectUnit unit in _section.SectionUnits)
            {
                unit.UnShowClimbableEffect();
            }
        }
    }
}