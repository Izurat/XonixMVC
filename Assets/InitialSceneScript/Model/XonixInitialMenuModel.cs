using UnityEngine;
using UnityEngine.SceneManagement;

public enum EXonixDifficultyLevel
{
    easy,medium,hard
}

public class XonixInitialMenuModel {

    public static int initialEnemysCount { get; private set; }

    public static float levelPartToRemove { get; private set; }

    public static float stepTime { get; private set; }

    public static int levelTime { get; private set; }

    public static int scoresMultiplyer{ get; private set; }

    public delegate void OnUnloadScene();
    public static event OnUnloadScene OnUnloadSceneEvent = delegate { };

    public void onDifficultySelected(EXonixDifficultyLevel difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case EXonixDifficultyLevel.easy:
                initialEnemysCount = 1;
                levelPartToRemove = 0.25f;
                stepTime = 0.1f;
                levelTime = 60;
                scoresMultiplyer = 1;
                break;
            case EXonixDifficultyLevel.medium:
                initialEnemysCount = 2;
                levelPartToRemove = 0.17f;
                stepTime = 0.07f;
                levelTime = 40;
                scoresMultiplyer = 3;
                break;
            case EXonixDifficultyLevel.hard:
                initialEnemysCount = 4;
                levelPartToRemove = 0.12f;
                stepTime = 0.05f;
                levelTime = 20;
                scoresMultiplyer = 10;
                break;
        }
        
        OnUnloadSceneEvent();
        SceneManager.LoadScene("xonixScene1"/*, LoadSceneMode.Additive*/);
    }

    public void onExitSelected()
    {
        Application.Quit();
    }

}
