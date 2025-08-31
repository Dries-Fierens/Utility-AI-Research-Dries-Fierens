using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKamikazeCharacter : MonoBehaviour
{
    [SerializeField]
    bool _useUtilityAI = true;
    [SerializeField]
    private int _damage = 4;
    [SerializeField]
    private GameObject _target = null;
    [SerializeField]
    private AudioClip _screamSound, _explodeSound, _damageSound;
    [SerializeField, Range(0f, 1f)]
    private float _screamSoundVolume = 0.5f, _explodeSoundVolume = 0.5f, _damageSoundVolume = 0.5f;
    [SerializeField]
    private ParticleSystem _explosionEffect;

    private Health _health;
    private NavMeshAgent _agent;
    private SoundManager _soundManager;
    private Renderer _renderer;
    private const string TAG = "Friendly";
    private bool _enabled = false;
    private bool _screamPlayed = false;
    public bool IsInLight { get; set; } = false;
    public bool IsVisible { get; set; } = false;

    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer component not found on " + gameObject.name);
            return;
        }

        if (_target != null)
        {
            _health = _target.GetComponent<Health>();
            if (_health == null) Debug.LogError("Health component not found on target " + _target.name);
        }
        else Debug.LogWarning("Target is not assigned.");

        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
            enabled = false; // Disable the script to prevent further errors
            return;
        }

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _soundManager = gameObject.AddComponent<SoundManager>();
    }

    private void Update()
    {
        _renderer.enabled = IsVisible;

        if (!_useUtilityAI)
        {
            // Enemy starts moving when lit
            if (IsInLight && _target != null)
            {
                _agent.SetDestination(_target.transform.position);
                _enabled = true;
            }

            // After 10 seconds the enemy explodes after she screams only once
            if (_enabled && !_screamPlayed)
            {
                _soundManager.PlaySoundWhenDestroy(_screamSound, _screamSoundVolume);
                _screamPlayed = true;
                Invoke("Explode", 10f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Explodes and does damage when enemy collides
        if (collision.gameObject.CompareTag(TAG))
        {
            _soundManager.PlaySoundWhenDestroy(_damageSound, _damageSoundVolume);

            if (_health != null) _health.Damage(_damage);
            else Debug.LogWarning("Health is not assigned or missing.");

            Explode();
        }
    }

    public bool HasExploded { get; private set; } = false;
    public event System.Action OnExploded;

    public void ArmExplosion(float delaySec)
    {
        if (!_screamPlayed)
        {
            _soundManager.PlaySoundWhenDestroy(_screamSound, _screamSoundVolume);
            _screamPlayed = true;
        }
        CancelInvoke(nameof(Explode));
        Invoke(nameof(Explode), delaySec);
    }

    public void CancelExplosion() => CancelInvoke(nameof(Explode));

    public void Explode()
    {
        if (HasExploded) return;
        HasExploded = true;
        OnExploded?.Invoke();
        _soundManager.PlaySoundWhenDestroy(_explodeSound, _explodeSoundVolume);
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Initialize objects that will spawn
    public void Initialize(GameObject player, Health health)
    {
        _target = player;
        _health = health;
    }

    public int GetDamage()
    {
        return _damage;
    }
}
