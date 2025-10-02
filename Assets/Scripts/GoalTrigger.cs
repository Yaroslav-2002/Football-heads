using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private int gateId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball"))
        {
            return;
        }

        GameManager manager = GameManager.Instance;
        if (manager == null)
        {
            Debug.LogWarning("GoalTrigger: No active GameManager found in the scene.", this);
            return;
        }

        manager.GoalScored(gateId);
    }
}
