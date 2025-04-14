using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TrialManager trialManager;
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip loseSound;
    public UIManager uiManager;
    public DataCollector dataCollector;
public int roundsPerTrial = 20; // default

public GameObject startGamePanel; // para mostrar "Presione Espacio"

private bool waitingToStart = false;

    private int currentTrial = 0;
    private int[] trialOrder = new int[] { 0, 1, 2 }; // puede cambiar a [0,2,1] para versión B

    void Start()
    {
        uiManager.ShowForm();
	trialManager.egg.gameObject.SetActive(false);
	trialManager.pan.gameObject.SetActive(false);

    }

    public void StartGame()
    {
        currentTrial = 0;
        StartNextTrial();
    }

    void StartNextTrial()
    {
	trialManager.egg.gameObject.SetActive(true);
	trialManager.pan.gameObject.SetActive(true);
        if (currentTrial >= 3)
        {
            dataCollector.SaveAllTrials();
            uiManager.ShowEndScreen();
            return;
        }

        PanController.PanMode mode = (PanController.PanMode)trialOrder[currentTrial];
        trialManager.StartTrial(mode);
    }

public void EndTrial(int successes, int total)
{
    uiManager.ShowSurvey((a, b, c) =>
    {
        dataCollector.SaveTrial(currentTrial + 1, successes, total, a, b, c);

        currentTrial++;
        if (currentTrial >= 3)
        {
            dataCollector.SaveAllTrials();
            uiManager.ShowEndScreen();
        }
        else
        {
            StartNextTrial();
        }
    });
}
void Update()
{
    if (waitingToStart && Input.GetKeyDown(KeyCode.Space))
    {
        waitingToStart = false;
        startGamePanel.SetActive(false);
        StartGame();
    }
}
public void ShowEndScreen()
{
    uiManager.ShowEndScreen();
}


public void SetTotalRounds(int rounds)
{
    roundsPerTrial = rounds;
}
public void ShowStartGamePanel()
{
    startGamePanel.SetActive(true);
    waitingToStart = true;
}

    public void PlayWinSound() => audioSource.PlayOneShot(winSound);
    public void PlayLoseSound() => audioSource.PlayOneShot(loseSound);
}
