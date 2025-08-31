using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Scorers/NotLit")]
public class NotLitScorer : Scorer
{
    public override float Score(EnemyContext ctx) => ctx.IsLit ? 0f : 1f;
}

