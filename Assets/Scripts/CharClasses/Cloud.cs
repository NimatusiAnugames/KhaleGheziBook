using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour
{
    public float Speed = 1;
    private Transform trans;
    private BoxCollider2D box;
    private Vector3 from, to;
    private float frame;

    // Use this for initialization
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        trans = transform.GetChild(0);
        from.z = to.z = transform.position.z;
        from.x = transform.position.x + box.offset.x + box.size.x / 2;
        to.x = transform.position.x + box.offset.x - box.size.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        frame += Time.deltaTime * Speed;
        trans.position = Vector3.Lerp(from, to, frame);
        if(frame >= 1)
        {
            frame = 0;
            from.y = Random.Range(transform.position.y + box.offset.y - box.size.y, transform.position.y + box.offset.y + box.size.y);
            to.y = from.y;
            trans.position = from;
        }
    }
}
