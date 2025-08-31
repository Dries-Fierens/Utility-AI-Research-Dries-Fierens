using System;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Executors/Chase")]
public class ChaseExecutor : ActionExecutor
{
    public override void Begin(EnemyContext ctx)
    {
        if (!ctx.agent) return;
        ctx.agent.isStopped = false;
        ctx.agent.ResetPath();
    }

    public override void Tick(EnemyContext ctx, float dt)
    {
        if (!ctx.agent || !ctx.player) return;

        Vector3 playerPos = ctx.player.position;
        if ((ctx.agent.destination - playerPos).sqrMagnitude > 0.01f)
            ctx.agent.SetDestination(playerPos);
    }

    public override bool ShouldFinish(EnemyContext ctx) => ctx.IsLit;

    public override void End(EnemyContext ctx)
    {
        if (!ctx.agent) return;
        ctx.agent.velocity = Vector3.zero;
    }
}


