using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class SettingButton : InteractableObject
{
    [SerializeField] private GameObject _window;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        _window.SetActive(true);
    }
}