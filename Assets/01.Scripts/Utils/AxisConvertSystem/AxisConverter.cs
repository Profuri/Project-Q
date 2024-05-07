using System;
using DG.Tweening;
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

        public void ConvertDimension(AxisType nextAxis, Action callback = null)
        {
            if (!Convertable)
            {
                return;
            }

            _cancelConvert = false;
            Convertable = false;
            
            foreach (var unit in _section.SectionUnits)
            {
                if (unit is IPassable passable)
                {
                    passable.PassableCheck(nextAxis);
                }
            }

            if(nextAxis == AxisType.None)
            {
                if (!SafeConvertAxis(AxisType, out var front, out var back))
                {
                    var frontUnit = front.collider ? front.collider.GetComponent<ObjectUnit>() : null;
                    if (frontUnit is IPassable { PassableLastAxis: true })
                    {
                        CancelChangeAxis(AxisType, frontUnit, null, () =>
                        {
                            Convertable = true;
                            callback?.Invoke();
                        });
                        return;
                    }
                }
                SoundManager.Instance.PlaySFX("AxisControl");
                ChangeAxis(nextAxis);
            }

            if (CameraManager.Instance.CurrentCamController is SectionCamController)
            {
                ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(nextAxis, () =>
                {
                    if (!SafeConvertAxis(nextAxis, out var front, out var back))
                    {
                        var frontUnit = front.collider ? front.collider.GetComponent<ObjectUnit>() : null;
                        var backUnit = back.collider ? back.collider.GetComponent<ObjectUnit>() : null;

                        HandleAxisConversionFailure(nextAxis, frontUnit, backUnit, () =>
                        {
                            Convertable = true;
                            Player.Converter.ConvertDimension(AxisType.None);
                            callback?.Invoke();
                        });
                    }

                    if (_cancelConvert)
                    {
                        return;
                    }
                    
                    if(nextAxis != AxisType.None)
                    {
                        SoundManager.Instance.PlaySFX("AxisControl");
                        ChangeAxis(nextAxis);
                    }

                    foreach (var obj in _section.SectionUnits)
                    {
                        obj.OnCameraSetting(nextAxis);
                    }
                    Convertable = true;
                    callback?.Invoke();
                });
            }
        }
        
        private void HandleAxisConversionFailure(AxisType axis, ObjectUnit frontUnit, ObjectUnit backUnit, Action onComplete = null)
        {
            Player.CheckStandObject(out var playerStandCol);

            // when y axis convert, already standing object
            if (axis == AxisType.Y && frontUnit is null && playerStandCol == backUnit.Collider)
            {
                if (!backUnit.climbableUnit)
                {
                    Player.StandingUnit = backUnit;
                }
            }
            else
            {
                if (frontUnit is null or IPassable { PassableAfterAxis: true } && 
                    backUnit is null or IPassable { PassableAfterAxis: true })
                {
                    return;
                }

                CancelChangeAxis(axis, frontUnit, backUnit, onComplete);
                _cancelConvert = true;
            }
        }

        private void CancelChangeAxis(AxisType canceledAxis, ObjectUnit frontUnit, ObjectUnit backUnit, Action onComplete = null)
        {
            SoundManager.Instance.PlaySFX("AxisControlFailure");

            var seq = DOTween.Sequence();
        
            var reverseAxisDir = Vector3ExtensionMethod.GetAxisDir(canceledAxis) - Vector3.one;
            var shakeDir = new Vector3(Mathf.Abs(reverseAxisDir.x), Mathf.Abs(reverseAxisDir.y), Mathf.Abs(reverseAxisDir.z));

            if (frontUnit != backUnit && frontUnit is not null)
            {
                seq.Join(frontUnit.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            }

            if (backUnit is not null)
            {
                seq.Join(backUnit.transform.DOShakePosition(0.25f, shakeDir * 0.5f, 30));
            }

            seq.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void ChangeAxis(AxisType nextAxis)
        {
            Player.Converter.UnShowClimbableEffect();
            CameraManager.Instance.ShakeCam(1f, 0.1f);
            VolumeManager.Instance.SetVolume(VolumeType.HighLight, 0.2f, true);
            LightManager.Instance.SetShadow(nextAxis == AxisType.None ? LightShadows.Soft : LightShadows.None);

            foreach (var unit in _section.SectionUnits) unit.Convert(nextAxis);
            foreach (var unit in _section.SectionUnits) unit.IntersectedUnits.Clear();
            foreach (var unit in _section.SectionUnits) unit.DepthHandler.CalcDepth(nextAxis);
            foreach (var unit in _section.SectionUnits) unit.ApplyUnitInfo(nextAxis);
            foreach (var unit in _section.SectionUnits) unit.ApplyDepth();

            AxisType = nextAxis;
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