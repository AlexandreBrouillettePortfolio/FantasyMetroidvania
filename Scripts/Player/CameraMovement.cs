using Cinemachine;
using UnityEngine;

/// <summary>
/// Provides camera movement based on the input.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// The delay in seconds before the camera starts panning down.
    /// </summary>
    [Range(0.0f, 5.0f)]
    public float timeBeforeLookingDown = 2.0f;

    /// <summary>
    /// The maximum screen Y delta that the camera can reach.
    /// </summary>
    [Range(-0.5f, 0.0f)]
    public float maximumScreenYDelta = -0.3f;

    /// <summary>
    /// The speed at which the camera pans down.
    /// </summary>
    public float panningSpeed = 0.5f;

    private bool _crouchIsPressed = false;
    private float _timeAtCrouch = 0.0f;

    private bool _isPanning = false;
    private float _originalScreenY = 0.0f;
    private CinemachineFramingTransposer _framingTransposer;

    private void Start()
    {
        if (!TryGetComponent<CinemachineVirtualCamera>(out var virtualCamera))
        {
            Debug.LogError("No CinemachineVirtualCamera component found on the GameObject.");
            return;
        }

        _framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown(Buttons.Crouch))
        {
            _crouchIsPressed = true;
            _timeAtCrouch = Time.time;
        }
        else if (Input.GetButtonUp(Buttons.Crouch))
        {
            _crouchIsPressed = false;
        }

        // If there horizontal movement, reset the time at crouch
        if (Input.GetAxisRaw(Axis.Horizontal) != 0.0f)
        {
            _timeAtCrouch = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (_crouchIsPressed)
        {
            // Check if the player has been crouching for a while
            if (Time.time - _timeAtCrouch > timeBeforeLookingDown)
            {
                if (!_isPanning)
                {
                    _originalScreenY = _framingTransposer.m_ScreenY;
                    _isPanning = true;
                }
            }
        }

        if (_isPanning)
        {
            if(_crouchIsPressed)
            {
                // Pan the camera down
                _framingTransposer.m_ScreenY = Mathf.Max(_framingTransposer.m_ScreenY - panningSpeed * Time.fixedDeltaTime, _originalScreenY + maximumScreenYDelta);
            }
            else
            {
                // Pan the camera back up
                _framingTransposer.m_ScreenY = Mathf.Min(_framingTransposer.m_ScreenY + panningSpeed * Time.fixedDeltaTime, _originalScreenY);

                if (_framingTransposer.m_ScreenY == _originalScreenY)
                {
                    _isPanning = false;
                }
            }
        }
    }
}
