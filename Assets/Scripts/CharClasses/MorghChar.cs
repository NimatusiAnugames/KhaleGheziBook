using UnityEngine;
using System.Collections.Generic;

public class MorghChar : MonoBehaviour
{
    private AnimationItem anim;
    private float frame1, frame2;

	// Use this for initialization
	void Start()
	{
        anim = GetComponent<AnimationItem>();
        frame1 = Random.Range(1, 4);
        frame2 = Random.Range(1, 2);
        anim.EndAction = new System.Action<int>((id) => 
        {
            if (id == AnimConsts.Morgh_Doon_Khordan)
            {
                frame1 = Random.Range(1, 4);
                frame2 = Random.Range(1, 2);
            }
        });
	}

	// Update is called once per frame
	void Update()
	{
        frame1 -= Time.deltaTime;
        switch (anim.CurrentState)
        {
            case AnimConsts.Morgh_Idle:
                frame2 -= Time.deltaTime;
                if (frame2 <= 0)
                {
                    frame2 = Random.Range(1, 2);
                    Vector3 scale = transform.localScale;
                    scale.x *= (Random.Range(0, 2) == 0 ? -1 : 1);
                    transform.localScale = scale;
                }
                if (frame1 <= 0)
                {
                    anim.SetState(AnimConsts.Morgh_Doon_Khordan);
                }
                break;
        }
	}
}
