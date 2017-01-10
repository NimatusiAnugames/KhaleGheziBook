using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

//This class used for all items in map That player can interactive
public class InteractiveItem : MonoBehaviour
{
    public UnityEvent ItemEvent;
    [HideInInspector]
    public Collider2D ItemCollider;

    // Use this for initialization
    void Start()
    {
        ItemCollider = GetComponent<Collider2D>();
        //Add this item to manager list
        IntItemManager.Instance.AddItem(transform.position.z, this);
    }
}
