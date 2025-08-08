using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedChance
{
    public float weight;
    public float interferenceValue;
}

public class InterferenceManager : MonoBehaviour
{
    public List<WeightedChance> weightedChances;
    private float totalWeights;

    void Start()
    {
        CalculateTotalWeights();
    }

    void CalculateTotalWeights()
    {
        totalWeights = 0f;
        foreach (var chance in weightedChances)
        {
            totalWeights += chance.weight;
        }
    }

    public float GetRandomWeightedInterference()
    {
        float randomValue = Random.Range(0f, totalWeights);
        float cumulativeWeight = 0f;

        foreach (var chance in weightedChances)
        {
            cumulativeWeight += chance.weight;
            if (randomValue <= cumulativeWeight)
            {
                return chance.interferenceValue;
            }
        }

        return weightedChances[0].interferenceValue; // Valor por defecto
    }
}