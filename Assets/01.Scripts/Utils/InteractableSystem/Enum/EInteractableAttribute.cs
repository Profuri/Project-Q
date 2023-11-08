using System;

namespace InteractableSystem
{
    [Flags]
    public enum EInteractableAttribute
    {
        NONE = 0,
        CAN_PRESS_THE_PRESSURE_PLATE = 1,
    }
}