using UnityEngine;

public class EggController : MonoBehaviour
{
    public float baseGravity = 1f;
    public float gravityIncrement = 0.2f;
    public Rigidbody2D rb;
    public TrialManager trialManager;

    private int currentRound = 0;

    void Start()
    {
        rb.gravityScale = baseGravity;
    }

    public void StartRound(int roundNumber)
    {
        currentRound = roundNumber;
        rb.gravityScale = baseGravity + gravityIncrement * (currentRound - 1);
        transform.position = new Vector2(Random.Range(-6f, 6f), 6f); // arriba
        rb.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pan"))
        {
            trialManager.RoundSuccess();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            trialManager.RoundFail();
        }
    }
}
