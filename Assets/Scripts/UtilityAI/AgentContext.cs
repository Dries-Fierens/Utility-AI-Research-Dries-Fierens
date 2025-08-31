using UnityEngine;
using UnityEngine.AI;

public class AgentContext : MonoBehaviour
{
    [Header("Refs")]
    public Transform enemy;
    public NavMeshAgent nav;
    public LayerMask lootMask;       

    [Header("Runtime")]
    public float health = 100f;
    public int ammoInMag = 30;
    public bool hasMedkit = false;
    public float timeSinceLastDamage = -999f;

    [SerializeField] int maxMag = 30;
    [SerializeField] float maxHealth = 100f;

    public float Health01 => Mathf.Clamp01(health / maxHealth);
    public float Ammo01 => Mathf.Clamp01(ammoInMag / (float)maxMag);
    public float DistanceToEnemy => enemy ? Vector3.Distance(transform.position, enemy.position) : 999f;

    public void ConsumeMedkit(float healAmount)
    {
        if (!hasMedkit) return;
        health = Mathf.Min(maxHealth, health + healAmount);
        hasMedkit = false;
    }
    public void RefillMagazine() => ammoInMag = maxMag;
}
