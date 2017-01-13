using UnityEngine;
using System.Collections;

public class MovePath : MonoBehaviour
{
    [HideInInspector]
    public PathNode[] Nodes;

    void Awake()
    {
        Nodes = new PathNode[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Nodes[i] = transform.GetChild(i).GetComponent<PathNode>();
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
