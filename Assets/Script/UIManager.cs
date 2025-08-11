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

    public PanController panController;

    private GameManager gameManager;

    public Button modeAButton;
    public Button modeBButton;
    private string selectedMode;

    private void Start()
    {
        modeAButton.onClick.AddListener(() => SelectMode("ModeA"));
        modeBButton.onClick.AddListener(() => SelectMode("ModeB"));
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void SelectMode(string mode)
    {
        selectedMode = mode;
        
        modeAButton.interactable = false;
        modeBButton.interactable = false;

        
        gameManager.SetSelectedMode(mode);
    }

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
            gameManager.SetTotalRounds(rounds);
        }
        else
        {
            Debug.LogWarning("Cantidad de rondas inválida. Usando 20 por defecto.");
            gameManager.SetTotalRounds(20);
        }

        formPanel.SetActive(false);
        gameManager.ShowStartGamePanel();
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


    public void PlayInAMode()
    {
        panController.useModeA = true;
    }

    public void PlayInBMode()
    {
        panController.useModeA = false;
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
        modeAButton.interactable = true;
        modeBButton.interactable = true;
    }

}
