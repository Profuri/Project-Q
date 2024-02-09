using InputControl;
using ModuleSystem;
using AxisConvertSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : BaseModuleController
{
    [SerializeField] private InputReader _inputReader;
    public InputReader InputReader => _inputReader;
    
    [FormerlySerializedAs("_dataSO")] [SerializeField] private PlayerData data;
    public PlayerData Data => data;

    public Transform ModelTrm { get; private set; }
    
    public PlayerUIController PlayerUIController { get; private set; }
    public PlayerAnimatorController PlayerAnimatorController { get; private set; }
    public AxisConverter Converter { get; private set; }
    public PlayerUnit PlayerUnit { get; private set; }
    public CharacterController CharController { get; private set; }

    public ushort EnableAxis { get; private set; }

    public override void OnPop()
    {
        EnableAxis = (ushort)(AxisType.X | AxisType.Y | AxisType.Z);
        ModelTrm = transform.Find("Model");
        PlayerAnimatorController = ModelTrm.GetComponent<PlayerAnimatorController>();
        PlayerUIController = transform.Find("PlayerCanvas").GetComponent<PlayerUIController>();
        PlayerUnit = GetComponent<PlayerUnit>();
        CharController = GetComponent<CharacterController>();
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