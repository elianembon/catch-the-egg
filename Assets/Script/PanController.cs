using UnityEngine;

public class PanController : MonoBehaviour
{
    public enum PanMode { Manual, HelpPlayer, HinderPlayer }

    [Header("Debug")]
    [SerializeField]
    private float _currentInterference;


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
    [Range(0f, 1f)] public float maxAssistanceRatio;
    [Tooltip("Retraso antes de que comience la asistencia (segundos)")]
    public float assistanceDelay = 0.5f;

    [Range(0, 0.5f)] public float maxErrorMagnitude = 0.10f; //Margen maximo de error en HelpPlayer
    [Range(0, 1)] public float errorProbability = 0.3f; 

    private float assistanceTimer = 0f;
    private bool isPlayerMoving = false;

    private float currentSpeed = 0f;
    private bool canMove = true;
    private int currentRound = 1;
    private int totalRounds = 20;

    private void FixedUpdate()
    {
        _currentInterference = interferenceController.GetCurrentInterference();

        if (_currentInterference >= maxAssistanceRatio)
        {
            interferenceController.SetPause(true);
        }

        
        if (!canMove) return;

        float playerInput = Input.GetAxis("Horizontal");
        bool isPlayerMoving = Mathf.Abs(playerInput) > 0.1f;

        UpdateAssistanceTimer(isPlayerMoving);
        float finalInput = playerInput + CalculateAssistance();

        ApplyMovementPhysics(finalInput, Time.fixedDeltaTime);
        ClampPosition();
    }


    private void UpdateAssistanceTimer(bool isPlayerMoving)
    {
        assistanceTimer = isPlayerMoving ?
            Mathf.Min(assistanceTimer + Time.fixedDeltaTime, assistanceDelay) :
            0f;
    }



    private float CalculateAssistance()
    {
        if (currentPanMode == PanMode.Manual || assistanceTimer < assistanceDelay)
            return 0f;

        float distanceToEgg = horizontalDetector.position.x - transform.position.x;
        float normalizedDistance = Mathf.Clamp(distanceToEgg / 5f, -1f, 1f);


        if (currentPanMode == PanMode.HelpPlayer && Random.value < errorProbability)
        {
            float error = Random.Range(-maxErrorMagnitude, maxErrorMagnitude);
            normalizedDistance *= (1f + error);
        }

        bool shouldInvert = (currentPanMode == PanMode.HinderPlayer && useModeA) ||
                           (currentPanMode == PanMode.HelpPlayer && !useModeA);
        if (shouldInvert)
            normalizedDistance = -normalizedDistance;

        float interferenceFactor = interferenceController.GetCurrentInterference() * maxAssistanceRatio;
        return normalizedDistance * interferenceFactor;
    }

    void ApplyMovementPhysics(float targetInput, float deltaTime)
    {
        float targetSpeed = targetInput * maxSpeed;
        float accelerationRate = (Mathf.Sign(targetInput) != Mathf.Sign(currentSpeed) &&
                               Mathf.Abs(targetInput) > 0.1f ?
            acceleration * turnAroundMultiplier :
            Mathf.Abs(targetInput) > 0.1f ? acceleration : deceleration);

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate * deltaTime);
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        transform.position += Vector3.right * currentSpeed * deltaTime;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        if ((pos.x <= minX && currentSpeed < 0) || (pos.x >= maxX && currentSpeed > 0))
            currentSpeed = 0f;

        transform.position = pos;
    }

    public void SetMode(PanMode newMode)
    {
        currentPanMode = newMode;
        interferenceController.ResetCurve();
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
