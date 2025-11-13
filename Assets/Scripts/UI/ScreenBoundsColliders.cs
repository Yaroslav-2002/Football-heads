using UnityEngine;

public class ScreenBoundsColliders : MonoBehaviour
{
    [Tooltip("Target camera to match width with. If null, will use Camera.main.")]
    public Camera targetCamera;

    [Tooltip("Optional multiplier to slightly enlarge or shrink the width.")]
    public float widthMultiplier = 1f;

    private void Start()
    {
        ApplyScale();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Also update live in editor for layout previews
        if (!Application.isPlaying)
            ApplyScale();
#endif
    }

    private void OnEnable() => ApplyScale();

    public void ApplyScale()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
        if (targetCamera == null)
            return;

        if (!targetCamera.orthographic)
        {
            Debug.LogWarning("FitToScreenWidth works best with Orthographic cameras.", this);
            return;
        }

        // Get world width visible by camera
        float worldScreenHeight = targetCamera.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * targetCamera.aspect;

        // Match the local X scale to screen width
        Vector3 scale = transform.localScale;
        scale.x = worldScreenWidth * widthMultiplier;
        transform.localScale = scale;
    }
}
