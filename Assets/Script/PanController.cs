using UnityEngine;

public class PanController : MonoBehaviour
{
    public enum PanMode { Manual, HelpPlayer, HinderPlayer }

    [Header("Mode Settings")]
    public bool useModeA = true;
    private PanMode currentPanMode = PanMode.Manual;

    public Transform horizontalDetector;

    public InterferenceController interferenceController;

    [Header("Movement Settings")]
    public float maxSpeed = 15f;
    public float acceleration = 15f;
    public float deceleration = 15f;
    public float turnAroundMultiplier = 2f;

    [Header("Position Limits")]
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Assistance Settings")]
    [Tooltip("Fuerza máxima de asistencia (como porcentaje de la aceleración normal)")]
    [Range(0f, 1f)] public float maxAssistanceRatio = 0.3f;
    [Tooltip("Retraso antes de que comience la asistencia (segundos)")]
    public float assistanceDelay = 0.5f;

    private float assistanceTimer = 0f;
    private bool isPlayerMoving = false;

    private float currentSpeed = 0f;
    private bool canMove = true;
    private int currentRound = 1;
    private int totalRounds = 20;

    private void FixedUpdate()
    {
        if (!canMove) return;

        float playerInput = Input.GetAxis("Horizontal");
        isPlayerMoving = Mathf.Abs(playerInput) > 0.1f;

        // Debug útil (comenta esto en la versión final)
        Debug.Log($"Interferencia: {interferenceController.currentInterference:F2}");

        UpdateAssistanceTimer();
        float finalInput = CalculateFinalInput(playerInput);

        ApplyMovementPhysics(finalInput, Time.fixedDeltaTime);
        ClampPosition();
    }


    private void UpdateAssistanceTimer()
    {
        assistanceTimer = isPlayerMoving ?
                         Mathf.Min(assistanceTimer + Time.fixedDeltaTime, assistanceDelay) :
                         0f;
    }

    private float CalculateFinalInput(float playerInput)
    {
        float assistance = (assistanceTimer >= assistanceDelay) ?
                         CalculateAssistanceInput() * GetEffectiveAssistanceRatio() :
                         0f;

        return playerInput + assistance;
    }

    private float GetEffectiveAssistanceRatio()
    {
        return maxAssistanceRatio * interferenceController.currentInterference;
    }

    private float CalculateFinalAssistance()
    {
        // Obtiene la interferencia actual (0-1)
        float interference = interferenceController.currentInterference;

        // Combina con el ratio máximo
        float effectiveRatio = maxAssistanceRatio * interference;

        // Calcula la asistencia base
        float assistance = CalculateAssistanceInput();

        // Aplica el ratio efectivo
        return assistance * effectiveRatio;
    }


    float CalculateAssistanceInput()
    {
        if (currentPanMode == PanMode.Manual) return 0f;

        Vector3 pos = transform.position;
        float distanceToEgg = horizontalDetector.position.x - pos.x;

        // Normalizamos la distancia (valor entre -1 y 1)
        float normalizedDistance = Mathf.Clamp(distanceToEgg / 5f, -1f, 1f);

        // Aplicamos el modo (invertimos si es HinderPlayer)
        if ((currentPanMode == PanMode.HinderPlayer && useModeA) ||
            (currentPanMode == PanMode.HelpPlayer && !useModeA))
        {
            normalizedDistance = -normalizedDistance;
        }

        return normalizedDistance;
    }

    void ApplyMovementPhysics(float targetInput, float deltaTime)
    {
        float targetSpeed = targetInput * maxSpeed;

        bool wantsToChangeDirection = Mathf.Sign(targetInput) != Mathf.Sign(currentSpeed) &&
                                    Mathf.Abs(targetInput) > 0.1f;

        float effectiveAcceleration;

        if (wantsToChangeDirection)
        {
            effectiveAcceleration = acceleration * turnAroundMultiplier;
            currentSpeed += effectiveAcceleration * Mathf.Sign(targetInput) * deltaTime;
        }
        else if (Mathf.Abs(targetInput) > 0.1f)
        {
            effectiveAcceleration = acceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, effectiveAcceleration * deltaTime);
        }
        else
        {
            effectiveAcceleration = deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, effectiveAcceleration * deltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

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

    public void SetModeSelection(bool useA)
    {
        useModeA = useA;
       
    }

    public void SetMode(PanMode newMode)
    {
        currentPanMode = newMode;
        
    }

    public void SetCurrentRound(int round, int total)
    {
        currentRound = round;
        totalRounds = total;
    }

    public void Freeze()
    {
        canMove = false;
        currentSpeed = 0f;
    }

    public void Unfreeze()
    {
        canMove = true;
    }
}
