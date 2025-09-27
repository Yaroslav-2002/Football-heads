using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BootSwing : MonoBehaviour
{
    [Header("Swing Target")]
    public Transform kickPosition;

    [Header("Timing")]
    [Min(0f)]
    public float swingDuration = 0.35f;
    [Min(0f)]
    public float kickCooldown = 0.25f;

    [Header("Events")]
    public UnityEvent onKickPeak;

    public event Action KickPeakReached;

    private bool _isSwinging;
    private float _nextKickAllowedTime;
    private Vector3 _restLocalPosition;
    private Quaternion _restLocalRotation;
    private Vector3 _kickLocalPosition;
    private Quaternion _kickLocalRotation;
    private Transform _parent;
    private bool _waitingForKickRelease;

    private void Awake()
    {
        CacheParentSpace();
        MoveToRestInstant();
    }

    private void OnValidate()
    {
        CacheParentSpace();
        MoveToRestInstant();
    }

    private void CacheParentSpace()
    {
        _parent = transform.parent;

        _restLocalPosition = WorldToLocal(gameObject.transform.position);
        _restLocalRotation = WorldToLocal(gameObject.transform.rotation);


        if (kickPosition != null)
        {
            _kickLocalPosition = WorldToLocal(kickPosition.position);
            _kickLocalRotation = WorldToLocal(kickPosition.rotation);
        }
        else
        {
            _kickLocalPosition = transform.localPosition;
            _kickLocalRotation = transform.localRotation;
        }
    }

    private void Update()
    {
        UpdateKickReleaseState();

        if (_isSwinging)
        {
            return;
        }

        if (Time.time < _nextKickAllowedTime)
        {
            return;
        }

        if (IsKickTriggered())
        {
            StartCoroutine(SwingRoutine());
        }
    }

    private bool IsKickTriggered()
    {
        if (_waitingForKickRelease)
        {
            return false;
        }

        return WasKickPressedThisFrame();
    }

    private void UpdateKickReleaseState()
    {
        if (!_waitingForKickRelease)
        {
            return;
        }

        if (WasKickReleasedThisFrame() || !IsKickPressed())
        {
            _waitingForKickRelease = false;
        }
    }

    private bool WasKickPressedThisFrame()
    {
        bool kickPressed = false;

        if (Keyboard.current != null)
        {
            kickPressed |= Keyboard.current.spaceKey.wasPressedThisFrame ||
                           Keyboard.current.kKey.wasPressedThisFrame ||
                           Keyboard.current.rightCtrlKey.wasPressedThisFrame;
        }

        if (Gamepad.current != null)
        {
            kickPressed |= Gamepad.current.buttonEast.wasPressedThisFrame ||
                           Gamepad.current.rightTrigger.wasPressedThisFrame;
        }

        return kickPressed;
    }

    private bool WasKickReleasedThisFrame()
    {
        bool kickReleased = false;

        if (Keyboard.current != null)
        {
            kickReleased |= Keyboard.current.spaceKey.wasReleasedThisFrame ||
                            Keyboard.current.kKey.wasReleasedThisFrame ||
                            Keyboard.current.rightCtrlKey.wasReleasedThisFrame;
        }

        if (Gamepad.current != null)
        {
            kickReleased |= Gamepad.current.buttonEast.wasReleasedThisFrame ||
                            Gamepad.current.rightTrigger.wasReleasedThisFrame;
        }

        return kickReleased;
    }

    private bool IsKickPressed()
    {
        bool kickPressed = false;

        if (Keyboard.current != null)
        {
            kickPressed |= Keyboard.current.spaceKey.isPressed ||
                           Keyboard.current.kKey.isPressed ||
                           Keyboard.current.rightCtrlKey.isPressed;
        }

        if (Gamepad.current != null)
        {
            kickPressed |= Gamepad.current.buttonEast.isPressed ||
                           Gamepad.current.rightTrigger.IsPressed();
        }

        return kickPressed;
    }

    private IEnumerator SwingRoutine()
    {
        if (kickPosition == null)
        {
            yield break;
        }

        CacheParentSpace();
        _isSwinging = true;
        _waitingForKickRelease = true;
        float duration = Mathf.Max(0.0001f, swingDuration);
        float halfDuration = duration * 0.5f;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float eased = EaseOutSine(t);
            ApplySwing(ArcLerp(_restLocalPosition, _kickLocalPosition, eased),
                       Quaternion.Slerp(_restLocalRotation, _kickLocalRotation, eased));
            yield return null;
        }

        ApplySwing(_kickLocalPosition, _kickLocalRotation);
        KickPeakReached?.Invoke();
        onKickPeak?.Invoke();
        Debug.Log("Boot reached peak swing position.");

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float eased = EaseInSine(t);
            ApplySwing(ArcLerp(_kickLocalPosition, _restLocalPosition, eased),
                       Quaternion.Slerp(_kickLocalRotation, _restLocalRotation, eased));
            yield return null;
        }

        MoveToRestInstant();
        _nextKickAllowedTime = Time.time + kickCooldown;
        _isSwinging = false;
    }

    private void MoveToRestInstant()
    {
        transform.localPosition = _restLocalPosition;
        transform.localRotation = _restLocalRotation;
    }

    private void ApplySwing(Vector3 localPosition, Quaternion localRotation)
    {
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }

    private Vector3 ArcLerp(Vector3 from, Vector3 to, float t)
    {
        if (from.sqrMagnitude < Mathf.Epsilon || to.sqrMagnitude < Mathf.Epsilon)
        {
            return Vector3.Lerp(from, to, t);
        }

        float fromMagnitude = from.magnitude;
        float toMagnitude = to.magnitude;
        Vector3 fromDirection = from / fromMagnitude;
        Vector3 toDirection = to / toMagnitude;
        Vector3 direction = Vector3.Slerp(fromDirection, toDirection, t);
        float magnitude = Mathf.Lerp(fromMagnitude, toMagnitude, t);
        return direction * magnitude;
    }

    private Vector3 WorldToLocal(Vector3 worldPosition)
    {
        return _parent != null ? _parent.InverseTransformPoint(worldPosition) : worldPosition;
    }

    private Quaternion WorldToLocal(Quaternion worldRotation)
    {
        return _parent != null ? Quaternion.Inverse(_parent.rotation) * worldRotation : worldRotation;
    }

    private static float EaseOutSine(float t)
    {
        return Mathf.Sin(t * Mathf.PI * 0.5f);
    }

    private static float EaseInSine(float t)
    {
        return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
    }
}
