using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class ToggleTypeInteractableObject : InteractableObject
    {
        [SerializeField] private List<AffectedObject> _affectedObjects;
        [SerializeField] private List<ToggleChangeEvent> _onToggleChangeEvents;

        private readonly Dictionary<int, SelectedBorder> _selectedBorderDictionary = new Dictionary<int, SelectedBorder>();
        
        private bool _lastToggleState;
        protected bool LastToggleState
        {
            get => _lastToggleState;
            set
            {
                _lastToggleState = value;
                if (value)
                {
                    ShowConnectedUnit();
                }
            }
        }

        protected bool _isToggle;
        private static float s_standardValue = 1.1f;
        
        private float AlphaValue
        {
            get
            {
                PlayerUnit player = SceneControlManager.Instance.Player;
                
                Vector3 playerPos = player.Collider.bounds.center;
                Vector3 curPos = Collider.bounds.center;
                playerPos.y = 0;
                curPos.y = 0;
                
                const float interactableRadius = 2f; 
                //float interactableRadius = player.Data.interactableRadius;
                float distance = Vector3.Distance(playerPos, curPos);
                float returnValue = s_standardValue - (float)(distance / interactableRadius);
                
                return returnValue;
            }
        }
                
        private bool IsInDistance
        {
            get
            {
                PlayerUnit player = SceneControlManager.Instance.Player;
                Vector3 playerPos = player.Collider.bounds.center;
                Vector3 curPos = Collider.bounds.center;
                //float interactableRadius = player.Data.interactableRadius;
                const float interactableRadius = 2f;

                return Vector3.Distance(playerPos, curPos) <= interactableRadius;
            }
        }

        public override void UpdateUnit()
        {
            base.UpdateUnit();
            
            foreach (var selectedBorder in _selectedBorderDictionary.Values)
            {
                selectedBorder.SetDistanceProgress(AlphaValue,LastToggleState);
            }
        }

        public override void Init(AxisConverter converter)
        {
            base.Init(converter);
            
            _lastToggleState = false;
            CallToggleChangeEvents(_lastToggleState);
            ShowConnectedUnit();
        }

        public override void OnPush()
        {
            UnShowConnectedUnit();
            base.OnPush();
        }

        private bool ShowSelectedBorder(ObjectUnit showObj)
        {
            if (!IsInDistance || showObj is null) return false;

            int instanceID = showObj.GetInstanceID();
            float alpha = AlphaValue;

            if (_selectedBorderDictionary.ContainsKey(instanceID) == false || _selectedBorderDictionary[instanceID] == null)
            {
                var selectedBorder = SceneControlManager.Instance.AddObject("SelectedBorder") as SelectedBorder;
                
                selectedBorder.Setting(showObj);
                _selectedBorderDictionary[instanceID] = selectedBorder;
            }
            
            _selectedBorderDictionary[instanceID].SetDistanceProgress(alpha,LastToggleState);
            return true;
        }

        private bool UnShowSelectedBorder(int instanceID)
        {
            if (_selectedBorderDictionary.ContainsKey(instanceID) == false) return false;
            
            SelectedBorder selectedBorder = _selectedBorderDictionary[instanceID];
            _selectedBorderDictionary.Remove(instanceID);
            SceneControlManager.Instance.DeleteObject(selectedBorder);
            return true;
        }

        private void UnShowConnectedUnit()
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
            
            _selectedBorderDictionary.Clear();
        }

        private void ShowConnectedUnit()
        {
            foreach (var obj in _affectedObjects)
            {
                ShowSelectedBorder(obj.interactableObject);
            }
            
            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                var curEvent = toggleChangeEvent.invokeEvent;
                for (var index = 0; index < curEvent.GetPersistentEventCount(); index++)
                {
                    if(curEvent.GetPersistentTarget(index) is ObjectUnit unit)
                    {
                        ShowSelectedBorder(unit);
                    }                   
                }
            }
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
                
                toggleChangeEvent.Invoke(value);
            }
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
    }
}