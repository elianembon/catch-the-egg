using System.Collections.Generic;
using UnityEngine;

public class InterferenceController : MonoBehaviour
{
    [Header("Configuración Sigmoide")]
    public float steepness = 2.5f; //Controla qué tan pronunciada es la curva
    public float midpoint = 0.5f;//punto de inflexión en base al tiempo 0.5 es el medio 
    public float duration = 10f;
    public float maxInterference = 1f;

    private float timer = 0f;
    private bool isPaused = false;

    void Update()
    {
        if (!isPaused)
        {
            timer += Time.deltaTime;
            timer %= duration; 
        }
    }

    
    public void ResetCurve()
    {
        timer = 0f;
        SetPause(false);
        Debug.Log("Curva sigmoide reiniciada");
    }

    
    public void SetPause(bool paused)
    {
        isPaused = paused;
    }

    public float GetCurrentInterference()
    {
        float progress = timer / duration;
        float rawSigmoid = 1f / (1f + Mathf.Exp(-steepness * (progress - midpoint)));
        return rawSigmoid * maxInterference;
    }
}