using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Scorers/Lit")]
public class LitScorer : Scorer
{
    public override float Score(EnemyContext ctx) => ctx.IsLit ? 1f : 0f;
}
