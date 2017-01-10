using UnityEngine;

public class Quake
{
	private float val;
	public float Value
	{
		get { return val; }
		set { val = value; }
	}
	public Vector2 Vector2
	{
		get
		{
			if (val <= 0f)
				return Vector2.zero;

            return new Vector2(Random.Range(-val, val), Random.Range(-val, val));
		}
	}
    public Vector3 Vector3
    {
        get
        {
            if (val <= 0f)
                return Vector3.zero;

            return new Vector3(Random.Range(-val, val), Random.Range(-val, val), 0);
        }
    }

	public Quake ()
	{
	}

	public void Update()
	{
		if (val > 0f)
			val -= Time.deltaTime;
		else if (val < 0f)
			val = 0f;
	}
}