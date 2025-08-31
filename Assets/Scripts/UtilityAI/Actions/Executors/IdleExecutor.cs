using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "UtilityAI/Executors/Idle")]
public class IdleExecutor : ActionExecutor
{
    public override void Begin(EnemyContext ctx)
    {
        if (!ctx.agent) return;
        ctx.agent.ResetPath();
        ctx.agent.isStopped = true;
        ctx.agent.velocity = Vector3.zero;
        ctx.agent.nextPosition = ctx.transform.position;
    }
    public override void Tick(EnemyContext ctx, float dt)
    {
        if (!ctx.agent) return;
        ctx.agent.isStopped = true;
        ctx.agent.velocity = Vector3.zero;
        ctx.agent.nextPosition = ctx.transform.position;
    }
    public override bool ShouldFinish(EnemyContext ctx) => !ctx.IsLit;
    public override void End(EnemyContext ctx)
    {
        if (!ctx.agent) return;
        ctx.agent.isStopped = false;
        ctx.agent.velocity = Vector3.zero;
    }
}

