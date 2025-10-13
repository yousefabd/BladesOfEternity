using UnityEngine;
[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    public Transform prefab;
    public float speed;
    public int damage;
}
