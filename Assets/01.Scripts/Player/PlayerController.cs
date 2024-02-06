using InputControl;
using ModuleSystem;
using AxisConvertSystem;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : BaseModuleController
{
    [SerializeField] private InputReader _inputReader;
    public InputReader InputReader => _inputReader;
    
    [SerializeField] private PlayerDataSO _dataSO;
    public PlayerDataSO DataSO => _dataSO;

    public Transform ModelTrm { get; private set; }
    public Transform CenterPoint { get; private set; }
    
    public PlayerUIController PlayerUIController { get; private set; }
    public PlayerAnimatorController PlayerAnimatorController { get; private set; }
    public AxisConverter Converter { get; private set; }
    public PlayableObjectUnit PlayerUnit { get; private set; }
    public CharacterController CharController { get; private set; }

    public ushort EnableAxis { get; private set; }

    public override void OnPop()
    {
        EnableAxis = (ushort)(AxisType.X | AxisType.Y | AxisType.Z);
        ModelTrm = transform.Find("Model");
        PlayerAnimatorController = ModelTrm.GetComponent<PlayerAnimatorController>();
        CenterPoint = transform.Find("CenterPoint");
        PlayerUIController = transform.Find("PlayerCanvas").GetComponent<PlayerUIController>();
        PlayerUnit = GetComponent<PlayableObjectUnit>();
        CharController = transform.Find("Collider").GetComponent<CharacterController>();
        Converter = GetComponent<AxisConverter>();
        base.OnPop();
    }

    public void SetSection(Section section)
    {
        transform.SetParent(section.transform);
        Converter.Init(section);
    }

    private bool GetAxisEnabled(AxisType axis)
    {
        return (EnableAxis & (ushort)axis) != 0;
    }

    public void SetEnableAxis(AxisType axis, bool enabled)
    {
        if(enabled)
        {
            EnableAxis |= (ushort)axis;
        }
        else
        {
            EnableAxis ^= (ushort)axis;
            if(axis == Converter.AxisType)
            {
                ConvertDimension(AxisType.None);
            }
        }
    }

    public void SetEnableInput(bool enabled)
    {
        _inputReader.SetEnableInput(enabled);
    }

    public void ConvertDimension(AxisType axis)
    {
        var movement = GetModule<PlayerMovementModule>();
        movement.SetEnableMove(false);
        movement.StopImmediately();
        Converter.ConvertDimension(axis, () => movement.SetEnableMove(true));
    }
}