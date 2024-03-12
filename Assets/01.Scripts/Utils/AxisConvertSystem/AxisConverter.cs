using System;
using DG.Tweening;
using TreeEditor;
using UnityEngine;

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
                        Player.CheckStandObject(out var hit);
                        
                        if (!(axisType == AxisType.Y && front.collider is null && hit.collider == back.collider))
                        {
                            CancelChangeAxis(axisType, front, back);
                            return;
                        }
                        // when y axis convert, already standing object
                        else
                        {
                        }
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

        private void CancelChangeAxis(AxisType canceledAxis, RaycastHit front, RaycastHit back)
        {
            var seq = DOTween.Sequence();
        
            var reverseAxisDir = Vector3ExtensionMethod.GetAxisDir(canceledAxis) - Vector3.one;
            var shakeDir = new Vector3(Mathf.Abs(reverseAxisDir.x), Mathf.Abs(reverseAxisDir.y), Mathf.Abs(reverseAxisDir.z));

            if (front.collider != back.collider && front.collider is not null)
            {
                seq.Join(front.collider.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            }

            if (back.collider is not null)
            {
                seq.Join(back.collider.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            }
        
            seq.OnComplete(() =>
            {
                AxisType = canceledAxis;
                Convertable = true;
                _section.SectionUnits.ForEach(unit => unit.RewriteUnitInfo());
                Player.Converter.ConvertDimension(AxisType.None);
            });
        }

        private void ChangeAxis(AxisType axisType)
        {
            CameraManager.Instance.ShakeCam(1f, 0.1f);
            VolumeManager.Instance.Highlight(0.2f);
            LightManager.Instance.SetShadow(axisType == AxisType.None ? LightShadows.Soft : LightShadows.None);

            _section.SectionUnits.ForEach(unit =>
            {
                unit.Convert(axisType);
            });
            
            _section.SectionUnits.ForEach(unit =>
            {
                unit.DepthHandler.CalcDepth(axisType);
            });
            
            _section.SectionUnits.ForEach(unit =>
            {
                unit.UnitSetting(axisType);
            });

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
    }
}