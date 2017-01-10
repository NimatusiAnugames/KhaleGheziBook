using System;
using UnityEngine;
using UnityEditor;

public class GraphFolder : GraphNodeBase, System.Collections.IEnumerable
{
    public readonly int ID;
    public string FullPath;
    private System.Collections.Generic.List<IGraphObject> list;
    public IGraphObject this[int index]
    {
        get { return list[index]; }
    }
    public int Count
    {
        get { return list.Count; }
    }
    public void Add(IGraphObject item)
    {
        list.Add(item);
    }
    public void Remove(IGraphObject item)
    {
        list.Remove(item);
    }
    public System.Collections.Generic.List<IGraphObject> GetList()
    {
        return list;
    }

    public GraphFolder(int _id, Texture _icon, string _name, Vector2 _loc, int _parentID)
        :base(_icon, _name, _loc, _parentID)
    {
        list = new System.Collections.Generic.List<IGraphObject>();
        lblRect = new Rect(iconRect.center.x - lblWidth / 2, 
            Location.y + iconRect.height, iconRect.width, 35);
        ID = _id;
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
            lblRect.x = iconRect.center.x - lblWidth / 2;
            lblRect.y = iconRect.y + iconRect.height;
        }
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
        return list.GetEnumerator();
    }
}