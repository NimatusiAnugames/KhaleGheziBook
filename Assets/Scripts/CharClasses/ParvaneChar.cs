using UnityEngine;
using System.Collections;

public class ParvaneChar : MonoBehaviour
{
    public MovePath Path;
    private AnimationItem item;
    private float frame;
    private int index = -1;

    void Start()
    {
        item = GetComponent<AnimationItem>();
        enabled = false;
    }

    public void Fly()
    {
        if (item.CurrentState != AnimConsts.Parvaneh_Idle)
            return;

        frame = 0;
        item.SetState(AnimConsts.Parvaneh_Parvaz2);
        index++;
        if (index == Path.Nodes.Length - 1)
            index = 0;
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        frame += Time.deltaTime * Path.Nodes[index].Speed;
        float val = Path.Nodes[index].Curve.Evaluate(frame);
        Vector3 diff = Path.Nodes[index + 1].transform.position - Path.Nodes[index].transform.position;
        transform.position = Path.Nodes[index].transform.position + new Vector3(diff.x * frame, diff.y * val, 0);
        if(frame >= 1)
        {
            item.SetState(AnimConsts.Parvaneh_Idle);
            enabled = false;
        }
    }
}
