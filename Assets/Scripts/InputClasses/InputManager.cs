using UnityEngine;
using System.Collections.Generic;

//This class manage all inputs in game
public class InputManager : MonoBehaviour
{
    //Instance of this input manager
    private static InputManager instance = null;
    public static InputManager Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = new GameObject("InputManager");
                instance = obj.AddComponent<InputManager>();
            }
            return instance;
        }
    }

    //Main camera instance for calculate sceen position to world position
    private Camera mainCamera;

    //All input disables
    [HideInInspector]
    public bool NoAllowInput;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public bool InputClick
    {
        get 
        {
            if (NoAllowInput)
                return false;
            return Input.GetMouseButtonUp(0);
        }
    }
    public Vector3 InputWorldPostion
    {
        get
        {
            return mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
