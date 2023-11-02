using ModuleSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class PlayerController : BaseModuleController
{
    [SerializeField]
    private PlayerDataSO _dataSO;
    public PlayerDataSO DataSO => _dataSO;

    private Transform _modelTrm;
    public Transform ModelTrm => _modelTrm;

    private StructureConverter _converter;
    public StructureConverter Converter => _converter;
    
    public override void Start()
    {
        _modelTrm = transform.Find("Model");
        _converter = transform.parent.GetComponent<StructureConverter>();
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.V))
        {
            _converter.ConvertDimension(EAxisType.NONE);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            _converter.ConvertDimension(EAxisType.X);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            _converter.ConvertDimension(EAxisType.Y);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _converter.ConvertDimension(EAxisType.Z);
        }
    }
}