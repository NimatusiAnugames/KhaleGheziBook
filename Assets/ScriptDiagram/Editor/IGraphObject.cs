using System;

public interface IGraphObject
{
    Type ObjectType { get; }
    void Draw(UnityEngine.Vector2 scroll, float zoom);
}
