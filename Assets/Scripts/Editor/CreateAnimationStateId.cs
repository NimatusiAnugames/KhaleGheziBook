using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateAnimationStateId : ScriptableObject
{
    private struct FieldInformation
    {
        public string ClipName;
        public string Name;
        public int ID;
    }

    private const string RegionCode =
    @"
    #region <RegionName>
    <CodePlace>
    #endregion";
    private const string CodeLineInt =
    @"
    public const int ";
    private const string CodeLineString =
    @"
    public const string ";



    [MenuItem("Tools/Create Animation IDs")]
    static void DoIt()
    {
        string[] data = System.IO.File.ReadAllLines("Assets/Scripts/AnimationClasses/AnimConsts.cs");

        //GameObject[] objects = Selection.gameObjects;
        //foreach (var item in objects)
        //{
        //    CreateCode(item, ref data);
        //}

        CreateCode(Selection.activeGameObject, ref data);

        System.IO.File.WriteAllLines("Assets/Scripts/AnimationClasses/AnimConsts.cs", data);

        EditorUtility.DisplayDialog("Info", "Successful !", "OK", "");
    }
    private static void CreateCode(GameObject obj, ref string[] data)
    {
        if (!obj)
            return;

        Animator animator = obj.GetComponent<Animator>();
        if (!animator)
            return;
        AnimationClip[] clips = AnimationUtility.GetAnimationClips(obj);
        AnimStateBehaviour[] anims = animator.GetBehaviours<AnimStateBehaviour>();
        List<FieldInformation> fields = new List<FieldInformation>();
        for (int c = 0; c < clips.Length; c++)
        {
            string name = clips[c].name.Replace(' ', '_');
            string clipname = name;
            name = obj.name + "_" + name;
            bool isInCode = false;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Contains(name))
                    isInCode = true;
            }
            if (!isInCode)
                fields.Add(new FieldInformation() { Name = name, ID = anims[c].ID, ClipName = clipname});
        }
        int index = 0;
        bool found = false;
        string regionName = obj.name + "States";
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].Contains("{"))
                index = i;
            if (data[i].Contains(regionName))
            {
                index = i;
                found = true;
                break;
            }
        }
        string codes = string.Empty;
        for (int i = 0; i < fields.Count; i++)
        {
            codes += "\r\n" + CodeLineInt + fields[i].Name + " = " + fields[i].ID.ToString() + ";";
            codes += "\r\n" + CodeLineString + fields[i].Name + "Clip = " + "\"" + fields[i].ClipName + "\"" + ";";
        }
        if (found)
        {
            data[index] += codes;
        }
        else
        {
            codes += "\r\n";
            string newRegion = RegionCode;
            newRegion = newRegion.Replace("<RegionName>", regionName);
            newRegion = newRegion.Replace("<CodePlace>", codes);
            data[index] += newRegion;
        }
    }
}
