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

    private IInteractable _selectedInteractable = null;

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
        _selectedInteractable?.OnInteraction(Controller, true);
    }

    private IInteractable FindInteractable()
    {
        var cols = new Collider[_InteractableCheckLimit];
        var size = Physics.OverlapSphereNonAlloc(Controller.transform.position, _interactableRadius, cols, _interactableMask);

        for(var i = 0; i < size; ++i)
        {
            if (cols[i].TryGetComponent<IInteractable>(out var interactable))
            {
                if (interactable.InteractType == EInteractType.INPUT_RECEIVE)
                {
                    return interactable;
                }
            }
        }

        return null;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        if (Controller)
        {
            Gizmos.DrawWireSphere(Controller.transform.position, _interactableRadius);
        }
        
        if (_selectedInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Controller.transform.position, _selectedInteractable.GetTransform.position);
        }
    }
#endif
}
