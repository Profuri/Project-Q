#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabgrid
{
    public class EventHandler
    {
        public Action<Event> OnSceneGUI;
        public Action<Event> OnMouseMove;
        public Action<Event> OnMouseDrag;
        public Action<Event> OnMouseUp;
        //public Action<Event> OnMouseDown;
        public Action<Event> OnKeyDown;
        public Action<Event> OnKeyUp;

        public Dictionary<string, Action<Event>> OnMouseDown = new Dictionary<string, Action<Event>>();

        public void ProcessEvents(Event e, Tool currentTool)
        {
            OnSceneGUI?.Invoke(e);

            switch (e.type)
            {
                case EventType.MouseDrag:
                    OnMouseDrag?.Invoke(e);
                    break;

                case EventType.MouseMove:
                    OnMouseMove?.Invoke(e);
                    break;

                case EventType.MouseDown:
                    {
                        foreach (var action in OnMouseDown.Values)
                            action?.Invoke(e);
                    }
                    break;

                case EventType.MouseUp:
                    OnMouseUp?.Invoke(e);
                    break;

                case EventType.KeyDown:
                    OnKeyDown?.Invoke(e);
                    currentTool?.OnKeyDown(e);
                    break;

                case EventType.KeyUp:
                    OnKeyUp?.Invoke(e);
                    currentTool?.OnKeyUp(e);
                    break;
            }
        }
    }
}

#endif