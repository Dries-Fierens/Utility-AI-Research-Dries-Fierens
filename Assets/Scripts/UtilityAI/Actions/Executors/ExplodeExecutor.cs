using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "UtilityAI/Executors/Explode")]
public class ExplodeExecutor : ActionExecutor
{
    EnemyKamikazeCharacter _kamikaze;
    Health _health;
    public override void Begin(EnemyContext ctx)
    {
        _kamikaze = ctx.kamikaze;
        _health = ctx.player.GetComponent<Health>();
    }
    public override void Tick(EnemyContext ctx, float dt) {}
    public override bool ShouldFinish(EnemyContext ctx) => true; // Finishes immediately
    public override void End(EnemyContext ctx) 
    {
        if (_kamikaze && _health != null)
        {
            _health.Damage(_kamikaze.GetDamage());
            _kamikaze.Explode();
        }
    }
}
