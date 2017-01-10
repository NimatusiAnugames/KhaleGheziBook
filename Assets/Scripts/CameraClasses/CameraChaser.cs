using UnityEngine;
using System.Collections.Generic;

//Class for map lsayer info
[System.Serializable]
public class MapLayerInfo
{
    public Transform Layer;
    public float ZoomFactor = 1;
    [System.NonSerialized]
    public Vector3 Position;
    [System.NonSerialized]
    public Transform Guid;
}

//This class use for smooth chase a taraget by camera
public class CameraChaser : MonoBehaviour
{
    //Instance of this camera chaser
    public static CameraChaser Instance;

    //Chase parameters
    public Transform Target;
    private bool targetImm;
    public Transform TargetImmediately
    {
        set
        {
            Target = value;
            targetImm = true;
        }
    }
    public float MaxSpeed = 50;
    public float SmoothTime = 0.1f;
    public float ScrollFactor = 0.25f;
    private Vector2 velocity = Vector2.zero;

    //Shake
    private QuakeManager quakeMan;
    private Vector3 camPos;
    public MapLayerInfo Background;
    public MapLayerInfo[] Backs;
    public MapLayerInfo[] Fores;

    //Camera
    [HideInInspector]
    public Camera Cam;

    //Zoom
    public float Zoom = 5;
    private float vel = 0;

    void Awake()
    {
        Instance = Camera.main.GetComponent<CameraChaser>();
        Cam = Camera.main;
        Zoom = Cam.orthographicSize;
        Target.position = transform.position;
    }
    void Start()
    {
        quakeMan = QuakeManager.Instance;
        camPos = transform.position;
        if(Background.Layer)
        {
            Background.Position = Background.Layer.position;
            Background.Guid = Background.Layer.GetChild(0);
            Background.Guid.position = transform.position;
        }
        for (int i = 0; i < Backs.Length; i++)
        {
            MapLayerInfo info = Backs[i];
            info.Position = info.Layer.position;
            info.Guid = info.Layer.GetChild(0);
            info.Guid.position = transform.position;
        }
        for (int i = 0; i < Fores.Length; i++)
        {
            MapLayerInfo info = Fores[i];
            info.Position = info.Layer.position;
            info.Guid = info.Layer.GetChild(0);
            info.Guid.position = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (!Target)
            return;

        #region Scrolling 
        Vector2 pos = camPos;
        Vector2 tPos = Target.position;
        Vector2 fPos = Vector2.SmoothDamp(pos, tPos, ref velocity, SmoothTime, MaxSpeed);
        if (targetImm)
        {
            targetImm = false;
            fPos = tPos;
        }
        Vector3 newPos = fPos;
        newPos.z = camPos.z;
        camPos = newPos;
        transform.position = newPos + quakeMan.Quake.Vector3;

        //Parallax Scrolling
        float factor = 0;
        for (int i = 0; i < Backs.Length; i++)
        {
            MapLayerInfo info = Backs[i];
            factor += ScrollFactor;//ScrollFactor * (i + 1);
            TranslateLayer(info, factor, fPos - pos);
            info.Guid.position = transform.position;
        }
        if (Background.Layer)
        {
            factor = 0.95f; //+= ScrollFactor * 2;
            TranslateLayer(Background, factor, fPos - pos);
            Background.Guid.position = transform.position;
        }
        factor = 0;
        for (int i = 0; i < Fores.Length; i++)
        {
            MapLayerInfo info = Fores[i];
            factor -= ScrollFactor;//ScrollFactor * (i + 1) * -1;
            TranslateLayer(info, factor, fPos - pos);
            info.Guid.position = transform.position;
        }
        #endregion

        #region Zooming

        float orthoSize = Cam.orthographicSize;
        float fSize = Mathf.SmoothDamp(orthoSize, Zoom, ref vel, SmoothTime, MaxSpeed);
        Cam.orthographicSize = fSize;

        //Parallax zoom
        for (int i = 0; i < Backs.Length; i++)
        {
            MapLayerInfo layerInfo = Backs[i];
            factor = ScrollFactor * 0.2f * layerInfo.ZoomFactor; //ScrollFactor * (i + 1) * 0.2f * layerInfo.ZoomFactor;
            layerInfo.Layer.localScale += Vector3.one * (fSize - orthoSize) * factor;
            layerInfo.Position -= layerInfo.Guid.position - transform.position;
        }
        if (Background.Layer)
        {
            factor = ScrollFactor * 0.2f * Background.ZoomFactor; //+= ScrollFactor * -2 * Background.ZoomFactor;
            Background.Layer.localScale += Vector3.one * (fSize - orthoSize) * factor;
            Background.Layer.position -= Background.Guid.position - transform.position;
        }
        for (int i = 0; i < Fores.Length; i++)
        {
            MapLayerInfo layerInfo = Fores[i];
            factor = ScrollFactor * layerInfo.ZoomFactor; //ScrollFactor * (i + 1) * -0.2f * layerInfo.ZoomFactor;
            layerInfo.Layer.localScale += Vector3.one * (fSize - orthoSize) * factor;
            layerInfo.Layer.position -= layerInfo.Guid.position - transform.position;
        }
        #endregion
    }

    private void TranslateLayer(MapLayerInfo layerInfo, float factor, Vector2 movment)
    {
        Transform layer = layerInfo.Layer;

        layer.position = layerInfo.Position;
        layer.Translate(movment * factor);
        Vector3 pos = layer.position;
        pos.z = 0.0f;
        layerInfo.Position = layer.position = pos;
        layer.Translate(quakeMan.Quake.Vector2 * factor);
    }
}
