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
        if (_selectedInteractable == null)
        {
            return;
        }
        
        _selectedInteractable.OnInteraction(Controller);
    }

    private IInteractable FindInteractable()
    {
        var cols = Physics.OverlapSphere(Controller.transform.position, _interactableRadius, _interactableMask);

        if (cols.Length <= 0)
        {
            return null;
        }

        return cols[0].TryGetComponent<IInteractable>(out var interactable) ? interactable : null;
    }

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
}
