using System;
using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class ToggleTypeInteractableObject : InteractableObject
    {
        [SerializeField] private List<AffectedObject> _affectedObjects;
        [SerializeField] private List<ToggleChangeEvent> _onToggleChangeEvents;

        protected static Dictionary<int, SelectedBorder> s_selectedBorderDictionary = new Dictionary<int, SelectedBorder>();

        private bool _lastToggleState;

        protected bool LastToggleState
        {
            get => _lastToggleState;
            set
            {
                _lastToggleState = value;
            }
        }

        protected bool _isToggle;
        protected const float _standardValue = 1.2f;
        public float AlphaValue
        {
            get
            {
                PlayerUnit player = SceneControlManager.Instance.Player;
                Vector3 playerPos = player.Collider.bounds.center;
                Vector3 curPos = Collider.bounds.center;

                float interactableRadius = player.Data.interactableRadius;

                playerPos.y = curPos.y = 0;

                return _standardValue - (Vector3.Distance(playerPos, curPos) / interactableRadius);
            }
        }
        public bool IsInDistance
        {
            get
            {
                PlayerUnit player = SceneControlManager.Instance.Player;
                Vector3 playerPos = player.Collider.bounds.center;
                Vector3 curPos = Collider.bounds.center;
                float interactableRadius = player.Data.interactableRadius;

                return Vector3.Distance(playerPos, curPos) <= interactableRadius;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _lastToggleState = false;
        }

        public override void UpdateUnit()
        {
            base.UpdateUnit();
            ShowSelectedBorderInConnectedUnit();
        }
        public override void Init(AxisConverter converter)
        {
            base.Init(converter);
            
            _lastToggleState = false;
            s_selectedBorderDictionary = new Dictionary<int, SelectedBorder>();
            CallToggleChangeEvents(_lastToggleState);
        }

        public override void Dissolve(float value, float time, bool useDissolve = true, Action callBack = null)
        {
            List<SelectedBorder> borderList = s_selectedBorderDictionary.Values.ToList();
            if(borderList.Count > 0 )
            {
                foreach (SelectedBorder selectedBorder in borderList)
                {
                    SceneControlManager.Instance.DeleteObject(selectedBorder);
                }
            }

            s_selectedBorderDictionary.Clear();
            base.Dissolve(value, time, useDissolve, callBack);
        } 

        protected void ShowSelectedBorder(ObjectUnit showObj)
        {
            if (!IsInDistance || showObj == null) return;

            int instanceID = showObj.GetInstanceID();
            float alpha = AlphaValue;

            if (s_selectedBorderDictionary.ContainsKey(instanceID) == false)
            {
                var selectedBorder = SceneControlManager.Instance.AddObject("SelectedBorder") as SelectedBorder;
                selectedBorder.Setting(showObj);
                selectedBorder.Activate(true);
                s_selectedBorderDictionary.Add(instanceID, selectedBorder);
            }
            s_selectedBorderDictionary[instanceID].Activate(true);
            s_selectedBorderDictionary[instanceID].SetDistanceProgress(alpha,LastToggleState);
        }

        protected bool UnShowSelectedBorder(int instanceID)
        {
            if(!s_selectedBorderDictionary.ContainsKey(instanceID))
            {
                return false;
            }

            float alpha = AlphaValue;
            s_selectedBorderDictionary[instanceID].SetDistanceProgress(alpha, LastToggleState);
            return true;
        }
        protected void InteractAffectedObjects(bool value)
        {
            if (Converter != null && !Converter.Convertable)
            {
                return;
            }

            foreach (var obj in _affectedObjects)
            {
                if (obj is not null)
                {
                    obj.Invoke(null, value);
                }
            }
        }

        public override void OnDetectedEnter(ObjectUnit communicator = null)
        {
            base.OnDetectedEnter(communicator);
            ShowSelectedBorderInConnectedUnit();
        }

        protected void CallToggleChangeEvents(bool value)
        {
            if (Converter != null && !Converter.Convertable)
            {
                return;
            }

            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                if (toggleChangeEvent is null)
                {
                    return;
                }

                var curEvent = toggleChangeEvent.invokeEvent;
                for (var index = 0; index < curEvent.GetPersistentEventCount(); index++)
                {
                    if(curEvent.GetPersistentTarget(index) is ObjectUnit unit)
                    {
                        if (value)
                        {
                            ShowSelectedBorder(unit);
                        }
                    }
                }
                toggleChangeEvent.Invoke(value);
            }
        }

        public void ShowSelectedBorderInConnectedUnit()
        {
            foreach (var obj in _affectedObjects)
            {
                Debug.Log($"Object: {obj}");
                Debug.Log($"AffectedObject: {obj.interactableObject}");
                ShowSelectedBorder(obj.interactableObject);
            }

            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                var curEvent = toggleChangeEvent.invokeEvent;
                for (var index = 0; index < curEvent.GetPersistentEventCount(); index++)
                {
                    if(curEvent.GetPersistentTarget(index) is ObjectUnit unit)
                    {
                        UnShowSelectedBorder(unit.GetInstanceID());
                    }                   
                }
            }
        }
        
        public void UnShowSelectedBorderInConnectedUnit()
        {
            foreach (var obj in _affectedObjects)
            {
                UnShowSelectedBorder(obj.interactableObject.GetInstanceID());
            }

            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                var curEvent = toggleChangeEvent.invokeEvent;
                for (var index = 0; index < curEvent.GetPersistentEventCount(); index++)
                {
                    if(curEvent.GetPersistentTarget(index) is ObjectUnit unit)
                    {
                        UnShowSelectedBorder(unit.GetInstanceID());
                    }
                }
            }
        }
    }
}