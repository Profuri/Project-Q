using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputControl;
using InteractableSystem;
using ModuleSystem;
using UnityEngine;

public class PlayerInteractionModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;
    
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private float _interactableRadius;

    [SerializeField] private int _InteractableCheckLimit;

    [SerializeField] private InteractableObject _selectedInteractable = null;

    public override void Init(Transform root)
    {
        base.Init(root);
        _inputReader.OnInteractionEvent += OnInteraction;
    }

    public override void UpdateModule()
    {
        _selectedInteractable = FindInteractable();
        Controller.PlayerUIController.SetKeyGuide(_selectedInteractable != null);
    }

    public override void FixedUpdateModule()
    {
        // Do Nothing
    }

    private void OnInteraction()
    {
        _selectedInteractable?.OnInteraction(Controller.PlayerUnit, true);
    }

    private InteractableObject FindInteractable()
    {
        var cols = new Collider[_InteractableCheckLimit];
        var size = Physics.OverlapSphereNonAlloc(Controller.CenterPoint.position, _interactableRadius, cols, _interactableMask);

        for(var i = 0; i < size; ++i)
        {
            InteractableObject[] interactables = cols[i].GetComponents<InteractableObject>();
            
            for(int j = 0; j < interactables.Length; ++j)
            {
                if (interactables[j] is null)
                {
                    continue;
                }
                
                if(interactables[j].InteractType == EInteractType.INPUT_RECEIVE)
                {
                    return interactables[j];
                }
            }
            //if (cols[i].TryGetComponent<InteractableObject>(out var interactable))
            //{
            //    if (interactable.InteractType == EInteractType.INPUT_RECEIVE)
            //    {
            //        return interactable;
            //    }
            //}
        }

        return null;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        if (Controller)
        {
            Gizmos.DrawWireSphere(Controller.CenterPoint.position, _interactableRadius);
        }
        
        if (_selectedInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Controller.CenterPoint.position, _selectedInteractable.transform.position);
        }
    }
#endif
}
