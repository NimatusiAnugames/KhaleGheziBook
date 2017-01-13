using UnityEngine;
using System.Collections.Generic;

public class Plan3Part1  :  MapBase
{
    #region Plan States

    private const int CameraIntroState = 0;
    private const int DoneState = 1;

    #endregion

    #region Fields

    //Camera fields
    public float CameraMoveSpeed = 1;
    private Vector3 camFrom;
    private Vector3 camTo;
    public float Camera_X_To;
    private Transform camTarget;

    //These frames used in lerps
    private float frame1;

    #endregion

    protected override void Start()
    {
        base.Start();

        //Camera
        camTarget = CameraChaser.Instance.Target;
        camFrom = CameraChaser.Instance.transform.position;
        camTo = camFrom;
        camTo.x = Camera_X_To;

        Debug.Log("Camera intro state.");
        currentState = CameraIntroState;
    }

    protected override void Update()
    {
        base.Update();

        #region Map Animations

        #endregion

        #region States

        switch (currentState)
        {
            case CameraIntroState:
                CameraIntroAction();
                break;
        }

        #endregion
    }

    #region State Actions

    private void CameraIntroAction()
    {
        LerpItem(camTarget, camFrom, camTo, ref frame1, CameraMoveSpeed,
            new System.Action(() => 
            {
                Debug.Log("State Done.");
                currentState = DoneState;
            }));
    }
    private void StateDoneAction()
    {
        //Scroll camera by input
        //...
    }

    #endregion

    #region Interactive Actions

    public void KhaleHomeInteract()
    {
        if (currentState == DoneState)
        {
            Debug.Log("Kale home interact.");
            MapManager.Instance.Next();
        }
    }
    public void AsiabBadiInteract(AnimationItem item)
    {
        if (item.CurrentState == AnimConsts.AsiabBadi_Idle)
        {
            Debug.Log("Asiab badi interact.");
            item.SetState(AnimConsts.AsiabBadi_Click);
        }
    }
    public void KhoroosInteract(AnimationItem item)
    {
        if (item.CurrentState == AnimConsts.Khoroos_Idle)
        {
            Debug.Log("Khoroos interact.");
            item.SetState(AnimConsts.Khoroos_Ghogholi);
        }
    }
    public void MorghInteract(AnimationItem item)
    {
        Debug.Log("Morgh interact.");
        if (item.CurrentState == AnimConsts.Morgh_Rah_Raftan)
        {

        }
        item.SetState(AnimConsts.Morgh_Click);
    }
    public void SheepsInteract(AnimationItem item)
    {
        Debug.Log("Sheeps interact.");
    }
    public void JoojeInteract(AnimationItem item)
    {
        Debug.Log("Jooje interact.");
        if (item.CurrentState == AnimConsts.Jooje_Davidan2)
        {

        }
        item.SetState(AnimConsts.Jooje_Click);
    }

    #endregion

    #region Helper

    #endregion
}
