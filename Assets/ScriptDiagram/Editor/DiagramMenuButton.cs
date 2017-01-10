using UnityEngine;

class DiagramMenuButton
{
    private Texture texture, over;
    private bool mouseOver = false;
    public Rect rect;

    public DiagramMenuButton(Vector2 _position, Texture _texture, Texture _over)
    {
        texture = _texture;
        over = _over;
        rect = new Rect(_position.x, _position.y, texture.width, texture.height);
    }
    public bool Draw(ScriptDiagram diagram)
    {
        //Check for click event
        bool result = false;
        if (Event.current.type == EventType.mouseDown)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                mouseOver = true;
                diagram.Repaint();
            }
        }
        if (mouseOver)
        {
            diagram.Repaint();
            if (!rect.Contains(Event.current.mousePosition))
            {
                mouseOver = false;
            }
            else if (Event.current.type == EventType.mouseUp)
            {
                mouseOver = false;
                result = true;
            }
        }

        //Draw
        GUI.DrawTexture(rect, texture);
        if (mouseOver)
            GUI.DrawTexture(rect, over);

        return result;
    }
}
