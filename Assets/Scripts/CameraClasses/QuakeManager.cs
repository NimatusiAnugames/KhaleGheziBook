using UnityEngine;

public class QuakeManager : MonoBehaviour
{
    private static QuakeManager instance = null;
    public static QuakeManager Instance
    {
        get 
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ShakeManager");
                instance = obj.AddComponent<QuakeManager>();
            }

            return instance;
        }
    }

	public Quake Quake;

    private void Awake()
    {
        Quake = new Quake();
    }

	private void Update()
	{
		Quake.Update();
	}

	public void SetQuake(float val)
	{
		if (Quake.Value < val) Quake.Value = val;
	}
}


