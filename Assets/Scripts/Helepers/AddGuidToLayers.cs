using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AddGuidToLayers : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (Application.isPlaying)
        {
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Transform guid = child.Find("Guid");
            if (guid) guid.SetAsFirstSibling();
            else
            {
                GameObject obj = new GameObject("Guid");
                guid = obj.transform;
                guid.parent = child;
                guid.localPosition = guid.localScale = Vector3.zero;
                guid.SetAsFirstSibling();
            }
        }
    }
}
