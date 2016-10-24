using UnityEngine;
using System.Collections;

public class InitialMenuController : MonoBehaviour
{
    private XonixInitialMenuModel initialMenuModel;

    void Start()
    {
        initialMenuModel = new XonixInitialMenuModel();
        DifficultyLevelLogic.OnSelectedDifficultyEvent += onDifficultyLevelSelected;
        DifficultyLevelLogic.OnSelectedExitEvent += onExitSelected;
        XonixInitialMenuModel.OnUnloadSceneEvent += onSceneUnloaded;
    }

    private void onSceneUnloaded()
    {
        DifficultyLevelLogic.OnSelectedDifficultyEvent -= onDifficultyLevelSelected;
        DifficultyLevelLogic.OnSelectedExitEvent -= onExitSelected;
        XonixInitialMenuModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }

    private void onDifficultyLevelSelected(EXonixDifficultyLevel selectedLevel)
    {
        initialMenuModel.onDifficultySelected(selectedLevel);
    }

    private void onExitSelected()
    {
        initialMenuModel.onExitSelected();
    }
}
