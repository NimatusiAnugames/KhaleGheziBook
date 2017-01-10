using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ScriptDiagram : EditorWindow 
{
    #region Fields 

    //Pathes
    private const string AssetPath = @"Assets/Scripts/";
    private const string AssemblyPath = @"Library/ScriptAssemblies/Assembly-CSharp.dll";
    private string curPathText = "Scripts";
    private string PathText = "Scripts";

    //Code templates
    private string[] Mono_Template = 
    { 
      "using UnityEngine;", 
      "using System.Collections.Generic;",
      "",
      "public class <name> : MonoBehaviour",
      "{",
      "",
      "\t// Use this for initialization",
      "\tvoid Start()",
      "\t{",
      "",
      "\t}",
      "",
      "\t// Update is called once per frame",
      "\tvoid Update()",
      "\t{",
      "",
      "\t}",
      "}"
    };

    //Textures
    private static Texture FolderTex = null;
    private static Texture ScriptTex = null;
    private static Texture TopBarTex = null;
    private static Texture MenuButtonOverTex = null;
    private static Texture AddressTextboxTex = null;
    private static Texture SelectedScriptTex = null;
    private static Texture SelectedFolderTex = null;
    private static Texture BackButtonOverTex = null;
    private static GUIStyle LabelStyle = null;

    //Menu buttons
    private static DiagramMenuButton NewFolderButton,
                              NewScriptButton,
                              CompileButton,
                              FoldersViewButton,
                              ScriptsViewButton,
                              BackButton,
                              HomeButton;

    //Graph objects list
    private static int _id = 0;
    private static int ID_Increase
    {
        get
        {
            return _id++;
        }
    }
    private static Dictionary<int, GraphFolder> Graph;
    private static int CurFolderID = 0;
    private static List<GraphFolder> NewFolders = new List<GraphFolder>();

    //Drag
    private Event curEvent;
    private bool mouseDown = false;
    private Vector2 dragOffset;
    private GraphNodeBase curNode = null;
    private int gridSize = 5;

    //Double click
    private bool doubleClick = false;

    //Scroll
    private Vector2 scroll = Vector2.zero;
    private bool scrolling = false;

    //Zoom
    private float zoom = 1f; //zoom = 100%

    //Dialog boxes
    private bool newFolderDialogBox = false;
    private Rect newfolderRect = new Rect(100, 100, 200, 150);
    private string folderName = "";
    private bool newScriptDialogBox = false;
    private Rect newScriptRect = new Rect(100, 100, 200, 150);
    private string scriptName = "";

    //Relationships
    private bool dragInheritanceLine;
    private bool dragFieldLine;
    private GraphScript curScriptLine; 

    //All script view system
    private static bool IsAllScriptView;
    private static string viewButtonName;
    private static int lastFolderID;

    #endregion

    //Init
    private static bool IsInit = false;
    [MenuItem("Nita Game/Script Diagram")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(ScriptDiagram));

        Loading();
    }
    private static void Loading()
    {
        //Load textures from resources folder
        if (FolderTex == null)
            FolderTex = Resources.Load<Texture>("folder");
        if (ScriptTex == null)
            ScriptTex = Resources.Load<Texture>("script");
        if (TopBarTex == null)
            TopBarTex = Resources.Load<Texture>("topbar");
        if(MenuButtonOverTex == null)
            MenuButtonOverTex = Resources.Load<Texture>("newfolderover");
        if (AddressTextboxTex == null)
            AddressTextboxTex = Resources.Load<Texture>("textbox");
        if(SelectedScriptTex == null)
            SelectedScriptTex = Resources.Load<Texture>("selectedscript");
        if(SelectedFolderTex == null)
            SelectedFolderTex = Resources.Load<Texture>("selectedfolder");
        if(BackButtonOverTex == null)
            BackButtonOverTex = Resources.Load<Texture>("backbuttonover");

        //Menu Buttons
        NewFolderButton = new DiagramMenuButton(new Vector2(0, 0),
            Resources.Load<Texture>("newfolder"), MenuButtonOverTex);
        NewScriptButton = new DiagramMenuButton(new Vector2(NewFolderButton.rect.width + 2, 0),
            Resources.Load<Texture>("newscript"), MenuButtonOverTex);
        FoldersViewButton = new DiagramMenuButton(new Vector2((NewScriptButton.rect.width + 2) * 2, 0),
            Resources.Load<Texture>("folderview"), MenuButtonOverTex);
        ScriptsViewButton = new DiagramMenuButton(new Vector2((NewScriptButton.rect.width + 2) * 2, 0),
            Resources.Load<Texture>("scriptview"), MenuButtonOverTex);
        CompileButton = new DiagramMenuButton(new Vector2((NewScriptButton.rect.width + 2) * 3, 0),
            Resources.Load<Texture>("compile"), MenuButtonOverTex);
        BackButton = new DiagramMenuButton(new Vector2(CompileButton.rect.x + CompileButton.rect.width + 50,
            (int)(TopBarTex.height - 38) / 2 - 4),
            Resources.Load<Texture>("backbutton"), BackButtonOverTex);
        HomeButton = new DiagramMenuButton(new Vector2(10, TopBarTex.height + 3),
            Resources.Load<Texture>("homebutton"), BackButtonOverTex);

        //Label style
        if (LabelStyle == null)
        {
            LabelStyle = new GUIStyle();
            LabelStyle.normal.textColor = Color.white;
            LabelStyle.fontSize = 16;
            //LabelStyle.fontStyle = FontStyle.Bold;
        }

        //Load assembly types
        System.Type[] types = null;
        if (File.Exists(AssemblyPath))
        {
            Assembly assembly = Assembly.LoadFile(AssemblyPath);
            types = assembly.GetTypes();
        }
        else
        {
            Debug.LogError("Assembly path not found!");
        }

        //Load locations
        List<FolderLocation> fLoc = null;
        List<ScriptLocation> sLoc = null;
        if (File.Exists(@"Assets/ScriptDiagram/data.dat"))
        {
            fLoc = new List<FolderLocation>();
            sLoc = new List<ScriptLocation>();
            string[] data = File.ReadAllLines(@"Assets/ScriptDiagram/data.dat");
            int nodeType = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].StartsWith("#"))
                {
                    nodeType++;
                    continue;
                }

                //int count = 0;
                string[] split = null;
                if (nodeType == 0)
                {
                    if (data[i].StartsWith("Count"))
                        //count = int.Parse(data[i].Split(':')[1]);
                        continue;
                    else
                    {
                        split = data[i].Split(',');
                        Vector2 loc = new Vector2(float.Parse(split[2]), float.Parse(split[3]));
                        FolderLocation fl = new FolderLocation(split[0], split[1], loc);
                        fLoc.Add(fl);
                    }
                }
                else if (nodeType == 1)
                {
                    if (data[i].StartsWith("Count"))
                        //count = int.Parse(data[i].Split(':')[1]);
                        continue;
                    else
                    {
                        split = data[i].Split(',');
                        Vector2 loc1 = new Vector2(float.Parse(split[1]), float.Parse(split[2]));
                        Vector2 loc2 = new Vector2(float.Parse(split[3]), float.Parse(split[4]));
                        ScriptLocation sl = new ScriptLocation(split[0], loc1, loc2);
                        sLoc.Add(sl);
                    }
                }
            }
        }

        NewFolders.Clear();

        //Create Scripts folder if not exist
        if (!Directory.Exists(AssetPath))
        {
            Directory.CreateDirectory(AssetPath);
            AssetDatabase.Refresh();
        }

        #region Load folders 
        //Load folders
        _id = 0;
        CurFolderID = 0;
        Graph = new Dictionary<int, GraphFolder>();
        Graph.Add(ID_Increase, new GraphFolder(0, null, "Root", Vector2.zero, -1));
        Graph[0].FullPath = AssetPath.Substring(0, AssetPath.Length - 1);
        DirectoryInfo di = new DirectoryInfo(AssetPath);
        foreach (var folder in di.GetDirectories())
        {
            GraphFolder newFolder = new GraphFolder(ID_Increase, FolderTex, folder.Name,
                Vector2.zero, 0);
            newFolder.FullPath = AssetPath + folder.Name;
            if (fLoc != null && fLoc.Count > 0)
            {
                try
                {
                    var fl = fLoc.First(f => f.Path == newFolder.FullPath);
                    newFolder.Location = fl.Location;
                }
                catch { }
            }
            Graph[0].Add(newFolder);
            Graph.Add(newFolder.ID, newFolder);
        }
        for (int i = 1; i < Graph.Keys.Count; i++)
        {
            int id = Graph.Keys.ElementAt(i);
            string name = AssetPath + GetPath(Graph[id].ParentID) + Graph[id].Name;
            di = new DirectoryInfo(name);
            foreach (var folder in di.GetDirectories())
            {
                GraphFolder newFolder = new GraphFolder(ID_Increase, FolderTex, folder.Name,
                Vector2.zero, Graph[id].ID);
                newFolder.FullPath = name + "/" + newFolder.Name;
                if (fLoc != null && fLoc.Count > 0)
                {
                    var fl = fLoc.First(f => f.Path == newFolder.FullPath);
                    newFolder.Location = fl.Location;
                }
                Graph[id].Add(newFolder);
                Graph.Add(newFolder.ID, newFolder);
            }
        } 
        #endregion

        #region Load scripts 
        //Load scripts
        di = new DirectoryInfo(AssetPath);
        foreach (var file in di.GetFiles())
        {
            if (Path.GetExtension(file.Name) != ".cs")
                continue;

            string name = Path.GetFileNameWithoutExtension(file.Name);
            System.Type ty = null;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Name == name)
                {
                    ty = types[i];
                    break;
                }
            }
            if (ty != null)
            {
                GraphScript newScript = new GraphScript(ScriptTex, name, Vector2.zero, 0, ty, ty.BaseType);
                if (sLoc != null && sLoc.Count > 0)
                {
                    try
                    {
                        var sl = sLoc.First(s => s.Name == newScript.Name);
                        newScript.Location = sl.Location1;
                        newScript.Location2 = sl.Location2;
                    }
                    catch { }
                }
                newScript.Read(file.FullName);
                Graph[0].Add(newScript); 
            }
        }
        for (int i = 1; i < Graph.Count; i++)
        {
            int id = Graph.Keys.ElementAt(i);
            string str = AssetPath + GetPath(Graph[id].ParentID) + Graph[id].Name;
            di = new DirectoryInfo(str);
            foreach (var file in di.GetFiles())
            {
                if (Path.GetExtension(file.Name) != ".cs")
                    continue;

                string name = Path.GetFileNameWithoutExtension(file.Name);
                System.Type ty = null;
                for (int j = 0; j < types.Length; j++)
                {
                    if (types[j].Name == name)
                    {
                        ty = types[j];
                        break;
                    }
                }
                if (ty != null)
                {
                    GraphScript newScript = new GraphScript(ScriptTex, name, Vector2.zero, id, ty, ty.BaseType);
                    if (sLoc != null && sLoc.Count > 0)
                    {
                        try
                        {
                            var sl = sLoc.First(s => s.Name == newScript.Name);
                            newScript.Location = sl.Location1;
                            newScript.Location2 = sl.Location2;
                        }
                        catch { }
                    }
                    newScript.Read(file.FullName);
                    Graph[id].Add(newScript); 
                }
            }
        } 
        #endregion

        #region Load relationship lines 
        List<GraphScript> allScripts = new List<GraphScript>();
        List<GraphRelationship> rels = new List<GraphRelationship>();
        foreach (var folder in Graph.Values)
        {
            if (folder.ID < 0)
                continue;

            foreach (var item in folder)
            {
                GraphScript script = item as GraphScript;
                if (script != null)
                {
                    allScripts.Add(script);
                }
            }
        }
        foreach (var script in allScripts)
        {
            FieldInfo[] fields = script.ScriptType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | 
                                                             BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < types.Length; i++)
            {
                if (script.BaseType == types[i])
                {
                    GraphScript bs = allScripts.First(s => script.BaseType == s.ScriptType);
                    GraphRelationship newRel =
                        new GraphRelationship(bs, script, RelationshipType.Inheritance);
                    if (bs.ParentID == script.ParentID)
                    {
                        newRel.ParentID = script.ParentID;
                        Graph[newRel.ParentID].Add(newRel);
                    }
                    else
                    {
                        newRel.ParentID = -1;
                        rels.Add(newRel);
                    }
                    script.InheritanceRelationship = newRel;
                    //break;
                }
                foreach (var field in fields)
                {
                    if (field.FieldType == types[i] || field.FieldType.GetElementType() == types[i])
                    {
                        try
                        {
                            GraphScript sc = allScripts.First(s => field.FieldType == s.ScriptType || 
                                                              field.FieldType.GetElementType() == s.ScriptType);
                            script.AddField(sc, false, field.IsPublic, 
                                field.FieldType.GetElementType() == sc.ScriptType);
                            GraphRelationship newRel =
                                new GraphRelationship(sc, script, RelationshipType.Field);
                            if (sc.ParentID == script.ParentID)
                            {
                                newRel.ParentID = script.ParentID;
                                Graph[newRel.ParentID].Add(newRel);
                            }
                            else
                            {
                                newRel.ParentID = -1;
                                rels.Add(newRel);
                            }
                        }
                        catch { }
                    }
                }
            }
        } 
        #endregion

        #region All scripts 

        GraphFolder virtualFolder = new GraphFolder(-1, null, "All Scripts", Vector2.zero, -1);
        foreach (var folder in Graph.Values)
        {
            foreach (var item in folder)
            {
                if (item is GraphFolder)
                    continue;
                virtualFolder.Add((IGraphObject)item);
            }
        }
        foreach (var item in rels)
        {
            virtualFolder.Add(item);
        }
        Graph.Add(-1, virtualFolder);

        IsAllScriptView = false;
        viewButtonName = "All Script View";//"Category View";
        lastFolderID = CurFolderID;
        //CurFolderID = -1;
        //foreach (var item in Graph[-1])
        //{
        //    GraphScript script = item as GraphScript;
        //    if (script != null)
        //        script.ApplyParentLocation(true, Graph[script.ParentID].Location);
        //}

        #endregion

        IsInit = true;
    }

    //Grid
    private void DrawVerticalGrid(float posX)
    {
        Vector2 newScroll = new Vector2(posX, 0);
        int newGridSize = (int)(gridSize * zoom);
        if(newGridSize <= 1)
        return;

        for (float i = ((int)-posX / newGridSize) * newGridSize; i <= ((int)-posX / newGridSize) * newGridSize +
            position.width; i += newGridSize)
        {
            
            Handles.DrawBezier(new Vector2(i, 0) * 1 + newScroll,
                               new Vector2(i, position.height) * 1 + newScroll,
                               new Vector2(i, 0) * 1 + newScroll,
                               new Vector2(i, position.height) * 1 + newScroll,
                               new Color(1, 1, 1, (i % (gridSize * 10) == 0 ? 0.3f : 0.15f)), null, 1.0f);
        }
    }
    private void DrawHorizontalGrid(float posY)
    {
        Vector2 newScroll = new Vector2(0, posY);
        int newGridSize = (int)(gridSize * zoom);
        if (newGridSize <= 1)
            return;

        for (float i = ((int)-posY / newGridSize) * newGridSize; i <= ((int)-posY / newGridSize) * newGridSize +
            position.height; i += newGridSize)
        {
            Handles.DrawBezier(new Vector2(0, i) * 1 + newScroll,
                              new Vector2(position.width, i) * 1 + newScroll,
                              new Vector2(0, i) * 1 + newScroll,
                              new Vector2(position.width, i) * 1 + newScroll,
                              new Color(1, 1, 1, (i % (gridSize * 10) == 0 ? 0.3f : 0.15f)), null, 1.0f);
        }
    }

    private GraphNodeBase selectedNode = null;
    private void OnGUI()
    {
        if (!IsInit)
        {
            Loading();
        }

        #region Check events 

        if (!newFolderDialogBox && !newScriptDialogBox)
        {
            curEvent = Event.current;

            #region Check for double click on folders
            //Check for double click on folders
            if (curEvent.clickCount == 2 && curEvent.button == 0)
            {
                if (doubleClick)
                {
                    doubleClick = false;
                }
                else
                {
                    for (int i = Graph[CurFolderID].Count - 1; i >= 0; i--)
                    {
                        GraphFolder folder = Graph[CurFolderID][i] as GraphFolder;
                        if (folder != null)
                        {
                            if (folder.MouseEnter(curEvent.mousePosition - scroll, zoom))
                            {
                                CurFolderID = folder.ID;
                                doubleClick = true;
                                curPathText = curPathText + "/" + Graph[CurFolderID].Name;
                                if (LabelStyle.CalcSize(new GUIContent(curPathText)).x >
                                    AddressTextboxTex.width - 10)
                                    PathText = ".../" + Graph[CurFolderID].Name;
                                else
                                    PathText = curPathText;
                                Repaint();
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Check for mouse down
            //Check for mouse down
            if (curEvent.type == EventType.mouseDown)
            {
                selectedNode = null;
                for (int i = Graph[CurFolderID].Count - 1; i >= 0; i--)
                {
                    GraphNodeBase node = Graph[CurFolderID][i] as GraphNodeBase;
                    if (node != null)
                    {
                        #region Is mouse over this node
                        if (node.MouseEnter(curEvent.mousePosition - scroll, zoom))
                        {
                            bool is_cr = false;
                            if (node is GraphScript)
                            {
                                is_cr = ((GraphScript)node).IsRelationshipRect
                                    (curEvent.mousePosition - scroll, zoom);
                            }
                            if (curEvent.button == 0)
                            {
                                #region Create Inheritance relationship
                                //Create Inheritance relationship
                                if (is_cr)
                                {
                                    dragInheritanceLine = true;
                                    curScriptLine = (GraphScript)node;
                                }
                                #endregion

                                #region Drag objects by left mouse button
                                //Drag objects by left mouse button
                                else
                                {
                                    mouseDown = true;
                                    dragOffset = curEvent.mousePosition - node.Location * zoom;
                                    curNode = node;
                                    selectedNode = node;
                                    Graph[CurFolderID].Remove(node);
                                    Graph[CurFolderID].Add(node);
                                    break;
                                }
                                #endregion
                            }
                            //...
                            else if (curEvent.button == 1)
                            {
                                #region Create Field relationship
                                //Create Field relationship
                                if (is_cr)
                                {
                                    dragFieldLine = true;
                                    curScriptLine = (GraphScript)node;
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                if (selectedNode == null)
                    Repaint();

                //Scroll graph by middle mouse button
                if (curEvent.button == 2)
                {
                    scrolling = true;
                    dragOffset = curEvent.mousePosition - scroll;
                }
            }
            #endregion

            //Cencel all mouse down actions
            else if (curEvent.type == EventType.mouseUp)
            {
                mouseDown = false;
                scrolling = false;

                #region Create Relationships
                //Create inheritance relationships
                if (dragInheritanceLine || dragFieldLine)
                {
                    for (int i = Graph[CurFolderID].Count - 1; i >= 0; i--)
                    {
                        GraphScript scr = Graph[CurFolderID][i] as GraphScript;
                        if (scr != null)
                        {
                            if (scr.MouseEnter(curEvent.mousePosition - scroll, zoom))
                            {
                                #region Inheritance 
                                if (dragInheritanceLine)
                                {
                                    GraphRelationship oldRel = curScriptLine.InheritanceRelationship;
                                    if (oldRel != null)
                                    {
                                        Graph[oldRel.ParentID].Remove(oldRel);
                                        Graph[-1].Remove(oldRel);
                                    }
                                    curScriptLine.SetBaseType(scr);
                                    GraphRelationship newRel =
                                        new GraphRelationship(scr, curScriptLine, RelationshipType.Inheritance);
                                    if (scr.ParentID == curScriptLine.ParentID)
                                    {
                                        newRel.ParentID = scr.ParentID;
                                        Graph[newRel.ParentID].Add(newRel);
                                        Graph[-1].Add(newRel);
                                    }
                                    else
                                    {
                                        newRel.ParentID = -1;
                                        Graph[-1].Add(newRel);
                                    }
                                    curScriptLine.InheritanceRelationship = newRel;
                                    break;
                                }
                                #endregion

                                #region Field 
                                if (dragFieldLine)
                                {
                                    curScriptLine.AddField(scr);
                                    GraphRelationship newRel =
                                        new GraphRelationship(scr, curScriptLine, RelationshipType.Field);
                                    if (scr.ParentID == curScriptLine.ParentID)
                                    {
                                        newRel.ParentID = scr.ParentID;
                                        Graph[newRel.ParentID].Add(newRel);
                                        Graph[-1].Add(newRel);
                                    }
                                    else
                                    {
                                        newRel.ParentID = -1;
                                        Graph[-1].Add(newRel);
                                    }
                                    break;
                                }
                                #endregion
                            }
                        }
                    }

                    dragInheritanceLine = false;
                    dragFieldLine = false;
                }
                #endregion
            }

            //Zoom graph by mouse scroll wheel
            if (curEvent.type == EventType.scrollWheel)
            {
                Vector2 pos1 = (curEvent.mousePosition - scroll) / zoom;
                zoom -= curEvent.delta.y * 0.02f;
                Vector2 pos2 = (curEvent.mousePosition - scroll) / zoom;
                scroll += (pos2 - pos1) * zoom;
                Repaint();
            }

            //Drag objects
            if (mouseDown)
            {
                Vector2 loc = curEvent.mousePosition / zoom - dragOffset / zoom;
                //if ((int)loc.x % gridSize != 0) loc.x = curNode.Location.x;
                //if ((int)loc.y % gridSize != 0) loc.y = curNode.Location.y;
                loc.x -= (int)loc.x % gridSize;
                loc.y -= (int)loc.y % gridSize;
                curNode.Location = loc;
                Repaint();
            }
            //Scroll graph
            else if (scrolling)
            {
                scroll = curEvent.mousePosition - dragOffset;
                Repaint();
            }

            //Reset scroll
            if (curEvent.keyCode == KeyCode.Home)
            {
                scroll = Vector2.zero;
                zoom = 1.0f;
                Repaint();
            }

            #region Delete selected node from graph and project 
            //Delete selected node from graph and project
            if (curEvent.keyCode == KeyCode.Delete)
            {
                if (selectedNode != null)
                {
                    if (selectedNode is GraphFolder)
                    {
                        GraphFolder folder = (GraphFolder)selectedNode;
                        if (NewFolders.Contains(folder))
                        {
                            NewFolders.Remove(folder);
                            foreach (var item in folder)
                            {
                                Graph[-1].Remove((IGraphObject)item);
                            }
                            Graph[folder.ParentID].Remove(folder);
                            Graph.Remove(folder.ID);
                            selectedNode = null;
                            Repaint();
                        }
                    }
                    else
                    {
                        GraphFolder folder = Graph[selectedNode.ParentID];
                        if (File.Exists(folder.FullPath + "/" + selectedNode.Name + ".c") == false)
                        {
                            Graph[folder.ID].Remove(selectedNode);
                            selectedNode = null;
                            Repaint();
                        }
                    }
                }
            } 
            #endregion
        } 

        #endregion

        #region Draw 

        #region Draw lines

        Handles.BeginGUI();

        //Draw grids
        DrawVerticalGrid(scroll.x);
        DrawHorizontalGrid(scroll.y);

        //Draw relationship lines
        foreach (var item in Graph[CurFolderID].OfType<GraphRelationship>())
        {
            item.Draw(scroll, zoom);
        }

        //Drag line for create relationship
        if (dragInheritanceLine || dragFieldLine)
        {
            Handles.DrawBezier(curScriptLine.Center * zoom + scroll, curEvent.mousePosition,
                               curScriptLine.Center * zoom + scroll, curEvent.mousePosition,
                               (dragInheritanceLine ? Color.black : Color.yellow), null, 5.0f);
            Repaint();
        }

        Handles.EndGUI();

        #endregion

        //Draw graph nodes and folders
        foreach (var item in Graph[CurFolderID].OfType<GraphNodeBase>())
        {
            item.Draw(scroll, zoom);
            if (item == selectedNode)
            {
                if (item is GraphFolder)
                    item.DrawSelected(SelectedFolderTex, scroll, zoom);
                else if (item is GraphScript)
                    item.DrawSelected(SelectedScriptTex, scroll, zoom);
            }
        }

        #region Top bar 

        //Top bar
        GUI.DrawTexture(new Rect(0, 0, position.width, TopBarTex.height), TopBarTex);

        //Address bar
        GUI.DrawTexture(new Rect(CompileButton.rect.x + CompileButton.rect.width + 100,
            (int)(TopBarTex.height - AddressTextboxTex.height) / 2 - 4, AddressTextboxTex.width, 
            AddressTextboxTex.height), AddressTextboxTex);
        if (BackButton.Draw(this))
        {
            if (!IsAllScriptView && Graph[CurFolderID].ID > 0)
            {
                curPathText = curPathText.Remove(curPathText.Length - Graph[CurFolderID].Name.Length - 1);
                CurFolderID = Graph[CurFolderID].ParentID;
                if (LabelStyle.CalcSize(new GUIContent(curPathText)).x >
                            AddressTextboxTex.width - 10)
                    PathText = ".../" + Graph[CurFolderID].Name;
                else
                    PathText = curPathText; 
            }
        }

        GUI.Label(new Rect(CompileButton.rect.x + CompileButton.rect.width + 100 + 5,
                (int)(TopBarTex.height - AddressTextboxTex.height) / 2 + 4, 100, AddressTextboxTex.width),
                IsAllScriptView ? "Scripts" : PathText, LabelStyle);

        //if (!IsAllScriptView)
        //    GUI.Label(new Rect(CompileButton.rect.x + CompileButton.rect.width + 100 + 5,
        //        (int)(TopBarTex.height - AddressTextboxTex.height) / 2 + 4, 100, AddressTextboxTex.width),
        //        PathText, LabelStyle);
        //else
        //    GUI.Label(new Rect(CompileButton.rect.x + CompileButton.rect.width + 100 + 5,
        //        (int)(TopBarTex.height - AddressTextboxTex.height) / 2 + 4, 100, AddressTextboxTex.width),
        //        "All Scripts", LabelStyle);

        //Menu buttons
        #region New folder button 
        if (NewFolderButton.Draw(this))
        {
            if (!IsAllScriptView)
            {
                newfolderRect = new Rect((int)(position.width - newfolderRect.width) / 2,
                    (int)(position.height - newfolderRect.height) / 2,
                    newfolderRect.width, newfolderRect.height);
                newFolderDialogBox = true;
                folderName = "";
            }
        } 
        #endregion
        #region New script button 
        if (NewScriptButton.Draw(this))
        {
            //if (!IsAllScriptView)
            //{
                newScriptRect = new Rect((int)(position.width - newScriptRect.width) / 2,
                    (int)(position.height - newScriptRect.height) / 2,
                    newScriptRect.width, newScriptRect.height);
                newScriptDialogBox = true;
                scriptName = "";
            //}
        } 
        #endregion
        #region Folders/Scripts View 
        if (IsAllScriptView)
        {
            if (ScriptsViewButton.Draw(this))
            {
                IsAllScriptView = false;
                CurFolderID = lastFolderID;
                foreach (var item in Graph[-1])
                {
                    GraphScript script = item as GraphScript;
                    if (script != null)
                        script.ApplyParentLocation(false, Graph[script.ParentID].Location);
                }

                scroll = Vector2.zero;
                zoom = 1.0f;
            }
        }
        else
        {
            if (FoldersViewButton.Draw(this))
            {
                IsAllScriptView = true;
                lastFolderID = CurFolderID;
                CurFolderID = -1;
                foreach (var item in Graph[-1])
                {
                    GraphScript script = item as GraphScript;
                    if (script != null)
                        script.ApplyParentLocation(true, Graph[script.ParentID].Location);
                }

                scroll = Vector2.zero;
                zoom = 1.0f;
            }
        }
        #endregion
        #region Compile button
        if (CompileButton.Draw(this))
        {
            BuildAnApplyChanges();
        } 
        #endregion 
        #region Home Button 
        if (HomeButton.Draw(this))
        {
            scroll = Vector2.zero;
            zoom = 1.0f;
            Repaint();
        }
        #endregion

        #endregion

        #region Draw Window

        BeginWindows();

        //Create new folder
        if (newFolderDialogBox)
        {
            newfolderRect = GUI.Window(1, newfolderRect, CreateNewFolder, "New Folder");
        }

        //Create new script
        if (newScriptDialogBox)
        {
            newScriptRect = GUI.Window(2, newScriptRect, CreateNewMonoBehaviour, "New Script");
        }

        EndWindows();

        #endregion 

        #endregion
    }

    private void BuildAnApplyChanges()
    {
        //Folders
        for (int i = 0; i < NewFolders.Count; i++)
        {
            NewFolders[i].FullPath = AssetPath + GetPath(NewFolders[i].ParentID) +
                NewFolders[i].Name;
            Directory.CreateDirectory(NewFolders[i].FullPath);
        }
        NewFolders.Clear();

        //Scripts
        foreach (var folder in Graph.Values)
        {
            if (folder.ID < 0)
                continue;

            foreach (var item in folder)
            {
                GraphScript script = item as GraphScript;
                if (script != null)
                {
                    string str = AssetPath + GetPath(script.ParentID) + script.Name + ".cs";
                    script.Write(str);
                }
            }
        }

        SaveLocations();

        AssetDatabase.Refresh();
    }
    private void PropertiesFunction(int windowID)
    {
        #region Create new folder button 
        //Create new folder
        if (GUI.Button(new Rect(10, 25, 180, 35), "New Folder"))
        {
            if (!IsAllScriptView)
            {
                newfolderRect = new Rect(position.width - 210 - newfolderRect.width, 25,
                    newfolderRect.width, newfolderRect.height);
                newFolderDialogBox = true;
                folderName = "";
            }
        } 
        #endregion

        #region Create new MonoBehaviour button 
        //Create new MonoBehaviour
        if (GUI.Button(new Rect(10, 70, 180, 35), "New MonoBehaviour"))
        {
            if (!IsAllScriptView)
            {
                newScriptRect = new Rect(position.width - 210 - newScriptRect.width, 70,
                    newScriptRect.width, newScriptRect.height);
                newScriptDialogBox = true;
                scriptName = "";
            }
        } 
        #endregion

        #region All script view button

        if (GUI.Button(new Rect(10, 115, 180, 35), viewButtonName))
        {
            IsAllScriptView = !IsAllScriptView;
            if (IsAllScriptView)
            {
                viewButtonName = "Category View";
                lastFolderID = CurFolderID;
                CurFolderID = -1;
                foreach (var item in Graph[-1])
                {
                    GraphScript script = item as GraphScript;
                    if (script != null)
                        script.ApplyParentLocation(true, Graph[script.ParentID].Location);
                }
            }
            else
            {
                viewButtonName = "All Script View";
                CurFolderID = lastFolderID;
                foreach (var item in Graph[-1])
                {
                    GraphScript script = item as GraphScript;
                    if (script != null)
                        script.ApplyParentLocation(false, Graph[script.ParentID].Location);
                }
            }
        }

        #endregion

        #region Apply chenges button 
        //Import Folders and Scripts and apply chenges to unity assets
        if (GUI.Button(new Rect(10, 200, 180, 35), "Apply changes"))
        {
            //Folders
            for (int i = 0; i < NewFolders.Count; i++)
            {
                NewFolders[i].FullPath = AssetPath + GetPath(NewFolders[i].ParentID) +
                    NewFolders[i].Name;
                Directory.CreateDirectory(NewFolders[i].FullPath);
            }
            NewFolders.Clear();

            //Scripts
            foreach (var folder in Graph.Values)
            {
                if (folder.ID < 0)
                    continue;

                foreach (var item in folder)
                {
                    GraphScript script = item as GraphScript;
                    if (script != null)
                    {
                        string str = AssetPath + GetPath(script.ParentID) + script.Name + ".cs";
                        script.Write(str);
                    }
                }
            }

            SaveLocations();

            AssetDatabase.Refresh();
        } 
        #endregion
    }
    private static string GetPath(int parentId)
    {
        if (Graph[parentId].ID < 1)
            return string.Empty;
        else
        {
            return GetPath(Graph[parentId].ParentID) + Graph[parentId].Name + "/";
        }
    }
    private void CreateNewFolder(int windowID)
    {
        GUI.Label(new Rect(20, 30, 50, 20), "Name: ");
        GUI.SetNextControlName("FolderName");
        folderName = GUI.TextField(new Rect(70, 30, 100, 20), folderName);
        GUI.FocusControl("FolderName");
        if (GUI.Button(new Rect(newfolderRect.width - 80, newfolderRect.height - 30, 70, 20), "Cancel"))
            newFolderDialogBox = false;
        if (GUI.Button(new Rect(newfolderRect.width - 160, newfolderRect.height - 30, 70, 20), "OK"))
        {
            if (!string.IsNullOrEmpty(folderName))
            {
                newFolderDialogBox = false;
                GraphFolder folder = new GraphFolder(ID_Increase, FolderTex, folderName,
                                                     (new Vector2(newfolderRect.x, newfolderRect.y) - scroll) / zoom, CurFolderID);
                Graph[CurFolderID].Add(folder);
                Graph[folder.ID] = folder;
                NewFolders.Add(folder);
                selectedNode = folder;
            }
        }
        GUI.DragWindow();
    }
    private void CreateNewMonoBehaviour(int windowID)
    {
        GUI.Label(new Rect(20, 30, 50, 20), "Name: ");
        GUI.SetNextControlName("ScriptName");
        scriptName = GUI.TextField(new Rect(70, 30, 100, 20), scriptName);
        GUI.FocusControl("ScriptName");
        if (GUI.Button(new Rect(newScriptRect.width - 80, newScriptRect.height - 30, 70, 20), "Cancel"))
            newScriptDialogBox = false;
        if (GUI.Button(new Rect(newScriptRect.width - 160, newScriptRect.height - 30, 70, 20), "OK"))
        {
            if (!string.IsNullOrEmpty(scriptName))
            {
                newScriptDialogBox = false;
                string[] code = new string[Mono_Template.Length];
                Mono_Template.CopyTo(code, 0);
                code[3] = code[3].Replace("<name>", scriptName);
                GraphScript script = new GraphScript(ScriptTex, scriptName, 
                    (new Vector2(newScriptRect.x, newScriptRect.y) - scroll) / zoom, CurFolderID, 
                    null, typeof(MonoBehaviour), code);
                if (IsAllScriptView)
                {
                    int count = Graph[lastFolderID].Count;
                    int y = count / 4 * 150 + 100;
                    int x = count % 4 * 250 + 100;
                    script.Location1 = new Vector2(x, y);
                    Graph[lastFolderID].Add(script);
                    Graph[CurFolderID].Add(script);
                }
                else
                {
                    Graph[CurFolderID].Add(script);
                    Graph[-1].Add(script);
                }
                selectedNode = script;
            }
        }
        GUI.DragWindow();
    }
    private void SaveLocations()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(File.Open(@"Assets/ScriptDiagram/data.dat",
                    FileMode.OpenOrCreate)))
            {
                sw.WriteLine("#Folders");
                sw.WriteLine("Count:" + (Graph.Values.Count - 1 - NewFolders.Count).ToString());
                foreach (var folder in Graph.Values)
                {
                    if (folder.ID < 0 || NewFolders.Contains(folder))
                        continue;

                    string str = string.Format("{0},{1},{2},{3}", folder.Name, folder.FullPath, folder.Location.x, folder.Location.y);
                    sw.WriteLine(str);
                }

                List<GraphScript> scriptList = new List<GraphScript>();
                foreach (var folder in Graph.Values)
                {
                    if (folder.ID < 0 || NewFolders.Contains(folder))
                        continue;

                    foreach (var item in folder)
                    {
                        if (item is GraphScript)
                        {
                            //if (NewScripts.Contains((GraphScript)item))
                            //    continue;

                            scriptList.Add((GraphScript)item);
                        }
                    }
                }

                if (IsAllScriptView)
                {
                    foreach (var item in scriptList)
                    {
                        item.ApplyParentLocation(false, Graph[item.ParentID].Location);
                    }
                }
                sw.WriteLine("#Scripts");
                sw.WriteLine("Count:" + scriptList.Count.ToString());
                foreach (var item in scriptList)
                {
                    string str = string.Format("{0},{1},{2},{3},{4}", item.Name, item.Location.x, item.Location.y, item.Location2.x, item.Location2.y);
                    sw.WriteLine(str);
                }
                if (IsAllScriptView)
                {
                    foreach (var item in scriptList)
                    {
                        item.ApplyParentLocation(true, Graph[item.ParentID].Location);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    private struct FolderLocation
    {
        public string Name;
        public string Path;
        public Vector2 Location;
        public FolderLocation(string _name, string _path, Vector2 _loc)
        {
            Name = _name;
            Path = _path;
            Location = _loc;
        }
    }
    private struct ScriptLocation
    {
        public string Name;
        public Vector2 Location1;
        public Vector2 Location2;
        public ScriptLocation(string _name, Vector2 _loc1, Vector2 _loc2)
        {
            Name = _name;
            Location1 = _loc1;
            Location2 = _loc2;
        }
    }

    private void OnDestroy()
    {
        SaveLocations();
    }
}
