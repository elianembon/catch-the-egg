using UnityEngine;

public class TrialManager : MonoBehaviour
{
    public EggController egg;
    public PanController pan;
    public int totalRounds = 20;
    private int currentRound = 0;
    private int successfulCatches = 0;

    public GameManager gameManager;
public void StartTrial(PanController.PanMode panMode)
{
    currentRound = 0;
    successfulCatches = 0;
    pan.SetMode(panMode);
    totalRounds = FindAnyObjectByType<GameManager>().roundsPerTrial;
    NextRound();
}


   public void NextRound()
{
    currentRound++;
    if (currentRound > totalRounds)
    {
        gameManager.EndTrial(successfulCatches, totalRounds);
        return;
    }

    egg.Unfreeze();
    pan.Unfreeze();

    pan.transform.position = new Vector3(0f, pan.transform.position.y, pan.transform.position.z);

    egg.StartRound(currentRound);
    pan.SetCurrentRound(currentRound, totalRounds);
}



    public void RoundSuccess()
    {
        successfulCatches++;
        egg.Freeze();
        pan.Freeze();
        gameManager.PlayWinSound();
        Invoke(nameof(NextRound), 1f);
    }

    public void RoundFail()
    {
        pan.Freeze();
        gameManager.PlayLoseSound();
        Invoke(nameof(NextRound), 1f);
    }
}
