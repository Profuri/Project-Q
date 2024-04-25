using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class PortalObject : InteractableObject
{
    [SerializeField] private PortalObject _linkedPortal;
    [SerializeField] private AxisType _portalAxis;

    [SerializeField] private float _portalOutDistance = 1f;

    private SoundEffectPlayer _soundEffectPlayer;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _soundEffectPlayer = new SoundEffectPlayer(this);
    }
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayerUnit playerUnit)
        {
            SoundManager.Instance.PlaySFX("Portal", false, _soundEffectPlayer);
            var center = _linkedPortal.Collider.bounds.center;
            var dest = center + Vector3ExtensionMethod.GetAxisDir(_linkedPortal._portalAxis) * _portalOutDistance;
            
            playerUnit.SetPosition(dest);
        }
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_portalAxis == AxisType.X)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (_portalAxis == AxisType.Y)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (_portalAxis == AxisType.Z)
        {
            transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
    }

#endif
}
