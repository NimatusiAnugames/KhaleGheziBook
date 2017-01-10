using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AtlasData
{
    public Texture2D Atlas;
    public SortedList<string, Rect> RectList;
    public AtlasData(Texture2D atlas)
    {
        Atlas = atlas;
        RectList = new SortedList<string, Rect>();
    }
}
public class AtlasCreator : ScriptableObject
{
    //Atlas properties
    private const int atlasWidth = 2048;
    private const int atlasHeight = 2048;
    private const int gridSize = 32;

    //Main method that create atlases
    [MenuItem("Tools/Photoshop To Unity/Create Atlas")]
    static void DoIt()
    {
        //Get all file names from SingleArts folder
        string[] names = Directory.GetFiles("Assets/PhotoshopToUnity/Resources/SingleArts");

        //List of normal arts that less from (atlasWidth by atlasHeight)
        List<Texture2D> normalArts = new List<Texture2D>();

        //lits of big arts that great from (atlasWidth by atlasHeight)
        List<Texture2D> bigArts = new List<Texture2D>();

        //Create a grid for sector atlas
        byte[,] grid = new byte[(atlasWidth / gridSize) , (atlasHeight / gridSize)];
 
        //List of output atlases
        List<AtlasData> atlasList = new List<AtlasData>();
        AtlasData atlas = new AtlasData(CreateEmptyAtlas(atlasWidth, atlasHeight));
        atlasList.Add(atlas);

        #region Load textures in lists 
        for (int i = 0; i < names.Length; i++)
        {
            string name = Path.GetFileNameWithoutExtension(names[i]);
            //name must be an integer id
            //if (name.Contains("_") || name.Contains(".png"))
            //    continue;
            if (name.Contains(".png"))
                continue;

            Texture2D tex = Resources.Load<Texture2D>("SingleArts/" +
                name);
            if (tex.width > atlasWidth || tex.height > atlasHeight)
                bigArts.Add(tex);
            else
                normalArts.Add(tex);
        }
        #endregion
 
        #region Create (atlasWidth by atlasHeight) atlases 
        do
        {
            for (int i = 0; i < normalArts.Count; i++)
            {
                Texture2D art = normalArts[i];
                int gridWidth = atlasWidth / gridSize, gridHeight = atlasHeight / gridSize;
                int artGridWidth = art.width / gridSize + (art.width % gridSize > 0 ? 1 : 0);
                int artGridHeight = art.height / gridSize + (art.height % gridSize > 0 ? 1 : 0);
                bool set = false;
                for (int h = 0; h <= gridHeight - artGridHeight; h++)
                {
                    for (int w = 0; w <= gridWidth - artGridWidth; w++)
                    {
                        if (CheckGrid(ref grid, w, h, artGridWidth, artGridHeight))
                        {
                            atlas.Atlas.SetPixels
                                (w * gridSize, h * gridSize, art.width, art.height, art.GetPixels());
                            atlas.RectList.Add(art.name, 
                                new Rect(w * gridSize, h * gridSize, art.width, art.height));
                            normalArts.Remove(art);
                            i--;
                            set = true;
                            break;
                        }
                    }
                    if (set)
                        break;
                }
            }
            if (normalArts.Count > 0)
            {
                atlas = new AtlasData(CreateEmptyAtlas(atlasWidth, atlasHeight));
                atlasList.Add(atlas);
                for (int i = 0; i < atlasWidth / gridSize; i++)
                {
                    for (int j = 0; j < atlasHeight / gridSize; j++)
                    {
                        grid[i, j] = 0;
                    }
                }
            }
        } while (normalArts.Count > 0); 
        #endregion

        #region Save stlases 
        for (int i = 0; i < atlasList.Count; i++)
        {
            atlasList[i].Atlas.Apply();
            File.WriteAllBytes("Assets/PhotoshopToUnity/Resources/AtlasOutput/atlas" + (i + 1) + ".png",
                atlasList[i].Atlas.EncodeToPNG());
        }

        //Save big arts
        for (int i = 0; i < bigArts.Count; i++)
        {
            int w = bigArts[i].width;
            int h = bigArts[i].height;
            if (w % 4 != 0)
                w += 4 - (w % 4);
            if (h % 4 != 0)
                h += 4 - (h % 4);
            Texture2D tex = CreateEmptyAtlas(w, h);//new Texture2D(w, h);
            tex.SetPixels(0, 0, bigArts[i].width, bigArts[i].height, bigArts[i].GetPixels());
            tex.Apply();
            File.WriteAllBytes("Assets/PhotoshopToUnity/Resources/AtlasOutput/" + bigArts[i].name + ".png",
                tex.EncodeToPNG());
        }

        AssetDatabase.Refresh(); 
        #endregion

        #region Slice atlases 
        for (int i = 0; i < atlasList.Count; i++)
        {
            Texture2D tex = Resources.Load<Texture2D>("AtlasOutput/atlas" + (i + 1));
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.mipmapEnabled = false;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            List<SpriteMetaData> newData = new List<SpriteMetaData>();
            for (int s = 0; s < atlasList[i].RectList.Keys.Count; s++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = 9;
                smd.name = atlasList[i].RectList.Keys[s];
                smd.rect = atlasList[i].RectList.Values[s];
                newData.Add(smd);
            }
            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path);
        } 
        #endregion

        AssetDatabase.Refresh();

        //OK...Done
        EditorUtility.DisplayDialog("Info", "Successfully!", "OK", "");
    }

    //Helpers
    static Texture2D CreateEmptyAtlas(int width, int height)
    {
        Texture2D atlas = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0, 0, 0, 0);
        }
        atlas.SetPixels(colors);
        return atlas;
    }
    static bool CheckGrid(ref byte[,] grid, int w, int h, int artW, int artH)
    {
        #region Check for grid is fill or not 
        for (int i = h; i < h + artH; i++)
        {
            for (int j = w; j < w + artW; j++)
            {
                if (grid[j, i] == 1)
                {
                    return false;
                }
            }
        } 
        #endregion

        #region Fill grid 
        for (int i = h; i < h + artH; i++)
        {
            for (int j = w; j < w + artW; j++)
            {
                grid[j, i] = 1;
            }
        } 
        #endregion

        return true;
    }
}