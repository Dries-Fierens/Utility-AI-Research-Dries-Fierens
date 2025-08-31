using System.Collections.Generic;
using UnityEngine;

public class UtilityBrain : MonoBehaviour
{
    public EnemyContext ctx;
    public UtilityAction[] actions;
    [Range(0f, 1f)] public float hysteresis = 0.1f;
    public float updateHz = 8f;

    [System.Serializable]
    public struct DebugScore { public string action; public float score01; } // 0..1

    public UtilityAction CurrentAction { get; private set; }      // expose current selection
    public List<DebugScore> DebugScores { get; private set; } = new(); // filled every evaluation
    public event System.Action<UtilityAction> OnActionChanged;    // optional: HUD can subscribe

    float timer;

    void Awake() { if (!ctx) ctx = GetComponent<EnemyContext>(); }

    void Update()
    {
        float dt = Time.deltaTime; 
        timer += dt;
        foreach (var a in actions) a.UpdateCooldown(dt);

        // limiting expensive decision-making to a set frequency
        if (timer < 1f / Mathf.Max(1f, updateHz)) 
        {
            CurrentAction?.executor.Tick(ctx, dt); 
            return; 
        }
        timer = 0f;

        DebugScores.Clear();
        UtilityAction best = null; 
        float bestScore = -1f;
        foreach (var a in actions) 
        { 
            float s = a.Score(ctx);
            DebugScores.Add(new DebugScore { action = a.name, score01 = Mathf.Clamp01(s) });
            if (s > bestScore) 
            { 
                best = a;
                bestScore = s;
            } 
        }

        float curScore = CurrentAction ? CurrentAction.Score(ctx) : -1f;
        bool switchNow = best && (CurrentAction != best) && (bestScore > curScore + hysteresis);
        if (switchNow) SwitchTo(best);

        CurrentAction?.executor.Tick(ctx, dt);

        if (CurrentAction != null && CurrentAction.executor.ShouldFinish(ctx))
        {
            CurrentAction.executor.End(ctx);
            CurrentAction.SetCooldown();
            CurrentAction = null;
        }
    }

    void SwitchTo(UtilityAction next)
    {
        Debug.Log($"[UtilityBrain] {name}: current: {CurrentAction?.actionName ?? "<none>"} -> next: {next?.actionName ?? "<none>"}");
        CurrentAction?.executor.End(ctx);
        CurrentAction = next;
        CurrentAction?.executor.Begin(ctx);
        OnActionChanged?.Invoke(CurrentAction);
    }

    UtilityAction FindAction(string name)
    {
        foreach (var a in actions) if (a && a.actionName == name) return a;
        return null;
    }
}
