using UnityEngine;
using System.Collections;

public class MovePath : MonoBehaviour
{
    [HideInInspector]
    public Transform[] Nodes;

    void Awake()
    {
        Nodes = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Nodes[i] = transform.GetChild(i);
        }
    }

    public float TotalDistance
    {
        get
        {
            //return Vector2.Distance(Nodes[0].position, Nodes[transform.childCount - 1].position);
            float total = 0;
            for (int i = 0; i < Nodes.Length - 1; i++)
            {
                total += Vector2.Distance(Nodes[i].position, Nodes[i + 1].position);
            }
            return total;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform node = transform.GetChild(i);
            Gizmos.DrawSphere(node.position, 0.08f);
            if (i > 0)
            {
                Transform _node = transform.GetChild(i - 1);
                Gizmos.DrawLine(_node.position, node.position);
            }
        }
    }

}
