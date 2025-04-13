using UnityEngine;

public class PanController : MonoBehaviour
{
    public enum PanMode { Manual, HelpPlayer, HinderPlayer }
    public PanMode mode = PanMode.Manual;

    public float baseSpeed = 5f;
    public Transform horizontalDetector;

    private bool canMove = true;
    private int currentRound = 1;
    private int totalRounds = 20;

    void Update()
{
    if (!canMove) return;

    Vector3 pos = transform.position;

    float playerInput = Input.GetAxis("Horizontal");

    float autoInput = 0f;

    if (mode == PanMode.HelpPlayer || mode == PanMode.HinderPlayer)
    {
        float distanceToEgg = horizontalDetector.position.x - pos.x;
        autoInput = Mathf.Clamp(distanceToEgg, -1f, 1f);

        if (mode == PanMode.HinderPlayer)
            autoInput *= -1f;
    }

    float lossOfControl = 0f;
    if (mode != PanMode.Manual)
    {
        lossOfControl = Mathf.Clamp01((float)(currentRound - 1) / (totalRounds - 1));
    }

    float finalInput = (playerInput * (1f - lossOfControl)) + (autoInput * lossOfControl);

    pos.x += finalInput * baseSpeed * Time.deltaTime;

    pos.x = Mathf.Clamp(pos.x, -7f, 7f);
    transform.position = pos;
}


    public void SetMode(PanMode newMode)
    {
        mode = newMode;
    }

    public void SetCurrentRound(int round, int total)
    {
        currentRound = round;
        totalRounds = total;
    }

    public void Freeze()
    {
        canMove = false;
    }

    public void Unfreeze()
    {
        canMove = true;
    }
}
