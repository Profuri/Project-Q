using ModuleSystem;
using UnityEngine;

public class PlayerController : BaseModuleController
{
    [SerializeField]
    private PlayerDataSO _dataSO;
    public PlayerDataSO DataSO => _dataSO;

    private Transform _modelTrm;
    public Transform ModelTrm => _modelTrm;

public override void Start()
        {
        _modelTrm = transform.Find("Model");
        base.Start();
    }
}