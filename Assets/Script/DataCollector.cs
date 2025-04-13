using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticipantData
{
    public string nombre;
    public string sexo;
    public int edad;
    public string dni;
    public string numeroParticipante;
}

[System.Serializable]
public class TrialData
{
    public int trial;
    public int correct;
    public int total;
    public int a, b, c;
}

[System.Serializable]
public class FullResult
{
    public ParticipantData participante;
    public TrialData trial1;
    public TrialData trial2;
    public TrialData trial3;
}

public enum SaveFormat
{
    JSON,
    CSV
}

public class DataCollector : MonoBehaviour
{
    public ParticipantData participante = new ParticipantData();
    private List<TrialData> trials = new List<TrialData>();

    public SaveFormat saveFormat = SaveFormat.JSON;

    public void SaveTrial(int trial, int correct, int total, int a, int b, int c)
    {
        Debug.Log($"Guardando Trial {trial} - Correctas: {correct}, Total: {total}, A: {a}, B: {b}, C: {c}");

        trials.Add(new TrialData
        {
            trial = trial,
            correct = correct,
            total = total,
            a = a,
            b = b,
            c = c
        });
    }

    public void SaveAllTrials()
    {
        if (trials.Count < 3)
        {
            Debug.LogError("No hay suficientes trials para guardar. Trials actuales: " + trials.Count);
            return;
        }

        string sanitizedNombre = participante.nombre.Replace(" ", "_");
        string sanitizedNumero = participante.numeroParticipante.Replace(" ", "_");
        string fileName = $"{sanitizedNombre}_{sanitizedNumero}";

        if (saveFormat == SaveFormat.JSON)
        {
            FullResult result = new FullResult
            {
                participante = participante,
                trial1 = trials[0],
                trial2 = trials[1],
                trial3 = trials[2]
            };

            string path = Application.persistentDataPath + "/" + fileName + ".json";
            string json = JsonUtility.ToJson(result, true);
            System.IO.File.WriteAllText(path, json);

            Debug.Log("Datos guardados en: " + path);
        }
        else if (saveFormat == SaveFormat.CSV)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".csv";
            List<string> lines = new List<string>();

            lines.Add("Trial,Correct,Total,A,B,C");
            lines.Add($"{trials[0].trial},{trials[0].correct},{trials[0].total},{trials[0].a},{trials[0].b},{trials[0].c}");
            lines.Add($"{trials[1].trial},{trials[1].correct},{trials[1].total},{trials[1].a},{trials[1].b},{trials[1].c}");
            lines.Add($"{trials[2].trial},{trials[2].correct},{trials[2].total},{trials[2].a},{trials[2].b},{trials[2].c}");

            System.IO.File.WriteAllLines(path, lines);

            Debug.Log("Datos guardados en CSV en: " + path);
        }
    }
public void ResetData()
{
    participante = new ParticipantData();
    trials.Clear();
    saveFormat = SaveFormat.JSON;
}

}
