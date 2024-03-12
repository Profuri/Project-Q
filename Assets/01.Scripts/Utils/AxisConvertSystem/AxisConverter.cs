using System;
using UnityEngine;

namespace AxisConvertSystem
{
    public class AxisConverter : MonoBehaviour
    {
        [SerializeField] private LayerMask _objectMask;
        
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
                        AxisType = axisType;
                        Convertable = true;
                        CancelChangeAxis(axisType);
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

        private void CancelChangeAxis(AxisType canceledAxis)
        {
            // var seq = DOTween.Sequence();
        
            // foreach (var unit in obstructiveUnits)
            // {
                // var reverseAxisDir = Vector3ExtensionMethod.GetAxisDir(canceledAxis) - Vector3.one;
                // var shakeDir = new Vector3(Mathf.Abs(reverseAxisDir.x), Mathf.Abs(reverseAxisDir.y), Mathf.Abs(reverseAxisDir.z));
                // seq.Join(unit.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            // }
        
            // seq.OnComplete(() =>
            // {
                // Player.Rigidbody.isKinematic = false;
            // });
            Player.Converter.ConvertDimension(AxisType.None);
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
            var center = Player.Collider.bounds.center;
            var dir = Vector3ExtensionMethod.GetAxisDir(axis); 

            var radius = ((CapsuleCollider)Player.Collider).radius;
            var height = ((CapsuleCollider)Player.Collider).height;
            
            var p1 = center + Vector3.up * (height / 2f);
            var p2 = center - Vector3.up * (height / 2f);

            var isHit1 = Physics.CapsuleCast(p1, p2, radius, dir, out front, Mathf.Infinity, _objectMask);
            var isHit2 = Physics.CapsuleCast(p1, p2, radius, -dir, out back, Mathf.Infinity, _objectMask);

            return !(isHit1 || isHit2);
        }
    }
}