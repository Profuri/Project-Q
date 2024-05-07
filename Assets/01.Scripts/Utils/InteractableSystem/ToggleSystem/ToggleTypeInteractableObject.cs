using System;
using System.Collections;
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

        private static Dictionary<int, SelectedBorder> s_selectedBorderDictionary = new Dictionary<int, SelectedBorder>();

        
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
        private bool _canFindBorders;
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
            if (_canFindBorders)
            {
                List<SelectedBorder> selectedBorderList = s_selectedBorderDictionary.Values.ToList();
                foreach (var selectedBorder in selectedBorderList)
                {
                    selectedBorder.SetDistanceProgress(AlphaValue,LastToggleState);
                }
            }
        }

        public override void Init(AxisConverter converter)
        {
            base.Init(converter);
            
            _lastToggleState = false;
            _canFindBorders = false;
            CallToggleChangeEvents(_lastToggleState);
        }

        public override void Dissolve(float value, float time, bool useDissolve = true, Action callBack = null)
        {
            List<SelectedBorder> borderList = s_selectedBorderDictionary.Values.ToList();
            if(borderList.Count > 0 )
            {
                foreach (SelectedBorder selectedBorder in borderList)
                {
                    s_selectedBorderDictionary.Remove(selectedBorder.GetInstanceID());
                    SceneControlManager.Instance.DeleteObject(selectedBorder);
                }
            }
            base.Dissolve(value, time, useDissolve, callBack);
        } 

        private bool ShowSelectedBorder(ObjectUnit showObj)
        {
            if (!IsInDistance || showObj is null) return false;

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
            return true;
        }

        private bool UnShowSelectedBorder(int instanceID)
        {
            if (LastToggleState) return false;
            if (s_selectedBorderDictionary.ContainsKey((instanceID)) == false) return false;
            
            s_selectedBorderDictionary[instanceID].Activate(false);
            
            SelectedBorder selectedBorder = s_selectedBorderDictionary[instanceID];
            s_selectedBorderDictionary.Remove(instanceID);
            SceneControlManager.Instance.DeleteObject(selectedBorder);
            return true;
        }
        
        public override void OnDetectedEnter(ObjectUnit communicator = null)
        {
            _canFindBorders = true;
            base.OnDetectedEnter(communicator);
            PlayerUnit player = SceneControlManager.Instance.Player;
                
            Vector3 playerPos = player.Collider.bounds.center;
            Vector3 curPos = Collider.bounds.center;
            playerPos.y = 0;
            curPos.y = 0;
                
            float distance = Vector3.Distance(playerPos, curPos);
            
            Debug.Log($"Distance: {distance}");
            ShowConnectedUnit();
        }

        public override void OnDetectedLeave(ObjectUnit communicator = null)
        {
            base.OnDetectedLeave(communicator);
            _canFindBorders = false;
            UnShowConnectedUnit();
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