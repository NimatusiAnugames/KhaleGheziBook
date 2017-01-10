using System;
using UnityEngine;
using UnityEditor;

public class GraphNodeBase : IGraphObject
{
    public Type ObjectType
    {
        get { return this.GetType(); }
    }

    public int ParentID = -1;
    public Texture Icon;
    public string Name;
    private Vector2 location;
    public virtual Vector2 Location
    {
        get { return location; }
        set
        {
            location = value;
            iconRect.x = location.x;
            iconRect.y = location.y;
        }
    }
    protected Rect iconRect;
    public Vector2 Center
    {
        get { return iconRect.center; }
    }
    protected Rect lblRect;
    protected GUIStyle style;
    protected int fontSize = 14;
    protected float lblWidth;

    public GraphNodeBase(Texture _icon, string _name, Vector2 _loc, int _parentID)
    {
        ParentID = _parentID;
        Icon = _icon;
        Name = _name;
        Location = _loc;

        if (Icon == null)
            return;

        iconRect = new Rect(Location.x, Location.y, Icon.width, Icon.height);
        style = new GUIStyle();
        style.normal.textColor = new Color(1, 1, 1, 0.7f);
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        lblWidth = style.CalcSize(new GUIContent(Name)).x;
    }

    public bool MouseEnter(Vector2 mousePosition, float zoom)
    {
        Rect rect = new Rect(iconRect.x * zoom,
                             iconRect.y * zoom,
                             iconRect.width * zoom,
                             iconRect.height * zoom);
        return rect.Contains(mousePosition);
    }

    public virtual void Draw(Vector2 scroll, float zoom)
    {
        if (Icon == null)
            return;

        GUI.DrawTexture(new Rect(iconRect.x * zoom + scroll.x, iconRect.y * zoom + scroll.y, 
            iconRect.width * zoom, iconRect.height * zoom), Icon);
        style.fontSize = (int)(fontSize * zoom);
        GUI.Label(new Rect(lblRect.x * zoom + scroll.x, lblRect.y * zoom + scroll.y,
            lblRect.width * zoom, lblRect.height * zoom), Name, style);
    }
    public void DrawSelected(Texture texture, Vector2 scroll, float zoom)
    {
        GUI.DrawTexture(new Rect(iconRect.x * zoom + scroll.x, iconRect.y * zoom + scroll.y,
            iconRect.width * zoom, iconRect.height * zoom), texture);
    }
}