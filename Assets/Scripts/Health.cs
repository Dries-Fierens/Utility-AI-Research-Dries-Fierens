using UnityEngine;

// To interact with an object by healing or damaging it
public class Health : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _bloodEffect;
    [SerializeField]
    private int _startHealth;
    private int _currentHealth = 0;
    public float StartHealth { get { return _startHealth; } }
    public float CurrentHealth { get { return _currentHealth; } }
    public delegate void HealthChange(float startHealth, float currentHealth);
    public event HealthChange OnHealthChanged;


    private void Awake()
    {
        _currentHealth = _startHealth;
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        if (_currentHealth >= _startHealth) _currentHealth = _startHealth;
        OnHealthChanged?.Invoke(_startHealth, _currentHealth);
    }

    public void Damage(int amount)
    {
        Instantiate(_bloodEffect, transform.position, Quaternion.identity);
        _currentHealth -= amount;
        OnHealthChanged?.Invoke(_startHealth, _currentHealth);
        if (_currentHealth <= 0) Destroy(gameObject);
    }
}
