using UnityEngine;

public class InterferenceController : MonoBehaviour
{
    public InterferenceManager interferenceManager;
    public float currentInterference = 0f;
    public float transitionSpeed = 0.5f;
    public int currentRound = 1;
    public int totalRounds = 20;

    // Parámetros de la sigmoide
    public float sigmoidSteepness = 0.8f;
    public float sigmoidMidPoint = 0.6f;

    void Update()
    {
        // Obtener el valor objetivo basado en la ruleta
        float targetInterference = interferenceManager.GetRandomWeightedInterference();

        // Aplicar curva sigmoide al progreso del juego
        float roundProgress = (float)currentRound / totalRounds;
        float sigmoidProgress = Sigmoid(roundProgress, sigmoidSteepness, sigmoidMidPoint);

        // Interpolación suave hacia el valor objetivo
        currentInterference = Mathf.Lerp(
            currentInterference,
            targetInterference * sigmoidProgress,
            Time.deltaTime * transitionSpeed
        );
    }

    // Función sigmoide personalizable
    private float Sigmoid(float x, float steepness, float midPoint)
    {
        return 1f / (1f + Mathf.Exp(-steepness * (x - midPoint)));
    }

    public void AdvanceRound()
    {
        currentRound = Mathf.Clamp(currentRound + 1, 1, totalRounds);
    }
}