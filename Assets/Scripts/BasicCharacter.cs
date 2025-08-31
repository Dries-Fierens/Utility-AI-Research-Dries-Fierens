using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    protected MovementBehaviour _movementBehaviour;

    protected virtual void Awake()
    {
        _movementBehaviour = GetComponent<MovementBehaviour>();
        if (_movementBehaviour == null)
        {
            Debug.LogError("MovementBehaviour component is missing on " + gameObject.name);
        }
    }
}
