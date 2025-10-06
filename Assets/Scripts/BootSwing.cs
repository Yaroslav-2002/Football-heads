using System;
using UnityEngine;
using UnityEngine.Events;

public class BootSwing : MonoBehaviour
{
    [Header("Swing Target")]
    [SerializeField] private Transform kickPosition;

    [Header("Timing")]
    [Min(0f)]
    [SerializeField] private float swingDuration = 0.35f;
    [Min(0f)]
    [SerializeField] private float kickCooldown = 0.25f;

    [Min(0f)]
    [SerializeField] private float kickForce = 0.25f;

    [Header("Events")]
    [SerializeField] private UnityEvent onKickPeak;

    public event Action KickPeakReached;

    public float LastKickCharge { get; private set; }

    private float _nextKickAllowedTime;
    private Vector3 _restLocalPosition;
    private Quaternion _restLocalRotation;
    private Vector3 _kickLocalPosition;
    private Quaternion _kickLocalRotation;
    private Transform _parent;
    private float _kickProgress;
    private bool _kickHeld;
    private bool _peakInvokedForCurrentPress;
    private float _pressStartTime;

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

    private void Update()
    {
        if (kickPosition == null)
        {
            return;
        }

        if (_kickHeld)
        {
            float elapsed = Time.time - _pressStartTime;
            float progress = CalculateChargeNormalized(elapsed);
            SetKickProgress(progress);
        }
        else if (!Mathf.Approximately(_kickProgress, 0f))
        {
            float progress = Mathf.MoveTowards(_kickProgress, 0f, GetProgressStep());
            SetKickProgress(progress);

            if (Mathf.Approximately(_kickProgress, 0f))
            {
                _peakInvokedForCurrentPress = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Vector2 dir = -collision.GetContact(0).normal;
            collision.rigidbody.AddForce(dir * kickForce, ForceMode2D.Impulse);
        }
    }

    private void CacheParentSpace()
    {
        _parent = transform.parent;

        _restLocalPosition = WorldToLocal(transform.position);
        _restLocalRotation = WorldToLocal(transform.rotation);

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

    public void BeginKickPress()
    {
        if (kickPosition == null)
        {
            return;
        }

        if (Time.time < _nextKickAllowedTime)
        {
            return;
        }

        if (_kickHeld)
        {
            return;
        }

        CacheParentSpace();

        float duration = Mathf.Max(0.0001f, swingDuration);
        _pressStartTime = Time.time - _kickProgress * duration;
        _kickHeld = true;
        _peakInvokedForCurrentPress = _kickProgress >= 0.999f;
    }

    public void EndKickPress()
    {
        if (!_kickHeld && Mathf.Approximately(_kickProgress, 0f))
        {
            return;
        }

        if (_kickHeld)
        {
            LastKickCharge = _kickProgress;
        }

        _kickHeld = false;
        _nextKickAllowedTime = Time.time + kickCooldown;
    }

    private void MoveToRestInstant()
    {
        _kickProgress = 0f;
        _kickHeld = false;
        _peakInvokedForCurrentPress = false;
        transform.localPosition = _restLocalPosition;
        transform.localRotation = _restLocalRotation;
    }

    private void ApplySwing(Vector3 localPosition, Quaternion localRotation)
    {
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }

    private void SetKickProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        bool reachedPeak = !_peakInvokedForCurrentPress && progress >= 0.999f;

        Vector3 position = ArcLerp(_restLocalPosition, _kickLocalPosition, progress);
        Quaternion rotation = Quaternion.Slerp(_restLocalRotation, _kickLocalRotation, progress);

        _kickProgress = progress;
        ApplySwing(position, rotation);

        if (reachedPeak)
        {
            _peakInvokedForCurrentPress = true;
            KickPeakReached?.Invoke();
            onKickPeak?.Invoke();
        }
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

    private float CalculateChargeNormalized(float holdDuration)
    {
        float duration = Mathf.Max(0.0001f, swingDuration);
        return Mathf.Clamp01(holdDuration / duration);
    }

    private float GetProgressStep()
    {
        float duration = Mathf.Max(0.0001f, swingDuration);
        return Time.deltaTime / duration;
    }
}
