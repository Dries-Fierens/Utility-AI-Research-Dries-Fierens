public interface IActionExecutor
{
    void Begin(EnemyContext ctx);
    void Tick(EnemyContext ctx, float dt);
    bool ShouldFinish(EnemyContext ctx);
    void End(EnemyContext ctx);
}
