using UnityEngine;
using System.Collections;

public class MoveInPath : MonoBehaviour
{
    public MovePath Path;
    public float Speed;
    private float[] speeds;
    private int curIndex;
    private float frame;

    // Use this for initialization
    void Start()
    {
        curIndex = 0;
        float totalDistance = Path.TotalDistance;
        speeds = new float[Path.Nodes.Length - 1];
        for (int i = 0; i < speeds.Length; i++)
        {
            float d = Vector2.Distance(Path.Nodes[i].position, Path.Nodes[i + 1].position);
            speeds[i] = (totalDistance / d) * Speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Path == null)
            return;
        if(curIndex < Path.Nodes.Length - 1)
        {
            frame += Time.deltaTime * speeds[curIndex];
            transform.position = Vector3.Lerp(Path.Nodes[curIndex].position, Path.Nodes[curIndex + 1].position, frame);
            if(frame >= 1)
            {
                frame = 0;
                curIndex++;
            }
        }
        else
        {

        }
    }
}
