using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public float gravity;

    public float jumpPower;

    public float rotationSpeed;

    public float walkSpeed;
}