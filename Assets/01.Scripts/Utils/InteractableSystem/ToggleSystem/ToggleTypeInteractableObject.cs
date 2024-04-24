using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class ToggleTypeInteractableObject : InteractableObject
    {
        [SerializeField] private List<AffectedObject> _affectedObjects;
        [SerializeField] private List<ToggleChangeEvent> _onToggleChangeEvents;

        protected bool LastToggleState;

        public override void Awake()
        {
            base.Awake();
            LastToggleState = false;
        }

        public override void Init(AxisConverter converter)
        {
            base.Init(converter);
            LastToggleState = false;
            CallToggleChangeEvents(LastToggleState);
        }

        protected void InteractAffectedObjects(bool value)
        {
            foreach (var obj in _affectedObjects)
            {
                if (obj is null)
                {
                    return;
                }
                
                obj.interactableObject.SelectedBorderActivate(value);
                obj.Invoke(null, value);
            }
        }

        protected void CallToggleChangeEvents(bool value)
        {
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
                        unit.SelectedBorderActivate(value);
                    }
                }
                
                toggleChangeEvent.Invoke(value);
            }
        }

        public override void OnDetectedEnter(ObjectUnit communicator = null)
        {
            base.OnDetectedEnter(communicator);
            ShowSelectedBorder();
            ShowSelectedBorderInConnectedUnit();
        }

        public override void OnDetectedLeave(ObjectUnit communicator = null)
        {
            base.OnDetectedLeave(communicator);
            UnShowSelectedBorder();
            UnShowSelectedBorderInConnectedUnit();
        }

        public void ShowSelectedBorderInConnectedUnit()
        {
            foreach (var obj in _affectedObjects)
            {
                obj.interactableObject.ShowSelectedBorder();
            }

            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                var curEvent = toggleChangeEvent.invokeEvent;
                for (var index = 0; index < curEvent.GetPersistentEventCount(); index++)
                {
                    if(curEvent.GetPersistentTarget(index) is ObjectUnit unit)
                    {
                        unit.ShowSelectedBorder();
                    }
                }
            }
        }
        
        public void UnShowSelectedBorderInConnectedUnit()
        {
            foreach (var obj in _affectedObjects)
            {
                obj.interactableObject.UnShowSelectedBorder();
            }

            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                var curEvent = toggleChangeEvent.invokeEvent;
                for (var index = 0; index < curEvent.GetPersistentEventCount(); index++)
                {
                    if(curEvent.GetPersistentTarget(index) is ObjectUnit unit)
                    {
                        unit.UnShowSelectedBorder();
                    }
                }
            }
        }
    }
}