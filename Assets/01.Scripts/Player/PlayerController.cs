using InputControl;
using ModuleSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class PlayerController : BaseModuleController
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerDataSO _dataSO;
    public PlayerDataSO DataSO => _dataSO;

    private Transform _modelTrm;
    public Transform ModelTrm => _modelTrm;

    private PlayerUIController _playerUIController;
    public PlayerUIController PlayerUIController => _playerUIController;

    private StructureConverter _converter;
    public StructureConverter Converter => _converter;

    private PlayableObjectUnit _playerUnit;
    public PlayableObjectUnit PlayerUnit => _playerUnit;

    private CharacterController _charController;
    public CharacterController CharController => _charController;

    public ushort EnableAxis { get; private set; }

    public override void Init()
    {
        EnableAxis = (ushort)(EAxisType.X | EAxisType.Y | EAxisType.Z);
        _modelTrm = transform.Find("Model");
        _playerUIController = transform.Find("PlayerCanvas").GetComponent<PlayerUIController>();
        _playerUnit = GetComponent<PlayableObjectUnit>();
        _charController = GetComponent<CharacterController>();
        base.Init();
    }

    public void SetStage(Stage stage)
    {
        transform.SetParent(stage.transform);
        _converter = stage.Converter;
    }

    private bool GetAxisEnabled(EAxisType axis)
    {
        return (EnableAxis & (ushort)axis) != 0;
    }

    public void SetEnableAxis(EAxisType axis, bool enabled)
    {
        if(enabled)
        {
            EnableAxis |= (ushort)axis;
        }
        else
        {
            EnableAxis ^= (ushort)axis;
            if(axis == _converter.AxisType)
            {
                ConvertDimension(EAxisType.NONE);
            }
        }
    }

    public void SetEnableInput(bool enabled)
    {
        _inputReader.SetEnableInput(enabled);
    }

    public override void Update()
    {
        base.Update();
        
        // test key
        // if (Input.GetKeyDown(KeyCode.V))
        // {
        //     ConvertDimension(EAxisType.NONE);
        // }
        // if (Input.GetKeyDown(KeyCode.B) && GetAxisEnabled(EAxisType.X)) 
        // {
        //     ConvertDimension(EAxisType.X);
        // }
        // if (Input.GetKeyDown(KeyCode.N) && GetAxisEnabled(EAxisType.Y))
        // {
        //     ConvertDimension(EAxisType.Y);
        // }
        // if (Input.GetKeyDown(KeyCode.M) && GetAxisEnabled(EAxisType.Z))
        // {
        //     ConvertDimension(EAxisType.Z);
        // }
    }

    public void ConvertDimension(EAxisType axis)
    {
        PlayerMovementModule movement = GetModule<PlayerMovementModule>();
        movement.SetEnableMove(false);
        movement.StopImmediately();
        _converter.ConvertDimension(axis, () => movement.SetEnableMove(true));
    }
}