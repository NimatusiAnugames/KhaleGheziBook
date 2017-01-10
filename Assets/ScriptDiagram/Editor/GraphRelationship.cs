using UnityEngine;
using UnityEditor;

public enum RelationshipType
{
    Inheritance,
    Field
}
public class GraphRelationship : IGraphObject
{
    public System.Type ObjectType
    {
        get { return null; }
    }

    public int ParentID = -1;
    public RelationshipType R_Type { get; private set; }
    public GraphNodeBase ParentNode { get; private set; }
    public GraphNodeBase ChildNode { get; private set; }
    private Color color;

    public GraphRelationship(GraphNodeBase _parent, GraphNodeBase _child, RelationshipType _type)
    {
        ParentNode = _parent;
        ChildNode = _child;
        R_Type = _type;
        switch (_type)
        {
            case  RelationshipType.Inheritance:
                color = Color.black;
                break;
            case RelationshipType.Field:
                color = Color.yellow;
                break;
        }
    }

    public void Draw(Vector2 scroll, float zoom)
    {
        Handles.DrawBezier(ParentNode.Center * zoom + scroll, ChildNode.Center * zoom + scroll,
                           ParentNode.Center * zoom + scroll, ChildNode.Center * zoom + scroll,
                           color, null, 5.0f);
    }
}