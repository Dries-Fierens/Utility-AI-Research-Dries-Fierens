using Assets.Scripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fuel : MonoBehaviour
{
    [SerializeField]
    private int _increase = 10;
    [SerializeField]
    private int _healing = 1;
    [SerializeField]
    private Light2D _light2D;
    [SerializeField]
    private Health _health;
    [SerializeField]
    private AudioClip _pickUpSound;
    [SerializeField, Range(0f, 1f)]
    private float _soundVolume = 0.5f;

    private SoundManager _soundManager;

    const string TAG = "Friendly";

    private void Start()
    {
        _soundManager = gameObject.AddComponent<SoundManager>();

        if (_light2D == null) Debug.LogError("Light2D component is not assigned to Fuel on " + gameObject.name);
        if (_health == null) Debug.LogError("Health component is not assigned to Fuel on " + gameObject.name);
    }

    // Increase the flashlight capacity, heals for a little and gets destroyed because its gets picked up
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TAG))
        {
            _soundManager.PlaySoundWhenDestroy(_pickUpSound, _soundVolume);

            if (_light2D != null) _light2D.pointLightOuterRadius += _increase;
            else Debug.LogWarning("Light2D component is null");

            if (_health != null) _health.Heal(_healing);
            else Debug.LogWarning("Health component is null");

            Destroy(gameObject);
        }
    }

    // Initialize objects that will spawn
    public void Initialize(Light2D light2D, Health health)
    {
        _light2D = light2D;
        _health = health;
    }
}
