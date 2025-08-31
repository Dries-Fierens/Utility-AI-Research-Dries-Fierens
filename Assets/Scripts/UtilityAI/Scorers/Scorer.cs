using UnityEngine;

public abstract class Scorer : ScriptableObject
{
    [Range(0f, 2f)] public float weight = 1f;
    public AnimationCurve response = AnimationCurve.Linear(0, 0, 1, 1);
    public float Evaluate(float x01) => Mathf.Clamp01(response.Evaluate(Mathf.Clamp01(x01))) * weight;
    public abstract float Score(EnemyContext ctx);
}
