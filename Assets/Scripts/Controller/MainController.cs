using System;
using UnityEngine;
using System.Collections;

public class MainController:MonoBehaviour
{
    private XonixMainModel mainModel;
    private XonixTimeModel timeModel;
    void Start ()
	{
	    InputManager.OnChangeDirectionInputEvent += onInputDirectionChanged;
        XonixGuiLogics.OnPauseStateChangedEvent += onPause;
        XonixGuiLogics.OnExitToMainMenuEvent += onExitToMainMenuSelected;
        XonixGuiLogics.OnExitGameEvent += onExitTGameSelected;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded;

        mainModel = new XonixMainModel();
        timeModel = gameObject.GetComponent<XonixTimeModel>();
        mainModel.Init();
        timeModel.init();
        
    }

    private void onSceneUnloaded()
    {
        InputManager.OnChangeDirectionInputEvent -= onInputDirectionChanged;
        XonixGuiLogics.OnPauseStateChangedEvent -= onPause;
        XonixGuiLogics.OnExitToMainMenuEvent -= onExitToMainMenuSelected;
        XonixGuiLogics.OnExitGameEvent -= onExitTGameSelected;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }

    private void onInputDirectionChanged(EPlayerMovementInputType inputDirectionValue)
    {
        mainModel.notifyPlayerDirection(inputDirectionValue);
    }

    private void onPause(Boolean pauseState)
    {
        timeModel.onTogglePause(pauseState);
    }

    private void onExitToMainMenuSelected()
    {
        mainModel.exitToMainMenu();
    }
    private void onExitTGameSelected()
    {
        mainModel.exitGame();
    }
}
