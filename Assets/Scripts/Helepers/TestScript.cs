using UnityEngine;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    public SpriteRenderer[] Sprites;
    private int index = 0;
    private int nextIndex = 1;
    private float frame;
    private Color c1, c2;

    void Start()
    {
        c1 = new Color(1, 1, 1, 0);
        c2 = new Color(1, 1, 1, 1);
        for (int i = 1; i < Sprites.Length; i++)
        {
            Sprites[i].color = c1;
        }
    }

    void Update()
    {
        frame += Time.deltaTime * 4;
        Sprites[index].color = Color.Lerp(c2, c1, frame);
        Sprites[nextIndex].color = Color.Lerp(c1, c2, frame);
        if(frame >= 1)
        {
            frame = 0;
            index++;
            if (index == Sprites.Length)
                index = 0;
            nextIndex++;
            if (nextIndex == Sprites.Length)
                nextIndex = 0;
        }
    }
}
