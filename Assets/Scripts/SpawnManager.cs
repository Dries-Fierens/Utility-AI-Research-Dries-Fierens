using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private Light2D _lightFOV;

    private Health _health;

    #region SINGLETON INSTANCE
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if (_instance == null && !ApplicationQuitting)
            {
                _instance = FindObjectOfType<SpawnManager>();
                if (_instance == null)
                {
                    GameObject newInstance = new GameObject("Singleton_SpawnManager");
                    _instance = newInstance.AddComponent<SpawnManager>();
                }
            }
            return _instance;
        }
    }

    public static bool Exists
    {
        get
        {
            return _instance != null;
        }
    }
    public static bool ApplicationQuitting = false;
    protected virtual void OnApplicationQuit()
    {
        ApplicationQuitting = true;
    }
    #endregion
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null) _instance = this;
        else if (_instance != this) Destroy(gameObject);
    }
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();
    public void RegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        if (!_spawnPoints.Contains(spawnPoint))
            _spawnPoints.Add(spawnPoint);
    }
    public void UnRegisterSpawnPoint(SpawnPoint spawnPoint)
    {
        _spawnPoints.Remove(spawnPoint);
    }

    private void Start()
    {
        if (_player != null) _health = _player.GetComponent<Health>();
        else Debug.LogWarning("No player is not assigned");
    }

    private void Update()
    {
        _spawnPoints.RemoveAll(s => s == null);
    }

    // Before an objects spawns, it has to be initialized
    // Prefabs cannot use objects from the scene, only other prefabs
    // This is to prevent the use of FindAnyObjectByType which is a heavy operation
    public void SpawnWave()
    {
        InitializeWhenRestart();

        if (_player != null && _lightFOV != null && _health != null)
        {
            foreach (SpawnPoint point in _spawnPoints)
            {
                var spawn = point.Spawn();

                if (spawn.TryGetComponent<EnemyCharacter>(out var enemy))
                {
                    enemy.Initialize(_player, _health);
                    var ctx = enemy.GetComponent<EnemyContext>() ?? enemy.gameObject.AddComponent<EnemyContext>();
                    ctx.player = _player.transform;
                }
                else if (spawn.TryGetComponent<Fuel>(out var pickUp))
                {
                    pickUp.Initialize(_lightFOV, _health);
                }
                else if (spawn.TryGetComponent<EnemyKamikazeCharacter>(out var enemyKamikaze))
                {
                    enemyKamikaze.Initialize(_player, _health);
                }
            }
        }
    }

    private void InitializeWhenRestart()
    {
        if (_player == null) _player = FindAnyObjectByType<PlayerCharacter>().gameObject;
        if (_lightFOV == null) _lightFOV = _player.GetComponentInChildren<FlashlightVision>().GetComponent<Light2D>();
        if(_health == null) _health = _player.GetComponent<Health>();
    }

}
