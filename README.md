# Utility AI for Enemies
## Introduction
#### The Project

This project demonstrates a Utility AI decision-making system for enemies in Unity. Instead of using simple state machines, enemies evaluate multiple possible actions (like chasing or idling) based on context (Distance, light exposure, proximity, etc.) and dynamically pick the most appropriate action. Basically the different actions are each given a score to compare which action is the best at the moment.

The system is built around Scorers that evaluate conditions, Executors that execute the action, Utility Actions that combine scorers with executors, and a Utility Brain that continuously selects the best action.

#### Final result
Normal enemy:
- They chase when the player is near.
- They idle when lit by the player.

Kamikaze enemy:
- They chase when the player is near or when lit.
- They idle when not lit by the player.
 
They switch smoothly between behaviors with configurable hysteresis and cooldowns. Hysteresis prevents enemies from rapidly switching back and forth between actions when their scores are very close

![Result](https://github.com/user-attachments/assets/ab94e9c5-5992-4022-937a-f88b1737b2bf)

## Implementation
I was inspired by reading about Utility AI systems in modern games, where enemies make decisions more fluidly compared to rigid state machines. This project shows a basic implementation in Unity.

### Step 1: Context

The EnemyContext gathers important information about the world:
- The nav mesh agent to chase the player for example
- Distance to the player
- Whether the enemy is lit
- Whether the enemy is visible
- The player direction
- Whether a kamikaze has exploded

This provides a clean way for actions and scorers to reason about the current situation.

    public class EnemyContext : MonoBehaviour
    {
        public Transform player;
        public NavMeshAgent agent;
        public EnemyCharacter enemy;
        public EnemyKamikazeCharacter kamikaze;
    
        public float DistanceToPlayer => player ? Vector2.Distance(transform.position, player.position) : 999f;
        public bool IsLit => (enemy && enemy.IsInLight) || (kamikaze && kamikaze.IsInLight);
        public bool IsVisible => (enemy && enemy.IsVisible) || (kamikaze && kamikaze.IsVisible);
        public Vector2 ToPlayerDir => player ? ((Vector2)(player.position - transform.position)).normalized : Vector2.right;
        public bool DidExplode => kamikaze && kamikaze.HasExploded;
    
        void Awake()
        {
            if (!agent) agent = GetComponent<NavMeshAgent>();
            if (!enemy) enemy = GetComponent<EnemyCharacter>();
            if (!kamikaze) kamikaze = GetComponent<EnemyKamikazeCharacter>();
    
            if (!player || !player.gameObject.scene.IsValid())
            {
                // there are only ever supposed to be one player
                var pc = FindAnyObjectByType<PlayerCharacter>(FindObjectsInactive.Exclude);
                if (pc) player = pc.transform;
            }
        }
    }

### Step 2: Scorers

Scorers evaluate a single aspect of the context and output a 0–1 score.

Examples:

    // LitScorer → returns 1 if enemy is lit, else 0
    public override float Score(EnemyContext ctx) => ctx.IsLit ? 1f : 0f;
    
    // ProximityScorer → closer = higher score
    public float nearDistance = 3f;
    public float farDistance = 12f;
    public override float Score(EnemyContext ctx)
    {
        float d = ctx.DistanceToPlayer;
        float x = Mathf.InverseLerp(farDistance, nearDistance, d); // near=1, far=0
        return Evaluate(x);
    }

Scorers can be combined and weighted, with response curves for fine-tuning.

    public abstract class Scorer : ScriptableObject
    {
        [Range(0f, 2f)] public float weight = 1f;
        public AnimationCurve response = AnimationCurve.Linear(0, 0, 1, 1);
        public float Evaluate(float x01) => Mathf.Clamp01(response.Evaluate(Mathf.Clamp01(x01))) * weight;
        public abstract float Score(EnemyContext ctx);
    }

### Step 3: Utility Actions

Each UtilityAction bundles:

- An ActionExecutor (what to do)
- A set of Scorers (when to do it)
- A weight and cooldown system

The action calculates its total score by summing scorers, applying weights, and ignoring itself if still on cooldown.

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

### Step 4: Executors
Executors implement the behavior logic:

Begin → setup before running

Tick → update each frame

ShouldFinish → condition to stop

End → cleanup when finished

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

### Step 5: Utility Brain

The UtilityBrain coordinates everything:

Periodically evaluates all actions.

Chooses the best scoring action.

Applies hysteresis to prevent rapid switching.

Handles cooldowns and action transitions.

    void Update()
    {
        // Advance time & cooldowns
        float dt = Time.deltaTime; 
        timer += dt;
        foreach (var a in actions) a.UpdateCooldown(dt);

        // Limiting expensive decision-making to a set frequency
        if (timer < 1f / Mathf.Max(1f, updateHz)) 
        {
            CurrentAction?.executor.Tick(ctx, dt); 
            return; 
        }
        timer = 0f;

        // Score every action & build debug data
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

        // Hysteresis check
        float curScore = CurrentAction ? CurrentAction.Score(ctx) : -1f;
        bool switchNow = best && (CurrentAction != best) && (bestScore > curScore + hysteresis);

        // Switch if needed
        if (switchNow) SwitchTo(best);

        // Run the chosen behavior
        CurrentAction?.executor.Tick(ctx, dt);

        // Check if should finish
        if (CurrentAction != null && CurrentAction.executor.ShouldFinish(ctx))
        {
            CurrentAction.executor.End(ctx);
            CurrentAction.SetCooldown();
            CurrentAction = null;
        }
    }

    // Switch the current action to the next best action
    void SwitchTo(UtilityAction next)
    {
        CurrentAction?.executor.End(ctx);
        CurrentAction = next;
        CurrentAction?.executor.Begin(ctx);
        OnActionChanged?.Invoke(CurrentAction);
    }

### Visualization

The brain stores debug scores for each action every tick, so you can display them in a HUD or editor to see why an action was chosen.

<img width="544" height="232" alt="Visualization" src="https://github.com/user-attachments/assets/b704f474-c903-4ce0-8cd3-d092dfe42957" />


## Conclusion

This project shows how Utility AI can make enemies feel more intelligent and less predictable. Instead of hard-coded states, actions compete dynamically based on context.

Potential improvements:
- Use utility AI on the player
- Implement combo actions (e.g., chase then attack).

## References
https://medium.com/@morganwalkupdev/ai-made-easy-with-utility-ai-fef94cd36161

https://www.youtube.com/watch?v=d63hbJYYqM8&ab_channel=ThisIsVini

https://www.youtube.com/watch?v=ejKrvhusU1I&list=PLDpv2FF85TOp2KpIGcrxXY1POzfYLGWIb&index=1&ab_channel=TooLoo
