using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAutoScaler : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;

    void Update()
    {
        if (!backgroundRenderer) return;

        float targetWidth = backgroundRenderer.bounds.size.x;
        float targetHeight = backgroundRenderer.bounds.size.y;

        Camera cam = GetComponent<Camera>();
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = targetWidth / targetHeight;

        if (screenRatio >= targetRatio)
        {
            // Wider screen: match height
            cam.orthographicSize = targetHeight / 2f;
        }
        else
        {
            // Taller screen: match width
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = targetHeight / 2f * differenceInSize;
        }
    }
}
