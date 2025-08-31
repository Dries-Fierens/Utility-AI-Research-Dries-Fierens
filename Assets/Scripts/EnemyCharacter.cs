using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : MonoBehaviour
{
    [SerializeField] 
    bool _useUtilityAI = true;
    [SerializeField]
    private int _damage = 2;
    [SerializeField]
    private GameObject _target = null;
    [SerializeField]
    private AudioClip _damageSound, _biteSound;
    [SerializeField, Range(0f, 1f)]
    private float _damageSoundVolume, _biteSoundVolume = 0.5f;

    private Health _health;
    private NavMeshAgent _agent;
    private SoundManager _soundManager;
    private Renderer _renderer;
    private const string TAG = "Friendly";
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
            if (_health == null)
            {
                Debug.LogError("Health component not found on target " + _target.name);
            }
        }
        else
        {
            Debug.LogWarning("Target is not assigned");
        }

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

        // When UtilityAI is enabled, movement is driven by the Brain/Executors.
        if (!_useUtilityAI)
        {
            if (!IsInLight && _target != null) _agent.SetDestination(_target.transform.position);
            else _agent.SetDestination(transform.position);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TAG))
        {
            _soundManager.PlaySoundWhenDestroy(_biteSound, _biteSoundVolume);
            _soundManager.PlaySoundWhenDestroy(_damageSound, _damageSoundVolume);

            if (_health != null) _health.Damage(_damage);
            else Debug.LogWarning("Health is not assigned or missing.");

            Destroy(gameObject);
        }
    }

    // Initialize objects that will spawn
    public void Initialize(GameObject player, Health health)
    {
        _target = player;
        _health = health;
    }
}
