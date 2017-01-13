using UnityEngine;
using System.Collections;

public class ChildFlying : MonoBehaviour
{
    public MovePath FlyPath;
    public MovePath RunPath;
    private bool FlyState = true;
    private int index;
    private float frame;
    public AnimationItem animItem;

    void Start()
    {
        animItem.EndAction = new System.Action<int>((id) => 
        {
            if (id == 2)
            {
                frame = 0;
                index = 0;
                FlyState = false;
            }
        });
    }

    void Update()
    {
        if (FlyState)
        {
            #region Flying
            frame += Time.deltaTime * FlyPath.Nodes[index].Speed;
            switch (index)
            {
                case 0:
                    transform.position = Vector3.Lerp(FlyPath.Nodes[index].transform.position,
                        FlyPath.Nodes[index + 1].transform.position, frame);
                    if (frame >= 1)
                    {
                        frame = 0;
                        animItem.SetState(1);
                        index++;
                    }
                    break;
                case 1:
                    float val = FlyPath.Nodes[index].Curve.Evaluate(frame);
                    Vector3 diff = FlyPath.Nodes[index + 1].transform.position - FlyPath.Nodes[index].transform.position;
                    transform.position = FlyPath.Nodes[index].transform.position +
                        new Vector3(diff.x * frame, diff.y * val, 0);
                    if (frame >= 1)
                    {
                        transform.position = FlyPath.Nodes[index + 1].transform.position;
                        animItem.SetState(2);
                        index++;
                    }
                    break;
            }
            #endregion
        }
        else
        {
            #region Runing
            frame += Time.deltaTime * RunPath.Nodes[index].Speed;
            transform.position = Vector3.Lerp(RunPath.Nodes[index].transform.position,
                RunPath.Nodes[index + 1].transform.position, frame);
            if (frame >= 1)
            {
                frame = 0;
                index++;
                if (index == RunPath.Nodes.Length - 1)
                {
                    enabled = false;
                }
            }
            #endregion
        }
    }
}
