using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class UICreatorWindow : EditorWindow
{
    [MenuItem("Tools/Photoshop To Unity/Create UI")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(UICreatorWindow), true, "Create UI");
    }

    private static string uiSizeText = "1";

    private void OnGUI()
    {
        //Fixed size window
        minSize = maxSize = new Vector2(200, 120);

        //Window ui
        GUI.Label(new Rect(20, 20, 50, 20), "UI Size: ");
        uiSizeText = GUI.TextField(new Rect(70, 20, 100, 20), uiSizeText);
        float uiSize = 1.0f;
        float.TryParse(uiSizeText, out uiSize);
        if (GUI.Button(new Rect((200 - 70) / 2, 70, 70, 20), "OK"))
        {
            OK(uiSize);
            Close();
        }
    }
    private void OK(float uiSize)
    {
        #region Load Atlases and Sprites 
        string[] names = Directory.GetFiles("Assets/PhotoshopToUnity/Resources/AtlasOutput");
        List<Sprite> spriteList = new List<Sprite>();
        for (int i = 0; i < names.Length; i++)
        {
            string name = Path.GetFileNameWithoutExtension(names[i]);
            if (name.Contains(".png"))
                continue;

            Texture2D atlas = Resources.Load<Texture2D>("AtlasOutput/" + name);
            string atlasPath = AssetDatabase.GetAssetPath(atlas);
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(atlasPath).OfType<Sprite>().ToArray();
            spriteList.AddRange(sprites);
        }
        #endregion

        #region Read Photoshop _Out.txt file 
        List<string> mapLayerName = new List<string>();
        string[] stringData = File.ReadAllLines("Assets/PhotoshopToUnity/Resources/AtlasOutput/_Out.txt");
        string[] split = stringData[0].Split(',');
        int mainWidth = int.Parse(split[1]);
        int mainHeight = int.Parse(split[2]);
        SortedList<int, List<MapSegmentData>> segmentList = new SortedList<int, List<MapSegmentData>>();
        int counter = -1;
        for (int i = 1; i < stringData.Length; i++)
        {
            if (stringData[i].StartsWith("LayerSet"))
            {
                counter++;
                segmentList.Add(counter, new List<MapSegmentData>());
                mapLayerName.Add(stringData[i].Split(',')[1]);
                continue;
            }

            split = stringData[i].Split(',');
            string name = split[0];
            //if (name.Contains("_"))
            //    name = name.Split('_')[0];
            Vector2 position = new Vector2(int.Parse(split[1]), int.Parse(split[2]));
            var sprites = from s in spriteList where s.name == name select s;
            Sprite sprite = sprites.First();
            position.x = ((position.x - mainWidth / 2) + sprite.rect.width / 2) / uiSize;
            position.y = ((mainHeight / 2 - position.y) - sprite.rect.height / 2) / uiSize;
            MapSegmentData segment = new MapSegmentData() { Sprite = sprite, Position = position };
            segmentList[counter].Add(segment);
        }
        #endregion

        #region Create Map 
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject map = new GameObject("UI");
        map.transform.parent = canvas.transform;
        map.transform.localPosition = Vector3.zero;
        map.transform.localScale = Vector3.one;
        RectTransform rt = map.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Screen.width, Screen.height);
        for (int i = segmentList.Keys.Count - 1; i >= 0; i--)
        {
            GameObject obj = new GameObject(mapLayerName[i]);
            obj.transform.parent = map.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            rt = obj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Screen.width, Screen.height);
            List<MapSegmentData> list = segmentList[i];
            for (int j = list.Count - 1; j >= 0; j--)
            {
                GameObject seg = new GameObject(list[j].Sprite.name);
                seg.transform.parent = obj.transform;
                seg.transform.localScale = Vector3.one;
                Vector3 pos = list[j].Position;
                seg.transform.localPosition = pos;
                Image img = seg.AddComponent<Image>();
                img.sprite = list[j].Sprite;
                rt = (RectTransform)seg.transform;
                rt.sizeDelta = new Vector2(img.sprite.rect.width / uiSize, img.sprite.rect.height / uiSize);
            }
        }
        #endregion

        EditorUtility.DisplayDialog("Info", "Successfully!", "OK", "");
    }
}