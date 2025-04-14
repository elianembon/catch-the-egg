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
        transform.position = new Vector2(Random.Range(-6f, 6f), 6f); // start from top
        rb.linearVelocity = Vector2.zero;
    }

    public void Freeze()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void Unfreeze()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Pan"))
    {
        float eggX = transform.position.x;
        float panX = collision.transform.position.x;
        float panWidth = collision.collider.bounds.size.x;

        float validRange = panWidth * 0.9f;

        if (Mathf.Abs(eggX - panX) <= validRange / 2f)
        {
            trialManager.RoundSuccess();
            
            Freeze();
            collision.gameObject.GetComponent<PanController>().Freeze();
        }    else  {
            Vector2 pushDirection = new Vector2(Mathf.Sign(eggX - panX), 1f).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(pushDirection * 5f, ForceMode2D.Impulse);
        }
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
