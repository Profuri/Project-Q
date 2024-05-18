using System.Collections.Generic;
using System.Linq;
using InteractableSystem;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    private PlayerUnit _player;
    private List<InteractableObject> _lastFindInteractableObjects;

    private void Awake()
    {
        _player = GetComponent<PlayerUnit>();
        _lastFindInteractableObjects = new List<InteractableObject>();
    }

    public void UpdateHandler()
    {
        FindInteractable();
    }
    
    public void OnInteraction()
    {
        if (_player.HoldingHandler.IsHold)
        {
            _player.HoldingHandler.Detach();
            return;
        }

        var interactable = _lastFindInteractableObjects.FirstOrDefault(
            interactable => interactable.InteractType == EInteractType.INPUT_RECEIVE);

        if (interactable != null)
        {
            interactable.OnInteraction(_player, true);
        }
    }
    
    private void FindInteractable()
    {
        var cols = new Collider[_player.Data.maxInteractableCnt];
        var size = Physics.OverlapSphereNonAlloc(_player.Collider.bounds.center, _player.Data.interactableRadius, cols, _player.Data.interactableMask);

        var tempList = new List<InteractableObject>();
        
        for(var i = 0; i < size; ++i)
        {
            if (cols[i].TryGetComponent<InteractableObject>(out var interactable))
            {
                var dir = (cols[i].bounds.center - _player.Collider.bounds.center).normalized;
                var isHit = Physics.Raycast(_player.Collider.bounds.center - dir, dir, out var hit, Mathf.Infinity,
                    _player.canStandMask);
                
                if ((isHit && cols[i] != hit.collider) || !interactable.activeUnit)
                {
                    continue;
                }

                interactable.OnDetectedEnter(_player);
                tempList.Add(interactable);
            }
        }
        
        foreach (var lastSelected in _lastFindInteractableObjects)
        {
            if (tempList.Count <= 0 || !tempList.Contains(lastSelected))
            {
                lastSelected.OnDetectedLeave(_player);
            }
        }
        
        _lastFindInteractableObjects = tempList;
    }
}
