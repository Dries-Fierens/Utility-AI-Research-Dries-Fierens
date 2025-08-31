using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawnTemplate = null;

    [SerializeField]
    private int _spawnAreaWidthMax = 81;
    [SerializeField]
    private int _spawnAreaHeightMax = 81;
    [SerializeField]
    private int _spawnAreaWidthMin = -81;
    [SerializeField]
    private int _spawnAreaHeightMin = -81;

    private const float SPAWN_COLLISION_RADIUS = 1f;

    private void OnEnable()
    {
        SpawnManager.Instance.RegisterSpawnPoint(this);
    }
    private void OnDisable()
    {
        if (SpawnManager.Exists) SpawnManager.Instance.UnRegisterSpawnPoint(this);
    }

    // Spawn objects at random location and makes sure that doesn't overlap with another object
    public GameObject Spawn()
    {
        int attempts = 0;
        const int maxAttempts = 5;
        GameObject newObject = null;

        while (attempts < maxAttempts)
        {
            attempts++;

            int x = Random.Range(_spawnAreaWidthMin, _spawnAreaWidthMax);
            int y = Random.Range(_spawnAreaHeightMin, _spawnAreaHeightMax);
            Vector2 randomOffset = new(x, y);

            Collider2D hitCollider = Physics2D.OverlapCircle(randomOffset, SPAWN_COLLISION_RADIUS);

            if (hitCollider == null)
            {
                newObject = Instantiate(_spawnTemplate, randomOffset, _spawnTemplate.transform.rotation);
                break;
            }
        }

        if (newObject == null) Debug.LogWarning("Failed to find a suitable spawn location after multiple attempts.");

        return newObject;
    }

}
