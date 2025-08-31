using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Action")]
public class UtilityAction : ScriptableObject
{
    public string actionName;
    public float actionWeight = 1f;
    public float cooldown = 0.2f;
    public Scorer[] scorers;
    public ActionExecutor executor;

    float _cooldownTimer;

    public float Score(EnemyContext ctx)
    {
        if (_cooldownTimer > 0f) return 0f;
        float sum = 0f;
        foreach (var s in scorers) sum += Mathf.Clamp01(s.Score(ctx));
        return Mathf.Clamp01(sum) * actionWeight;
    }
    public void UpdateCooldown(float dt) { if (_cooldownTimer > 0f) _cooldownTimer -= dt; }
    public void SetCooldown() { _cooldownTimer = cooldown; }
}

