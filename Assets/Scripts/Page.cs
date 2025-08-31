using Assets.Scripts;
using UnityEngine;

public class Page : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter _player = null;
    [SerializeField]
    private int _healing = 5;
    [SerializeField]
    private AudioClip _paperSound;
    [SerializeField, Range(0f, 1f)]
    private float _soundVolume = 0.5f;

    private Health _health;
    private SoundManager _soundManager;

    const string TAG = "Friendly";

    private void Start()
    {
        if(_player != null) _health = _player.GetComponent<Health>();
        else Debug.LogWarning("Player component is null");
        _soundManager = gameObject.AddComponent<SoundManager>();
    }

    // Update progress and destroy it after its pickup
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TAG))
        {
            _soundManager.PlaySoundWhenDestroy(_paperSound, _soundVolume);

            if (_player != null) _player.Progress();
            else Debug.LogWarning("Light2D component is null");

            if (_health != null) _health.Heal(_healing);
            else Debug.LogWarning("Health component is null");

            Destroy(gameObject);
        }
    }
}
