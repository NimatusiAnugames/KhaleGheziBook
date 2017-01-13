using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

//This script use for mnage all scenes:
//Nvigate between scenes
public class MapManager : MonoBehaviour
{
    public static MapManager Instance = null;

    #region List of Scene indices 
    private static int[] Indices =
{
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10
    };
    private static int curSceneIndex = 0; 
    #endregion

    //Current scene
    private Scene curScene;
    private MapBase map;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //Get current scene and Map class from it
        curScene = SceneManager.GetSceneAt(0);
        var obj = from o in curScene.GetRootGameObjects() where o.name == "Map" select o;
        map = obj.First().GetComponent<MapBase>();
    }

    //Load next scene
    public void Next()
    {
        curSceneIndex++;
        map.FadeIn(() =>
        {
            SceneManager.LoadScene(Indices[curSceneIndex]);
        });
    }

    //Load previuse scene
    public void Prev()
    {
        curSceneIndex--;
        map.FadeIn(() =>
        {
            SceneManager.LoadScene(Indices[curSceneIndex]);
        });
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(Indices[curSceneIndex]);
    }
}
