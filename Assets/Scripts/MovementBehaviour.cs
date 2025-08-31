using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 10.0f, _damping = 4.0f;

    private Rigidbody2D _rigidBody;
    private Vector2 _lastPosition;

    public Vector2 DesiredMovementDirection { get; set; }
    public Vector2 DesiredLookAtPoint { get; set; }
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
        {
            Debug.LogError("Rigidbody2D component is not assigned on " + gameObject.name);
            enabled = false; // Disable script if no Rigidbody2D is found.
            return;
        }
        _lastPosition = _rigidBody.position;
    }

    private void FixedUpdate()
    {
        // Personally a more simple way to move the character, also adds some damping to show smooth acceleration
        // https://www.alexisbacot.com/blog/the-art-of-damping
        _rigidBody.AddForce((DesiredMovementDirection * _movementSpeed - _rigidBody.velocity) / Time.fixedDeltaTime / _damping);
    }

    private void Update()
    {
        IsMoving = _rigidBody.position != _lastPosition;
        _lastPosition = _rigidBody.position;
        HandleMouse();
    }

    private void HandleMouse()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, DesiredLookAtPoint - (Vector2)transform.position);
    }

}
