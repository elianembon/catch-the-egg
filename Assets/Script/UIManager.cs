using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("Panels HUD")]
    public GameObject formPanel, surveyPanel, endPanel;

    [Header("Data FormPanel")]
    public InputField nombre, sexo, dni, edad, numero;

    [Header("SurveyPanel Elements")]
    public Slider respuestaA, respuestaB, respuestaC, respuestaD;


    public DataCollector data;

    public Dropdown formatoDropdown;

    public InputField inputRounds;

    public void ShowForm()
    {
        formPanel.SetActive(true);
    }

    public void SubmitForm()
    {
        data.participante.nombre = nombre.text;
        data.participante.sexo = sexo.text;
        data.participante.dni = dni.text;
        data.participante.edad = int.Parse(edad.text);
        data.participante.numeroParticipante = numero.text;

        SetSaveFormatFromDropdown();

        int rounds;
        if (int.TryParse(inputRounds.text, out rounds))
        {
            FindObjectOfType<GameManager>().SetTotalRounds(rounds);
        }
        else
        {
            Debug.LogWarning("Cantidad de rondas inválida. Usando 20 por defecto.");
            FindObjectOfType<GameManager>().SetTotalRounds(20);
        }

        formPanel.SetActive(false);
        FindObjectOfType<GameManager>().ShowStartGamePanel();
    }


    public void SetSaveFormatFromDropdown()
    {
        if (formatoDropdown.value == 0)
        {
            data.saveFormat = SaveFormat.JSON;
        }
        else if (formatoDropdown.value == 1)
        {
            data.saveFormat = SaveFormat.CSV;
        }
    }

    public void ShowSurvey(System.Action<int, int, int, int> onComplete)
    {
        surveyPanel.SetActive(true);
        surveyPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        surveyPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            surveyPanel.SetActive(false);
            onComplete((int)respuestaA.value, (int)respuestaB.value, (int)respuestaC.value, (int)respuestaD.value);
        });
    }

    public void ShowEndScreen()
    {
        endPanel.SetActive(true);
    }
    public void RestartGame()
    {
        data.ResetData();
        endPanel.SetActive(false);
        formPanel.SetActive(true);
    }

}
