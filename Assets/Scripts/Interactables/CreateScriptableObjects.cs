// Mirza Wilayat Ali (1034162)

using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Game/Interactable Object Data")]
public class InteractableData : ScriptableObject
{
    public GameObject prefab;
    public float spawnY;
    public bool isPowerUp;
    public string powerUpEffect;
}
