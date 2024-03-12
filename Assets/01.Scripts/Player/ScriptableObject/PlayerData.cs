using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Data")]
    public float walkSpeed;
    public float jumpPower;
    [Range(0.1f, 1f)] public float rotationSpeed;
    
    [Header("Ground Check Data")]
    public LayerMask groundMask;
    public float groundCheckDistance;
    
    [Header("Interactable Data")]
    public LayerMask interactableMask;
    public float interactableRadius;
    public int maxInteractableCnt;

    [Header("Holding Data")] 
    public float holdingRadius;
    [Range(0.1f, 1f)] public float holdingPointMoveSpeed;
}