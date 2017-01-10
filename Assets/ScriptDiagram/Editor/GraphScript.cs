using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class GraphScript : GraphNodeBase
{
    public System.Type ScriptType;
    public System.Type BaseType;
    private string baseTypeName = string.Empty;
    public string BaseTypeName
    {
        get { return baseTypeName; }
    }
    public void ApplyParentLocation(bool add, Vector2 parentLoc)
    {
        //if (add)
        //    Location += parentLoc * 2f;
        //else
        //    Location -= parentLoc * 2f;
        if (add)
        {
            Location1 = Location;
            Location = Location2;
        }
        else
        {
            Location2 = Location;
            Location = Location1;
        }
    }
    public override Vector2 Location
    {
        get
        {
            return base.Location;
        }
        set
        {
            base.Location = value;
            lblRect.x = iconRect.x + 8;
            lblRect.y = iconRect.y + 10;
        }
    }
    public Vector2 Location1;
    public Vector2 Location2;
    public GraphRelationship InheritanceRelationship = null;

    private bool isDirty = false;
    private List<string> code;

    public GraphScript(Texture _icon, string _name, Vector2 _loc, int _parentID, 
        System.Type _scriptType, System.Type _baseType, string[] _code = null)
        :base(_icon, _name, _loc, _parentID)
    {
        Location1 = Location2 = _loc;
        fontSize = 12;
        style.fontSize = 12;
        lblWidth = style.CalcSize(new GUIContent(Name)).x;
        lblRect = new Rect(iconRect.x + 8, 
            Location.y + 10, iconRect.width, 35);
        ScriptType = _scriptType;
        BaseType = _baseType;
        if (_baseType != null)
            baseTypeName = _baseType.Name;

        code = new List<string>();
        if (_code != null)
        {
            isDirty = true;
            code.AddRange(_code);
        }
    }

    //Is mouse in relationship rectangel for create relationship?
    public bool IsRelationshipRect(Vector2 mousePosition, float zoom)
    {
        Rect rect = new Rect((iconRect.x + 128) * zoom,
                             iconRect.y * zoom,
                             34 * zoom,
                             34 * zoom);
        return rect.Contains(mousePosition);
    }

    
    public void SetBaseType(GraphScript _baseScript)
    {
        #region Change BaseType in code

        //Change BaseType in code
        for (int i = 0; i < code.Count; i++)
        {
            if (IsComment(code[i]))
                continue;
            string className = "class " + this.Name;
            if (code[i].Contains(className))
            {
                string[] split = code[i].Split(':');
                if (split.Length == 1)
                {
                    code[i] = className + " : " + _baseScript.Name;
                }
                else
                {
                    split[1] = split[1].Replace(baseTypeName, _baseScript.Name);
                    code[i] = split[0] + " : " + split[1];
                }

                isDirty = true;
                break;
            }
        }

        #endregion
        
        if (_baseScript.ScriptType == null)
        {
            BaseType = null;
            baseTypeName = _baseScript.Name;
        }
        else
        {
            BaseType = _baseScript.ScriptType;
            baseTypeName = BaseType.Name;
        }
    }

    private class ScriptField
    {
        public GraphScript Script;
        public bool IsPublic;
        public bool IsArray;
    }
    private List<ScriptField> fields = new List<ScriptField>();
    public void AddField(GraphScript _field, bool modifyCode = true, bool isPublic = true, bool isArray = false)
    {
        if (modifyCode)
        {
            #region Add field to code

            string fieldName = string.Format("\tpublic {0} {1};", _field.Name, _field.Name.ToLower());
            for (int i = 0; i < code.Count; i++)
            {
                if (IsComment(code[i]))
                    continue;
                if (code[i].Contains("class " + this.Name))
                {
                    if (code[i].Contains("{"))
                    {
                        code.Insert(i + 1, fieldName);
                    }
                    else
                    {
                        int idx = i + 1;
                        do
                        {
                            if (!IsComment(code[idx]))
                            {
                                if (code[idx].Contains("{"))
                                {
                                    code.Insert(idx + 1, fieldName);
                                    break;
                                }
                            }
                            idx++;
                        } while (idx < code.Count);
                    }
                    isDirty = true;
                    break;
                }
            }

            #endregion
        }
        fields.Add(new ScriptField() { Script = _field, IsPublic = isPublic, IsArray = isArray });
    }

    private bool IsComment(string line)
    {
        if (line.TrimStart('\t').StartsWith("//"))
            return true;

        return false;
    }

    public override void Draw(Vector2 scroll, float zoom)
    {
        base.Draw(scroll, zoom);

        if (!string.IsNullOrEmpty(baseTypeName))
        {
            Rect rct = new Rect((iconRect.x + 5) * zoom + scroll.x, (iconRect.y + 35) * zoom + scroll.y,
            iconRect.width * zoom, 35 * zoom);
            GUI.Label(rct, ">" + baseTypeName, style);
        }
        for (int i = 0; i < fields.Count; i++)
        {
            Rect rct = new Rect((iconRect.x + 5) * zoom + scroll.x, 
                                (iconRect.y + 50 + (i * 15)) * zoom + scroll.y,
            iconRect.width * zoom, 35 * zoom);

            GUI.Label(rct, (fields[i].IsPublic ? "+" : "-") + fields[i].Script.Name + 
                (fields[i].IsArray ? "[]" : string.Empty), style);
        }
    }

    public void Read(string fileName)
    {
        using (StreamReader sr = File.OpenText(fileName))
        {
            while (sr.EndOfStream == false)
            {
                code.Add(sr.ReadLine());
            }
        }
    }
    public void Write(string fileName)
    {
        if (isDirty == false)
            return;

        using (StreamWriter sw = File.CreateText(fileName))
        {
            for (int i = 0; i < code.Count; i++)
            {
                sw.WriteLine(code[i]);
            }
        }
        isDirty = false;
    }
}