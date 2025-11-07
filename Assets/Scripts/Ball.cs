using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)] private float ballRespawnForce = 5f;

    public void Respawn(TeamSide teamside)
    {
        ApplyRespawnForce(teamside);
    }

    private Rigidbody2D _rigidbody2D;
    private Rigidbody2D Rigidbody2D
    {
        get
        {
            if (Rigidbody2D == null)
                _rigidbody2D = GetComponent<Rigidbody2D>();

            return _rigidbody2D;
        }
        set
        {
            _rigidbody2D = value;
        }
    }

    private void ResetBallPhysics()
    {

        Rigidbody2D.linearVelocity = Vector2.zero;
        Rigidbody2D.angularVelocity = 0f;

    }

    private void ApplyRespawnForce(TeamSide scoringTeam)
    {
        if (ballRespawnForce <= 0f)
            return;

        Vector2 direction = GetRespawnDirection(scoringTeam);
        if (direction.sqrMagnitude <= Mathf.Epsilon)
            return;

        ResetBallPhysics();
        Rigidbody2D.AddForce(direction.normalized * ballRespawnForce, ForceMode2D.Impulse);
    }

    protected virtual Vector2 GetRespawnDirection(TeamSide teamSide)
    {
        Vector2 direction = teamSide switch
        {
            TeamSide.Left => new Vector2(1f, 0),
            TeamSide.Right => new Vector2(-1f, 0),
            _ => Vector2.zero
        };

        return direction;
    }
}
