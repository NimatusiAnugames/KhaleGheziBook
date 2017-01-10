using UnityEngine;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    public AnimationCurve Curve;
    public Vector2 XRange, YRange;
    private Vector2 fromPos, toPos;
    private float frame;

    void Start()
    {
        fromPos = transform.position;
        toPos.x = fromPos.x + Random.Range(XRange.x, XRange.y);
        toPos.y = fromPos.y + Random.Range(YRange.x, YRange.y);
        frame = 0;
    }
    void Update()
    {
        frame += Time.deltaTime * 20;
        float val = Curve.Evaluate(frame);

        float diffulteX = toPos.x - fromPos.x;
        float diffulteY = toPos.y - fromPos.y;
        transform.position = new Vector2(fromPos.x + (val * diffulteX), fromPos.y + (val * diffulteY));
        if(val == 1)
        {
            frame = 0;
            fromPos = toPos;
            toPos.x = fromPos.x + Random.Range(XRange.x, XRange.y);
            toPos.y = fromPos.y + Random.Range(YRange.x, YRange.y);
        }
    }

    //   public AnimationCurve Curve;
    //   private Vector3 from, to;
    //   private float frame;
    //   private bool move;

    //// Use this for initialization
    //void Start()
    //{
    //       from = Vector3.zero;
    //       to = new Vector3(5, 5, 0);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //       if(Input.GetKeyDown( KeyCode.Space))
    //       {
    //           move = true;
    //           transform.position = from;
    //           frame = 0;
    //       }

    //       if (move)
    //       {
    //           frame += Time.deltaTime;
    //           float val = Curve.Evaluate(frame);

    //           float diffulteX = to.x - from.x;
    //           float diffulteY = to.y - from.y;
    //           transform.position = new Vector2(from.x + (frame * diffulteX), from.y + (val * diffulteY));
    //           if (frame >= 1)
    //               move = false;
    //       }
    //   }
}
