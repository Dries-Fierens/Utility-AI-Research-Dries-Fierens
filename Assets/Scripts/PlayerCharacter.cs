using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : BasicCharacter
{
    [SerializeField]
    private InputActionAsset _inputAsset;
    [SerializeField]
    private InputActionReference _movement, _mousePosition;

    public int StartProgress { get; set; } = 4;
    public int CurrentProgress { get; set; } = 0;
    public delegate void ProgressChange(float startProgress, float currentProgress);
    public event ProgressChange OnProgressChanged;

    protected override void Awake()
    {
        base.Awake();
        if (_inputAsset == null) return;
    }
    private void OnEnable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Enable();
    }
    private void OnDisable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Disable();
    }
    private void Update()
    {
        HandleMovementInput();
        HandleAimingInput();
    }
    private void HandleMovementInput()
    {
        if (_movementBehaviour == null || _movement == null) return;
        Vector2 movementInput = _movement.action.ReadValue<Vector2>();
        _movementBehaviour.DesiredMovementDirection = movementInput;
    }
    private void HandleAimingInput()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        _movementBehaviour.DesiredLookAtPoint = worldMousePosition;
    }

    public void Progress()
    {
        ++CurrentProgress;
        OnProgressChanged?.Invoke(StartProgress, CurrentProgress);
        if (CurrentProgress >= StartProgress) Destroy(gameObject);
    }
}
