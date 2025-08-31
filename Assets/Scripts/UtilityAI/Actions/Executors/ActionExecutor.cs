using UnityEngine;

public abstract class ActionExecutor : ScriptableObject, IActionExecutor
{
    public abstract void Begin(EnemyContext ctx);
    public abstract void Tick(EnemyContext ctx, float dt);
    public abstract bool ShouldFinish(EnemyContext ctx);
    public abstract void End(EnemyContext ctx);
}
