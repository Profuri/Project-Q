using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float gravity;

    public float jumpPower;

    [Range(0.1f, 1f)] public float rotationSpeed;

    public float walkSpeed;
    
    public LayerMask groundMask;
    public float groundCheckDistance;
}