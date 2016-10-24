using UnityEngine;

public class DifficultyLevelLogic : MonoBehaviour {

    public delegate void OnSelectedDifficulty(EXonixDifficultyLevel selectedLevel);
    public static event OnSelectedDifficulty OnSelectedDifficultyEvent = delegate { };

    public delegate void OnSelectedExit();
    public static event OnSelectedExit OnSelectedExitEvent = delegate { };

    public void selectEasy()
    {
        OnSelectedDifficultyEvent(EXonixDifficultyLevel.easy);
    }

    public void selectMedium()
    {
        OnSelectedDifficultyEvent(EXonixDifficultyLevel.medium);
    }

    public void selectHard()
    {
        OnSelectedDifficultyEvent(EXonixDifficultyLevel.hard);
    }

    public void selectExit()
    {
        OnSelectedExitEvent();
    }
}
