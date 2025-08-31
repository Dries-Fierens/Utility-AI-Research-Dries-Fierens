using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;
    [SerializeField]
    private float _firstWaveStart = 5.0f;
    [SerializeField]
    private float _waveStartFrequency = 15.0f;
    [SerializeField]
    private float _waveEndFrequency = 7.0f;
    [SerializeField]
    private float _waveFrequencyIncrement = 0.5f;

    private float _currentFrequency = 0.0f;

    private void Awake()
    {
        if (_player == null) Debug.LogError("Player GameObject is not assigned in GameMode");

        _currentFrequency = _waveStartFrequency;

        if (SpawnManager.Instance == null)
        {
            Debug.LogError("SpawnManager instance is not found. Ensure it is set up in the scene.");
            return;
        }

        Invoke("StartNewWave", _firstWaveStart);
    }

    // To spawn enemies at a set interval
    private void StartNewWave()
    {
        if (SpawnManager.Instance != null) SpawnManager.Instance.SpawnWave();

        _currentFrequency = Mathf.Clamp(_currentFrequency - _waveFrequencyIncrement, _waveEndFrequency, _waveStartFrequency);

        // Recursively invoke StartNewWave
        Invoke("StartNewWave", _currentFrequency);
    }

    private void Update()
    {
        if (_player == null) TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        SceneManager.LoadScene(0);
    }
}
