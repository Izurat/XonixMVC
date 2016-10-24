using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class XonixInfoDisplayLogics : MonoBehaviour
{
    private Text text;

    void Start()
    {
        text =(GameObject.Find("infoTextString").GetComponent<Text>());
        XonixLifeCycleModel.OnInfoUpdatedEvent += onInfoUpdated;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded; ;
    }
    private void onSceneUnloaded()
    {
        XonixLifeCycleModel.OnInfoUpdatedEvent -= onInfoUpdated;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }
    private void onInfoUpdated(int currentLifesCount,int initialLifesCount,int scoresValue,float levelPartLeft)
    {
        text.text = "Lifes count " + currentLifesCount + "/" + initialLifesCount + " scores " + scoresValue + " level part left " + Math.Round(levelPartLeft, 2) + " tap two fingers for menu";
    }

}
