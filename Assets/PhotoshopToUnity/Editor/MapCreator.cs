using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class MapSegmentData
{
    public Sprite Sprite;
    public Vector2 Position;
    public Vector2 OrginalSize;
}
public class MapCreator : ScriptableObject
{
    [MenuItem("Tools/Photoshop To Unity/Create Map")]
    static void DoIt()
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
            Vector2 size = new Vector2(int.Parse(split[3]), int.Parse(split[4]));
            size.x -= position.x;
            size.y -= position.y;

            var sprites = from s in spriteList where s.name == name select s;
            Sprite sprite = sprites.First();
            position.x = ((position.x - mainWidth / 2) + sprite.rect.width / 2) / 100;
            position.y = ((mainHeight / 2 - position.y) - sprite.rect.height / 2) / 100;
            MapSegmentData segment = new MapSegmentData() { Sprite = sprite, Position = position,
            OrginalSize = size};
            segmentList[counter].Add(segment);
        } 
        #endregion

        //Create Map
        #region Create Map 
        GameObject map = new GameObject("Map");
        map.transform.position = Vector3.zero;
        map.transform.localScale = Vector3.one;
        float posZ = Camera.main.transform.position.z + 1 + stringData.Length * 0.1f;
        int layerOrder = 0;
        for (int i = segmentList.Keys.Count - 1; i >= 0; i--)
        {
            GameObject obj = new GameObject(mapLayerName[i]);
            obj.transform.parent = map.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            List<MapSegmentData> list = segmentList[i];
            for (int j = list.Count - 1; j >= 0; j--)
            {
                GameObject seg = new GameObject(list[j].Sprite.name);
                seg.transform.parent = obj.transform;
                seg.transform.localScale = Vector3.one;
                Vector3 pos = list[j].Position;
                if (list[j].OrginalSize.x > 2048 || list[j].OrginalSize.y > 2048)
                {
                    float r = list[j].OrginalSize.x / list[j].Sprite.rect.width;
                    seg.transform.localScale = Vector3.one * r;
                    pos.x += (list[j].OrginalSize.x - list[j].Sprite.rect.width) / 100 / 2;
                    pos.y -= (list[j].OrginalSize.y - list[j].Sprite.rect.height) / 100 / 2;
                }
                pos.z = posZ;
                posZ -= 0.1f;
                seg.transform.position = pos;
                SpriteRenderer sr = seg.AddComponent<SpriteRenderer>();
                sr.sortingOrder = layerOrder;
                layerOrder++;
                sr.sprite = list[j].Sprite;
            }
        } 
        #endregion

        EditorUtility.DisplayDialog("Info", "Successfully!", "OK", "");
    }
}