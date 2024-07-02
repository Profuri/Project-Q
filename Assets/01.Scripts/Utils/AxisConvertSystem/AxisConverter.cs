using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
            Stage stage = StageManager.Instance.CurrentStage;
            if (!Convertable || stage is null)
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
                    var frontUnit = front == null ? null : front.GetComponent<ObjectUnit>();
                    if (frontUnit is IPassable passable)
                    {
                        if (!passable.IsPassableAfterAxis())
                        {
                            CancelChangeAxis(AxisType, frontUnit, null, () => 
                            {
                                Convertable = true;
                                callback?.Invoke();
                            });
                            passable.PassableCheck(AxisType);
                            return;
                        }
                    }
                }
                SoundManager.Instance.PlaySFX("AxisControl");
                ChangeAxis(nextAxis);
            }
            
            if (CameraManager.Instance.CurrentCamController is SectionCamController)
            {
                ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(nextAxis, () =>
                {
                    if(nextAxis != AxisType.None)
                    {
                        if (!SafeConvertAxis(nextAxis, out var front, out var back))
                        {
                            var frontUnit = front == null ? null : front.GetComponent<ObjectUnit>();
                            var backUnit =  back == null ? null : back.GetComponent<ObjectUnit>();

                            HandleAxisConversionFailure(nextAxis, frontUnit, backUnit, () =>
                            {
                                Convertable = true;
                                ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(AxisType);
                                Player.OnControllingAxisEvent?.Invoke(AxisType);
                                callback?.Invoke();
                            });
                        }

                        if (_cancelConvert)
                        {
                            return;
                        }
                        
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
            CameraManager.Instance.ShakeCam(1f, 0.1f);
            VolumeManager.Instance.SetVolume(VolumeType.HighLight, 0.2f, true);
            LightManager.Instance.SetShadow(nextAxis == AxisType.None ? LightShadows.Soft : LightShadows.None);

            foreach (var unit in _section.SectionUnits) unit.Convert(nextAxis);
            foreach (var unit in _section.SectionUnits) unit.IntersectedUnits.Clear();
            foreach (var unit in _section.SectionUnits) unit.DepthHandler.CalcDepth(nextAxis);
            foreach (var unit in _section.SectionUnits) unit.ApplyUnitInfo(nextAxis);
            foreach (var unit in _section.SectionUnits) unit.ApplyDepth();

            AxisType = nextAxis;
            Player.OnControllingAxisEvent?.Invoke(AxisType);
        }


        private bool SafeConvertAxis(AxisType axis, out Collider front, out Collider back)
        {
            var boxCol = (BoxCollider)Player.Collider;

            var center = boxCol.bounds.center + Vector3.up * _underOffset;

            Vector3 halfExtents = boxCol.bounds.size * 0.7f / 2f;
            var dir = Vector3ExtensionMethod.GetAxisDir(axis);
            
            var isHit1 = Physics.BoxCast(center - dir, halfExtents, dir, out var frontHit, Quaternion.identity, Mathf.Infinity, _objectMask, QueryTriggerInteraction.Ignore);
            var isHit2 = Physics.BoxCast(center + dir, halfExtents, -dir, out var backHit, Quaternion.identity, Mathf.Infinity, _objectMask, QueryTriggerInteraction.Ignore);

            front = frontHit.collider;
            back = backHit.collider;
            
            if (!isHit1 && !isHit2)
            {
                var cols = new Collider[1];
                isHit1 = Physics.OverlapBoxNonAlloc(center, halfExtents, cols, Quaternion.identity, _objectMask, QueryTriggerInteraction.Ignore) > 0;
                front = cols[0];
            }
            
            return !(isHit1 || isHit2);
        }
    }
}