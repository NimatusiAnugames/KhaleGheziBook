using UnityEngine;
using System.Collections.Generic;

public class IntItemManager : MonoBehaviour
{
    //Static instance of this class...This class is a singleton
    private static IntItemManager instance = null;
    public static IntItemManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("InteractiveItemManager");
                instance = obj.AddComponent<IntItemManager>();
            }
            return instance;
        }
    }

    //List of interactive items that sorted by position.z
    private SortedList<float, InteractiveItem> itemList = new SortedList<float,InteractiveItem>();
    public void AddItem(float posZ, InteractiveItem item)
    {
        itemList.Add(posZ, item);
    }


	// Update is called once per frame
	void Update()
	{
        for (int i = 0; i < itemList.Keys.Count; i++)
        {
            float key = itemList.Keys[i];
            InteractiveItem item = itemList[key];
            if (InputManager.Instance.InputClick && 
                item.ItemCollider.OverlapPoint(InputManager.Instance.InputWorldPostion))
            {
                item.ItemEvent.Invoke();
                break;
            }
        }
	}
}
