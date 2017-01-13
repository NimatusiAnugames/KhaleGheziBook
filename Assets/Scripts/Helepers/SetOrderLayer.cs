using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SetOrderLayer : MonoBehaviour 
{
    public int FirstIndex;
    public int LastIndex;
    private int counter;
    private List<Transform> segments;

    void Start()
    {
        if (Application.isPlaying)
        {
            enabled = false;
            return;
        }
        segments = new List<Transform>();
    }

    void Update()
    {
        counter = 0;
        segments.Clear();
        SetOrder(transform);
        LastIndex = FirstIndex + counter - 1;
        SetZ();
    }

    private void SetOrder(Transform t)
    {
        Renderer sr = t.GetComponent<Renderer>();
        if (sr)
        {
            sr.sortingOrder = FirstIndex + counter;
            counter++;
            segments.Add(t);
        }
        for (int i = 0; i < t.childCount; i++)
        {
            SetOrder(t.GetChild(i));
        }
    }
    private void SetZ()
    {
        float z = 0;
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            Vector3 pos = segments[i].position;
            pos.z = z;
            segments[i].position = pos;
            z += 0.1f;
        }
    }
}
