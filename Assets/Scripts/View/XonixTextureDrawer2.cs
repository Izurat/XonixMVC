using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XonixTextureDrawer2 : MonoBehaviour {
    Color WaterColor = Color.cyan;
    Color GroundColor = Color.black;
    Color EnemyColor = Color.red;
    Color PathColor = Color.magenta;
    Color PlayerColor = Color.yellow;

    private Image Display;

    public int cellSize = 2;

    void Start()
    {
        Display = (GameObject.Find("GameDisplay").GetComponent<Image>());
        XonixMainModel.OnRedrawEvent += redraw;
        XonixMainModel.OnUpdateComleteEvent+=displayChanges;
        XonixFieldModel.OnEnemyPositionUpdatedEvent += drawEnemy;
        XonixFieldModel.OnPlayerPositionUpdatedEvent+=drawPlayer;
        XonixFieldModel.OnFieldElementUpdatedEvent += redrawFieldElement;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded; ;
    }

    private void onSceneUnloaded()
    {
        XonixMainModel.OnRedrawEvent -= redraw;
        XonixMainModel.OnUpdateComleteEvent -= displayChanges;
        XonixFieldModel.OnEnemyPositionUpdatedEvent -= drawEnemy;
        XonixFieldModel.OnPlayerPositionUpdatedEvent -= drawPlayer;
        XonixFieldModel.OnFieldElementUpdatedEvent -= redrawFieldElement;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }



    public void redrawFieldElement(XonixFieldElement fieldElement)
    {
        Texture2D fieldTexture = Display.overrideSprite.texture;
        Color pixelToDraw = Color.black;
        bool isPainting = true;
        switch (fieldElement.CurrentState)
        {
            case EFieldCellState.Ground:
                pixelToDraw = GroundColor;
                break;
            case EFieldCellState.Path:
                pixelToDraw = PathColor;
                break;
            case EFieldCellState.Sea:
                pixelToDraw = WaterColor;
                break;
            default:
                isPainting = false;
                break;
        }
        if (isPainting)
        {
            for (int x = cellSize * (int)(fieldElement.Position.x); x < cellSize * ((int)(fieldElement.Position.x) + 1); x++)
            {
                for (int y = cellSize * (int)(fieldElement.Position.y); y< cellSize * ((int)(fieldElement.Position.y) + 1); y++)
                {
                    fieldTexture.SetPixel(x, y, pixelToDraw);
                }
            }
        }
    }

    public void drawEnemy(XonixEnemy currenemy,XonixFieldElement leavedElement)
    {
        drawEnemyInPosition(currenemy);
        redrawFieldElement(leavedElement);
    }

    private void drawEnemyInPosition(XonixEnemy enemy)
    {
        Texture2D fieldTexture = Display.overrideSprite.texture;
        for (int x = cellSize * (int)(enemy.currentPosition.x); x < cellSize * ((int)(enemy.currentPosition.x) + 1); x++)
        {
            for (int y = cellSize * (int)(enemy.currentPosition.y); y < cellSize * ((int)(enemy.currentPosition.y) + 1); y++)
            {
                fieldTexture.SetPixel(x, y, EnemyColor);
            }
        }
    }

    public void drawPlayer(int playerX,int playerY,XonixFieldElement leavedElement)
    {
        redrawFieldElement(leavedElement);
        drawPlayerInPosition(playerX, playerY);
    }

    private void drawPlayerInPosition(int playerX, int playerY)
    {
        Texture2D fieldTexture = Display.overrideSprite.texture;
        for (int x = cellSize * playerX; x < cellSize * (playerX + 1); x++)
        {
            for (int y = cellSize * playerY; y < cellSize * (playerY + 1); y++)
            {
                fieldTexture.SetPixel(x, y, PlayerColor);
            }
        }
    }

    public void redraw(XonixFieldElement[,] fieldArray,int fieldSizeX,int fieldSizeY,List<XonixEnemy> enemys, int playerX, int playerY)
    {
        Texture2D fieldTexture = new Texture2D(fieldSizeX * cellSize, fieldSizeY * cellSize, TextureFormat.ARGB32, false);
        fieldTexture.filterMode = FilterMode.Point;
        Display.overrideSprite = Sprite.Create(fieldTexture, Display.sprite.rect, Display.sprite.pivot);
        XonixFieldElement currentXonixFieldElement;
        for (int i = 0; i < fieldSizeX; i++)
        {
            for (int j = 0; j < fieldSizeY; j++)
            {
                currentXonixFieldElement = fieldArray[i, j];
                redrawFieldElement(currentXonixFieldElement);
            }
        }
        foreach (XonixEnemy currenemy in enemys)
        {
            drawEnemyInPosition(currenemy);
        }
        drawPlayerInPosition(playerX, playerY);
        displayChanges();
    }

    public void displayChanges()
    {
        Texture2D fieldTexture = Display.overrideSprite.texture;
        fieldTexture.Apply();
    }
}
