using InteractableSystem;
using UnityEngine;
using static Core.Define;

public class InteractionMark : PoolableMono
{
    [SerializeField] private Transform _keyGuideUI;
    private InteractableObject _agentInteractable;

    [SerializeField] private float _xRot;

    public void Setting(InteractableObject interactable)
    {
        _agentInteractable = interactable;
        SynchronizeTransform();
        SynchronizeRotation();
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
        SynchronizeRotation();
    }
    private void SynchronizeRotation()
    {
        Quaternion targetRot = Quaternion.LookRotation(MainCam.transform.forward);
        Quaternion plusRot = Quaternion.Euler(new Vector3(_xRot, 0, 0));

        Quaternion result = targetRot * plusRot;
        _keyGuideUI.rotation = result;
    }
    private void SynchronizeTransform()
    {
        Collider collider = _agentInteractable.Collider;
        Vector3 offset = _agentInteractable.Offset;
        if(offset == Vector3.zero)
        {
            offset = Vector3.up * 2f;
        }

        if (collider == null)
        {
            SceneControlManager.Instance.DeleteObject(this);
            return;
        }
        transform.position = collider.bounds.center + offset;
    }
}
