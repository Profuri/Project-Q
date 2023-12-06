using System;

namespace InteractableSystem
{
    [Flags]
    public enum EInteractableAttribute
    {
        NONE = 0,
        CAN_PRESS_THE_PRESSURE_PLATE = 1,
        AFFECTED_FROM_LASER = 2,
        DAMAGED_BY_THORN = 4,
    }
}