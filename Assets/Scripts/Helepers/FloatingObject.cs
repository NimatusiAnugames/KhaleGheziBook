using UnityEngine;
using System.Collections;

public class FloatingObject : MonoBehaviour
{
    public Vector2 Factor = Vector2.one;
    public float Speed = 1;
    private float rot;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rot += Time.deltaTime * Speed;
        //Vector3 parentPos = transform.parent.position;
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Cos(rot) * Factor.x;
        pos.y = Mathf.Sin(rot) * Factor.y;
        transform.localPosition = pos;
    }
}
