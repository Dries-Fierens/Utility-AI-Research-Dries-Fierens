using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    private UIDocument _attachedDocument = null;
    private VisualElement _root = null;
    private ProgressBar _healthBar = null;
    private ProgressBar _progressBar = null;
    private Label _tutorialText = null;

    // --- AI DEBUG ---
    private VisualElement _aiPanel;
    private Label _aiHeader;
    private VisualElement _aiBarsContainer;
    private readonly Dictionary<string, ProgressBar> _barsByAction = new();
    private PlayerCharacter _player;
    private UtilityBrain _trackedBrain;

    private void Start()
    {
        _attachedDocument = GetComponent<UIDocument>();
        if (_attachedDocument) _root = _attachedDocument.rootVisualElement;

        if (_root != null)
        {
            _healthBar = _root.Q<ProgressBar>("healthBar");
            _progressBar = _root.Q<ProgressBar>("progressBar");
            _tutorialText = _root.Q<Label>("tutorialText");

            _player = FindObjectOfType<PlayerCharacter>();
            if (_player != null)
            {
                Health playerHealth = _player.GetComponent<Health>();
                if (playerHealth)
                {
                    UpdateHealth(playerHealth.StartHealth, playerHealth.CurrentHealth);
                    playerHealth.OnHealthChanged += UpdateHealth;
                }
                UpdateProgress(_player.StartProgress, _player.CurrentProgress);
                _player.OnProgressChanged += UpdateProgress;
            }

            BuildAIDebugPanel();
            StartCoroutine(UpdateAIDebugLoop());
        }

        StartCoroutine(UpdateTutorialText());
    }

    // Update healthbar when healed or damaged
    public void UpdateHealth(float startHealth, float currentHealth)
    {
        if (_healthBar == null) return;
        _healthBar.value = currentHealth;
        _healthBar.title = string.Format("{0}/{1}", currentHealth, startHealth);
    }

    // Update progress when a page gets pickup
    public void UpdateProgress(float startProgress, float currentProgress)
    {
        if (_progressBar == null) return;
        _progressBar.value = currentProgress;
        _progressBar.title = string.Format("{0}/{1}", currentProgress, startProgress);
    }

    // Player feedback at the beginning of the game
    private IEnumerator UpdateTutorialText()
    {
        const float DELAY = 5f;
        if (_tutorialText == null) yield break;

        yield return new WaitForSeconds(DELAY);

        _tutorialText.text = "Collect 4 drawings that are found around the map";
        yield return new WaitForSeconds(DELAY);

        _tutorialText.text = "Use the light for protection, collect fuel to replenish it";
        yield return new WaitForSeconds(DELAY);

        _tutorialText.text = "";
    }

    private void GameOver()
    {
        _tutorialText.text = "GAME OVER";
    }

    void BuildAIDebugPanel()
    {
        _aiPanel = new VisualElement();
        _aiPanel.style.position = Position.Absolute;
        _aiPanel.style.top = 10;
        _aiPanel.style.right = 10;
        _aiPanel.style.width = 260;
        _aiPanel.style.backgroundColor = new Color(0f, 0f, 0f, 0.55f);
        _aiPanel.style.paddingTop = 8; 
        _aiPanel.style.paddingBottom = 8;
        _aiPanel.style.paddingLeft = 10; 
        _aiPanel.style.paddingRight = 10;
        _aiPanel.style.borderTopLeftRadius = 6; 
        _aiPanel.style.borderBottomLeftRadius = 6;
        _aiPanel.style.borderTopRightRadius = 6; 
        _aiPanel.style.borderBottomRightRadius = 6;

        _aiHeader = new Label("AI: (none)");
        _aiHeader.style.color = new Color(1f, 1f, 1f, 1f);
        _aiHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        _aiHeader.style.marginBottom = 6;
        _aiHeader.style.unityTextAlign = TextAnchor.MiddleCenter;

        _aiBarsContainer = new VisualElement();
        _aiBarsContainer.style.flexDirection = FlexDirection.Column;
        _aiBarsContainer.style.marginBottom = 4;
        _aiBarsContainer.style.marginTop = 4;
        _aiBarsContainer.style.marginLeft = 0;
        _aiBarsContainer.style.marginRight = 0;

        _aiPanel.Add(_aiHeader);
        _aiPanel.Add(_aiBarsContainer);
        _root.Add(_aiPanel);
    }

    IEnumerator UpdateAIDebugLoop()
    {
        var brains = new List<UtilityBrain>();
        var wait = new WaitForSeconds(0.2f);

        while (true)
        {
            brains.Clear();
            brains.AddRange(FindObjectsOfType<UtilityBrain>());
            if (_player == null || brains.Count == 0)
            {
                _aiHeader.text = "AI: (none)";
                _aiBarsContainer.Clear();
                _barsByAction.Clear();
                yield return wait;
                continue;
            }

            // Pick the closest brain to the player to show
            UtilityBrain closest = null;
            float best = float.MaxValue;
            Vector3 p = _player.transform.position;
            foreach (var b in brains)
            {
                float d = (b.transform.position - p).sqrMagnitude;
                if (d < best) { best = d; closest = b; }
            }

            if (_trackedBrain != closest)
            {
                _trackedBrain = closest;
                _barsByAction.Clear();
                _aiBarsContainer.Clear();
            }

            string cur = _trackedBrain?.CurrentAction ? _trackedBrain.CurrentAction.name : "(idle)";
            _aiHeader.text = $"AI: {_trackedBrain.gameObject.name} — {cur}";

            // Draw top 4 scores
            if (_trackedBrain != null && _trackedBrain.DebugScores != null)
            {
                foreach (var item in _trackedBrain.DebugScores
                         .OrderByDescending(s => s.score01)
                         .Take(4))
                {
                    if (!_barsByAction.TryGetValue(item.action, out var bar))
                    {
                        bar = new ProgressBar();
                        bar.lowValue = 0f; 
                        bar.highValue = 1f;
                        bar.style.height = 16;
                        bar.style.unityTextAlign = TextAnchor.MiddleLeft;
                        _aiBarsContainer.Add(bar);
                        _barsByAction[item.action] = bar;
                    }
                    bar.title = $"{item.action}  {(item.score01 * 100f):0}%";
                    bar.value = item.score01;
                }

                var keys = _barsByAction.Keys.ToList();
                foreach (var key in keys)
                {
                    if (!_trackedBrain.DebugScores.Any(ds => ds.action == key))
                    {
                        _aiBarsContainer.Remove(_barsByAction[key]);
                        _barsByAction.Remove(key);
                    }
                }
            }

            yield return wait;
        }
    }
}
