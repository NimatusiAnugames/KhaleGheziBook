using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CompairsonSprites : ScriptableObject
{
    private struct ArtInfo
    {
        public Texture2D Texture;
        public string Path;
        public ArtInfo(Texture2D texture, string path)
        {
            Texture = texture;
            Path = path;
        }
    }
    [MenuItem("Tools/Photoshop To Unity/Compairson Sprites")]
    static void DoIt()
    {
        //Get all file names from SingleArts folder
        string[] names = Directory.GetFiles("Assets/PhotoshopToUnity/Resources/SingleArts");

        //List of arts for compairson
        List<ArtInfo> Arts = new List<ArtInfo>();

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
            Arts.Add(new ArtInfo(tex, names[i]));
        }

        #endregion

        //Load _Out.txt for modify
        string[] stringData = File.ReadAllLines("Assets/PhotoshopToUnity/Resources/AtlasOutput/_Out.txt");

        List<Texture2D> comTexs = new List<Texture2D>();
        int dirIndex = -1;
        string mainArtName = string.Empty;
        for (int i = 0; i < Arts.Count - 1; i++)
        {
            if (comTexs.Contains(Arts[i].Texture))
                continue;

            for (int j = i + 1; j < Arts.Count; j++)
            {
                Texture2D tex1 = Arts[i].Texture;
                Texture2D tex2 = Arts[j].Texture;
                if(tex1.width == tex2.width && tex1.height == tex2.height)
                {
                    Color[] color1 = tex1.GetPixels();
                    Color[] color2 = tex2.GetPixels();
                    bool result = true;
                    for (int c = 0; c < color1.Length; c++)
                    {
                        if(color1[c]!= color2[c])
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        if (dirIndex != i)
                        {
                            dirIndex = i;
                            mainArtName = Path.GetFileNameWithoutExtension(Arts[i].Path);
                            comTexs.Add(tex1);
                        }
                        for (int s = 0; s < stringData.Length; s++)
                        {
                            string str = stringData[s];
                            string name = Path.GetFileNameWithoutExtension(Arts[j].Path);
                            //string[] namesplit = name.Split('_');
                            string[] datasplit = str.Split(',');
                            if (name == datasplit[0])
                            {
                                datasplit[0] = mainArtName;
                                string newDataLineString = datasplit[0];
                                for (int k = 1; k < datasplit.Length; k++)
                                {
                                    newDataLineString += "," + datasplit[k];
                                }
                                stringData[s] = newDataLineString;
                                break;
                            }
                        }
                        File.Delete(Arts[j].Path);
                        comTexs.Add(tex2);
                    }
                }
            }
        }

        File.WriteAllLines("Assets/PhotoshopToUnity/Resources/AtlasOutput/_Out.txt", stringData); 

        AssetDatabase.Refresh();

        //OK...Done
        EditorUtility.DisplayDialog("Info", "Successfully!", "OK", "");
    }
}