using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [SerializeField] GameobjectsRuntimeSet gameobjects;
    [SerializeField] Timer Timer;
    [SerializeField] GameSettingsView gameSettingsView;

    private void OnEnable()
    {
        Timer.OnTimeGameTmeExpired += Pause;
        gameSettingsView.OnShowed += Pause;
        gameSettingsView.OnClosed += UnPause;
    }

    private void OnDisable()
    {
        Timer.OnTimeGameTmeExpired -= Pause;
        gameSettingsView.OnShowed -= Pause;
        gameSettingsView.OnClosed -= UnPause;
    }

    private void Pause()
    {
        foreach (var gameObject in gameobjects.items)
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        Timer.Pause();
    }

    private void UnPause()
    {
        
        foreach (var gameObject in gameobjects.items)
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        Timer.UnPause();
    }
}
