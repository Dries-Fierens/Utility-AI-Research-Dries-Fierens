using UnityEngine;

// Proximity (closer => higher)
[CreateAssetMenu(menuName = "UtilityAI/Scorers/Proximity")]
public class ProximityScorer : Scorer
{
    public float nearDistance = 3f;
    public float farDistance = 12f;

    public override float Score(EnemyContext ctx)
    {
        float d = ctx.DistanceToPlayer;
        float x = Mathf.InverseLerp(farDistance, nearDistance, d); // near=1, far=0
        return Evaluate(x);
    }
}
