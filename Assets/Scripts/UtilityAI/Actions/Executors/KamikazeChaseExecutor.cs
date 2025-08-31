using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Executors/KamikazeChase")]
public class KamikazeChaseExecutor : ActionExecutor
{
    [Min(0f)] public float explodeDelay = 10f;

    public override void Begin(EnemyContext ctx)
    {
        if (ctx.agent) 
        { 
            ctx.agent.isStopped = false; 
            ctx.agent.ResetPath(); 
        }
        if (ctx.kamikaze) ctx.kamikaze.ArmExplosion(explodeDelay);
    }

    float logTimer;
    public override void Tick(EnemyContext ctx, float dt)
    {
        if (!ctx.agent || !ctx.player) return;
        ctx.agent.isStopped = false;
        ctx.agent.SetDestination(ctx.player.position);
    }

    public override bool ShouldFinish(EnemyContext ctx)
    {
        if (ctx.DidExplode) return true;
        if (!ctx.player) return true;
        return false;
    }

    public override void End(EnemyContext ctx)
    {
        if (ctx.kamikaze && !ctx.DidExplode) ctx.kamikaze.CancelExplosion();
        if (ctx.agent) 
        { 
            ctx.agent.ResetPath(); 
            ctx.agent.velocity = Vector3.zero; 
        }
    }
}
