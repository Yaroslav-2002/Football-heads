using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private int gateId;
    [SerializeField] private GameInitializer gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            gameManager.GoalScored(gateId);
        }
    }
}
