using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts
{
    public class Vision : MonoBehaviour
    {
        private Light2D _light2D;
        private const string LIT_TAG = "Enemy";

        private void Start()
        {
            _light2D = GetComponent<Light2D>();
            if (_light2D == null)
            {
                Debug.LogError("Light2D component is missing on " + gameObject.name);
                enabled = false; // Disable script to avoid further issues
                return;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(LIT_TAG))
            {
                EnemyCharacter enemy = other.GetComponent<EnemyCharacter>();
                if (enemy != null) enemy.IsVisible = true;

                EnemyKamikazeCharacter kamikazeEnemy = other.GetComponent<EnemyKamikazeCharacter>();
                if (kamikazeEnemy != null) kamikazeEnemy.IsVisible = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(LIT_TAG))
            {
                EnemyCharacter enemy = other.GetComponent<EnemyCharacter>();
                if (enemy != null) enemy.IsVisible = false;
            }
        }
    }
}
