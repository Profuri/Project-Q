using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class RedstoneInteractable : IInteractable
{
    public InteractableObject _interactableUnit;

    public EAxisType applyAxisType;
    
    public void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        _interactableUnit?.OnInteraction(communicator,interactValue,param);
    }
}
public class RedstoneObjectUnit : InteractableObject
{
    [SerializeField] private List<RedstoneInteractable> _redstoneInteractableList;
    //레드스톤을 바닥으로 깔았을 경우에는 y나 3d나 똑같이 작용해야됨.

    //3d 상태일떄도 전이되게 만들어 놓을건지 정해야됨.
    [SerializeField] private bool _isOn = false;
    private StructureConverter _converter;

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private bool _isToggle;

    private void Update()
    {
        if(_isToggle && _isOn)
        {
            OnInteraction(null,true);
        }        
    }
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        _isOn = interactValue;
        

        Debug.Log($"ObjectName: {this.gameObject.name} Value: {_isOn}");

        if (interactValue)
        {
            _renderer.material.color = new Color(0, 0, 255);
        }
        else
        {
            _renderer.material.color = new Color(255, 0, 0);
        }


        int paramIndex = 0;
        try
        {
            //EAxisType currentAxisType = (EAxisType)param[paramIndex];

            // This code can be fixed. because of get axisType method.
            EAxisType currentAxisType = GameManager.Instance.PlayerController.Converter.AxisType;

            Debug.Log($"CurrentAxisType: {currentAxisType}");

            foreach (RedstoneInteractable interactable in _redstoneInteractableList)
            {
                var applyAxisType = interactable.applyAxisType;
                if (currentAxisType == applyAxisType)
                {
                    interactable.OnInteraction(communicator, interactValue, param);
                }
                else if (applyAxisType == EAxisType.X && applyAxisType == EAxisType.Y && applyAxisType == EAxisType.Z)
                {
                    interactable.OnInteraction(communicator,interactValue,param);
                }
                else
                {
                    //만약에 레드스톤을 사용하는 발판이 여러개면 수정되어야 할 수도 있음.
                    //This can be fixed. 
                    interactable.OnInteraction(communicator, false, param);
                }
            }
        }
        catch
        {
            Debug.LogError($"Can't convert params index: {paramIndex} to EAxisType");
        }
    }
}
