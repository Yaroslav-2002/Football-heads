using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [SerializeField] GameobjectsRuntimeSet gameobjects;
    [SerializeField] Timer Timer;
    [SerializeField] GameSettingsView gameSettingsView;

    private void OnEnable()
    {
        Timer.OnGameTimeExpired += Pause;
        gameSettingsView.OnShowed += Pause;
        gameSettingsView.OnClosed += UnPause;
    }

    private void OnDisable()
    {
        Timer.OnGameTimeExpired -= Pause;
        gameSettingsView.OnShowed -= Pause;
        gameSettingsView.OnClosed -= UnPause;
    }

    private void Pause()
    {
        foreach (var gameObject in gameobjects.items)
        {
            if (gameObject.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.bodyType = RigidbodyType2D.Kinematic;
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        Time.timeScale = 0f;
        Timer.Pause();
    }

    private void UnPause()
    {
        
        foreach (var gameObject in gameobjects.items)
        {
            if (gameObject.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        Time.timeScale = 1f;
        Timer.UnPause();
    }
}
