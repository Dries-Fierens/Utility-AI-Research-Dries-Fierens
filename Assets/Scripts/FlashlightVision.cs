using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightVision : MonoBehaviour
{
    [SerializeField]
    private float flashLightDuration = 2f; // Duration for light reduction over time

    private MovementBehaviour _movementBehaviour;
    private Light2D _light2D;
    private float _lightRange;
    private PolygonCollider2D _visionCollider;

    private const int VERTICES = 5;
    private const string LIT_TAG = "Enemy";
    private const float MAX_RADIUS = 25;

    private void Start()
    {
        _light2D = GetComponent<Light2D>();
        if (_light2D == null)
        {
            Debug.LogError("Light2D component is missing on " + gameObject.name);
            enabled = false; // Disable script to avoid further issues
            return;
        }

        _visionCollider = GetComponent<PolygonCollider2D>();
        if (_visionCollider == null) _visionCollider = gameObject.AddComponent<PolygonCollider2D>();
        _visionCollider.isTrigger = true;

        _movementBehaviour = GetComponentInParent<MovementBehaviour>();
        if (_movementBehaviour == null) Debug.LogWarning("MovementBehaviour component is missing on the parent of " + gameObject.name);

        _lightRange = _light2D.pointLightOuterRadius;
    }

    private void Update()
    {
        if (_movementBehaviour != null && _movementBehaviour.IsMoving)
        {
            _light2D.pointLightOuterRadius = Mathf.Max(0, _light2D.pointLightOuterRadius - Time.deltaTime / flashLightDuration);
        }

        _light2D.pointLightOuterRadius = Mathf.Clamp(_light2D.pointLightOuterRadius, 0, MAX_RADIUS);
        _lightRange = _light2D.pointLightOuterRadius;

        UpdateVisionCone();
    }

    private void UpdateVisionCone()
    {
        List<Vector2> points = new List<Vector2> { Vector2.zero };
        float outerAngle = _light2D.pointLightOuterAngle;
        float angleStep = outerAngle / VERTICES;

        for (int i = 0; i <= VERTICES; i++)
        {
            float currentAngle = outerAngle + (angleStep * i);
            Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
            points.Add(direction * _lightRange);
        }

        _visionCollider.SetPath(0, points.ToArray());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(LIT_TAG))
        {
            EnemyCharacter enemy = other.GetComponent<EnemyCharacter>();
            if (enemy != null)
            {
                enemy.IsInLight = true;
                enemy.IsVisible = true;
            }

            EnemyKamikazeCharacter kamikazeEnemy = other.GetComponent<EnemyKamikazeCharacter>();
            if (kamikazeEnemy != null)
            {
                kamikazeEnemy.IsInLight = true;
                kamikazeEnemy.IsVisible = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(LIT_TAG))
        {
            EnemyCharacter enemy = other.GetComponent<EnemyCharacter>();
            if (enemy != null)
            {
                enemy.IsInLight = false;
                enemy.IsVisible = false;
            }
        }

        EnemyKamikazeCharacter kamikazeEnemy = other.GetComponent<EnemyKamikazeCharacter>();
        if (kamikazeEnemy != null) kamikazeEnemy.IsVisible = false;
    }
}
