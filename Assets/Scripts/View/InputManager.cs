using UnityEngine;

public class InputManager : MonoBehaviour {

    Vector2 fingerStart = new Vector2();
    Vector2 fingerEnd = new Vector2();
    int swipeSize = 20;

    public delegate void OnChangeDirectionInput(EPlayerMovementInputType directionInput);
    public static event OnChangeDirectionInput OnChangeDirectionInputEvent;

    public delegate void OnTogglePause();
    public static event OnTogglePause OnTogglePauseEvent = delegate{};

    void Update ()
    {
        EPlayerMovementInputType currentInput = updatePlayerMovementInputType();
        if (currentInput != EPlayerMovementInputType.NoChange)
        {
            OnChangeDirectionInputEvent(currentInput);
            return;

        }
        currentInput = updatePlayerMovementInputTypeTouch();
        if (currentInput != EPlayerMovementInputType.NoChange)
        {
            OnChangeDirectionInputEvent(currentInput);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            togglePause();
        }

        if (Input.touches.Length >= 2)
        {
            if (!isMultipleTouch)
            {
                isMultipleTouch = true;
                togglePause();
            }
        }
        else
        {
            isMultipleTouch = false;
        }
    }

    private void togglePause()
    {
        OnTogglePauseEvent();
    }


    private bool isMultipleTouch = false;
    private EPlayerMovementInputType updatePlayerMovementInputTypeTouch()
    {
        if (isMultipleTouch)
        {
            if (Input.touches.Length == 0)
            {
                isMultipleTouch = false;
            }
            return EPlayerMovementInputType.NoChange;
        }
        EPlayerMovementInputType res = EPlayerMovementInputType.NoChange;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerStart = touch.position;
                fingerEnd = touch.position;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                fingerEnd = touch.position;
                int dx = (int)(fingerStart.x) - (int)(fingerEnd.x);
                int dy = (int)(fingerStart.y) - (int)(fingerEnd.y);
                if (Mathf.Abs(dx) > Mathf.Abs(dy) && Mathf.Abs(dx) > swipeSize)
                {
                    if (dx < 0)
                    {
                        res = EPlayerMovementInputType.Right;
                    }
                    else
                    {
                        res = EPlayerMovementInputType.Left;
                    }
                }
                else if (Mathf.Abs(dy) > swipeSize)
                {
                    if (dy < 0)
                    {
                        res = EPlayerMovementInputType.Up;
                    }
                    else
                    {
                        res = EPlayerMovementInputType.Down;
                    }
                }
                fingerStart = fingerEnd;
            }
            if (touch.phase == TouchPhase.Ended)
            {

                fingerStart = Vector2.zero;
                fingerEnd = Vector2.zero;
            }
        }
        return res;
    }

    private EPlayerMovementInputType updatePlayerMovementInputType()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return EPlayerMovementInputType.Left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            return EPlayerMovementInputType.Right;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            return EPlayerMovementInputType.Up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            return EPlayerMovementInputType.Down;
        }
        return EPlayerMovementInputType.NoChange;
    }
}
