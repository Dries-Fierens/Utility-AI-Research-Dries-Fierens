using UnityEngine;
using UnityEngine.AI;

public class EnemyContext : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public EnemyCharacter enemy;
    public EnemyKamikazeCharacter kamikaze;

    public float DistanceToPlayer => player ? Vector2.Distance(transform.position, player.position) : 999f;
    public bool IsLit => (enemy && enemy.IsInLight) || (kamikaze && kamikaze.IsInLight);
    public bool IsVisible => (enemy && enemy.IsVisible) || (kamikaze && kamikaze.IsVisible);
    public Vector2 ToPlayerDir => player ? ((Vector2)(player.position - transform.position)).normalized : Vector2.right;
    public bool DidExplode => kamikaze && kamikaze.HasExploded;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!enemy) enemy = GetComponent<EnemyCharacter>();
        if (!kamikaze) kamikaze = GetComponent<EnemyKamikazeCharacter>();

        if (!player || !player.gameObject.scene.IsValid())
        {
            // there are only ever supposed to be one player
            var pc = FindAnyObjectByType<PlayerCharacter>(FindObjectsInactive.Exclude);
            if (pc) player = pc.transform;
        }
    }
}
