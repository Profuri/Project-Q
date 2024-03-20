using InteractableSystem;
using UnityEngine;
using static Core.Define;

public class InteractionMark : PoolableMono
{
    [SerializeField] private RectTransform _keyGuideUI;
    private InteractableObject _agentInteractable;

    public void Setting(InteractableObject interactable)
    {
        _agentInteractable = interactable;
    }

    public override void OnPop()
    {
        
    }

    public override void OnPush()
    {
        
    }
    
    private void Update()
    {
        SynchronizeTransform();
        _keyGuideUI.rotation = Quaternion.LookRotation(MainCam.transform.forward);
    }

    private void SynchronizeTransform()
    {
        Collider collider = _agentInteractable.Collider;
        
        float yOffset = _agentInteractable.Offset <= 0.1f 
            ? collider.bounds.size.y * 0.7f : _agentInteractable.Offset;

        Vector3 offset = new Vector3(0,yOffset,0);
            
        transform.SetParent(collider.transform);
        transform.position = collider.bounds.center + offset;
    }
}
