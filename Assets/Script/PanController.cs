using UnityEngine;

public class PanController : MonoBehaviour
{
    public enum PanMode { Manual, HelpPlayer, HinderPlayer }
    public PanMode mode = PanMode.Manual;


    public Transform horizontalDetector;

    [Header("Movement Settings")]
    public float maxSpeed = 15f;           
    public float acceleration = 15f;       
    public float deceleration = 15f;       
    public float turnAroundMultiplier = 2f; 


    [Header("Position Limits")]
    public float minX = -10f;
    public float maxX = 10f;



    private float currentSpeed = 0f;       

    private bool canMove = true;
    private int currentRound = 1;
    private int totalRounds = 20;


    private void FixedUpdate()
    {
        if (!canMove) return;

        float playerInput = Input.GetAxis("Horizontal");
        float autoInput = CalculateAutoInput();

        float lossOfControl = (mode != PanMode.Manual) ?
            Mathf.Clamp01((float)(currentRound - 1) / (totalRounds - 1)) : 0f;

        float targetInput = (playerInput * (1f - lossOfControl)) + (autoInput * lossOfControl);

        ApplyMovementPhysics(targetInput, Time.fixedDeltaTime);

        ClampPosition();
    }



    float CalculateAutoInput()
    {
        if (mode == PanMode.Manual) return 0f;

        Vector3 pos = transform.position;
        float distanceToEgg = horizontalDetector.position.x - pos.x;
        float autoInput = Mathf.Clamp(distanceToEgg, -1f, 1f);

        return (mode == PanMode.HinderPlayer) ? -autoInput : autoInput;
    }

    void ApplyMovementPhysics(float targetInput, float deltaTime)
    {
        float targetSpeed = targetInput * maxSpeed;

        // Determinar si estamos intentando cambiar de dirección
        bool wantsToChangeDirection = Mathf.Sign(targetInput) != Mathf.Sign(currentSpeed) &&
                                     Mathf.Abs(targetInput) > 0.1f;

        // Calcular aceleración efectiva
        float effectiveAcceleration;

        if (wantsToChangeDirection)
        {
            // Aplicar aceleración inmediata en la nueva dirección
            effectiveAcceleration = acceleration * turnAroundMultiplier;
            currentSpeed += effectiveAcceleration * Mathf.Sign(targetInput) * deltaTime;
        }
        else if (Mathf.Abs(targetInput) > 0.1f) // Aceleración normal
        {
            effectiveAcceleration = acceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, effectiveAcceleration * deltaTime);
        }
        else // Frenado natural
        {
            effectiveAcceleration = deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, effectiveAcceleration * deltaTime);
        }

        // Limitar velocidad máxima
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Aplicar movimiento
        Vector3 newPos = transform.position;
        newPos.x += currentSpeed * deltaTime;
        transform.position = newPos;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);


        if ((pos.x <= minX && currentSpeed < 0) || (pos.x >= maxX && currentSpeed > 0))
        {
            currentSpeed = 0f;
        }

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