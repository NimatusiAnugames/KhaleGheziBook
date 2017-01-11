using UnityEngine;
using System.Collections.Generic;

public class Plan2Part1  :  MapBase
{
    #region Plan States 

    private const int InitState = -2;
    private const int ZoomCameraState = 0;
    private const int ChildrenCommingState = 1;
    private const int AmooSpeech = 2;
    private const int DoneState = 3;

    #endregion

    #region Fields 

    //Camera zoom parameters
    public float ZoomTo = 3.48f;
    public float ZoomSpeed = 1;
    private float zoomFrom;

    //Characters
    public AnimationItem ShahreFarangDevice;
    public AnimationItem MahoorChar;
    public AnimationItem NavaChar;

    //Amoo
    public AnimationItem AmooChar;
    private float speechAmooTime = 5;

    //Asal
    public AnimationItem AsalChar;
    public Transform AsalPlace;
    private Vector3 asalfrom;

    //Firooz
    public AnimationItem FiroozChar;
    public Transform FiroozPlace;
    private Vector3 firoozfrom;

    //Navid
    public AnimationItem NavidChar;
    public Transform NavidPlace;
    private Vector3 navidfrom;

    //Parsa
    public AnimationItem ParsaChar;
    public Transform ParsaPlace;
    private Vector3 parsafrom;

    //These frames used in lerps
    private float frame1, frame2, frame3, frame4, frame5, frame6;

    #endregion

    protected override void Start()
    {
        base.Start();

        //Camera
        zoomFrom = CameraChaser.Instance.Zoom;

        //Amoo
        AmooChar.EndAction = new System.Action<int>((id) =>
        {
            if (currentState == AmooSpeech)
            {
                SetSpeechAmoo();
            }
        });

        currentState = InitState;
    }

    protected override void Update()
    {
        base.Update();

        #region Map Animations 

        #endregion

        #region States 

        switch (currentState)
        {
            case InitState:
                InitAction();
                break;
            case ZoomCameraState:
                ZoomCameraAction();
                break;
            case ChildrenCommingState:
                ChildrenCommingAction();
                break;
            case AmooSpeech:
                AmooSpeechAction();
                break;
        }

        #endregion
    }

    #region State Actions

    private void InitAction()
    {
        //Characters
        ShahreFarangDevice.SetAnimation(AnimConsts.ShahrFarang_IdleClip);
        AmooChar.SetAnimation(AnimConsts.Amoo_IdleClip);
        MahoorChar.SetAnimation(AnimConsts.MahoorChar_IdleClip);
        NavaChar.SetAnimation(AnimConsts.NavaChar_IdleClip);
        AsalChar.SetAnimation(AnimConsts.Asal_DavidanClip);
        NavidChar.SetAnimation(AnimConsts.Navid_DavidanClip);
        FiroozChar.SetAnimation(AnimConsts.Firooz_DavidanClip);
        ParsaChar.SetAnimation(AnimConsts.Parsa_DavidanClip);
        asalfrom = AsalPlace.position + new Vector3(8, 0, 0);
        AsalChar.transform.position = asalfrom;
        firoozfrom = FiroozPlace.position + new Vector3(8, 0, 0);
        FiroozChar.transform.position = firoozfrom;
        navidfrom = NavidPlace.position + new Vector3(8, 0, 0);
        NavidChar.transform.position = navidfrom;
        parsafrom = ParsaPlace.position + new Vector3(8, 0, 0);
        ParsaChar.transform.position = parsafrom;

        Debug.Log("Camera zomming state");
        currentState = ZoomCameraState;
    }
    private void ZoomCameraAction()
    {
        frame1 += Time.deltaTime * ZoomSpeed;
        CameraChaser.Instance.Zoom = Mathf.Lerp(zoomFrom, ZoomTo, frame1);

        if(frame1 >= 1)
        {
            Debug.Log("Children comming state");
            currentState = ChildrenCommingState;
        }
    }
    private void ChildrenCommingAction()
    {
        //Asal
        LerpItem(AsalChar.transform, asalfrom, AsalPlace.position, ref frame2, AsalChar.MoveSpeed,
            new System.Action(() => AsalChar.SetState(AnimConsts.Asal_Idle)));

        //Firooz
        LerpItem(FiroozChar.transform, firoozfrom, FiroozPlace.position, ref frame3, FiroozChar.MoveSpeed,
            new System.Action(() =>
            {
                Debug.Log("Amoo Speech");
                FiroozChar.SetState(AnimConsts.Firooz_Idle);
                frame6 = speechAmooTime;
                SetSpeechAmoo();
                currentState = AmooSpeech;
            }));

        //Navid
        LerpItem(NavidChar.transform, navidfrom, NavidPlace.position, ref frame4, NavidChar.MoveSpeed,
            new System.Action(() => NavidChar.SetState(AnimConsts.Navid_Idle)));

        //Parsa
        LerpItem(ParsaChar.transform, parsafrom, ParsaPlace.position, ref frame5, ParsaChar.MoveSpeed,
            new System.Action(() => ParsaChar.SetState(AnimConsts.Parsa_Idle)));
    }
    private void AmooSpeechAction()
    {
        frame6 -= Time.deltaTime;
        if (frame6 <= 0)
        {
            Debug.Log("Done State");
            currentState = DoneState;
            AmooChar.SetState(AnimConsts.Amoo_Idle);
        }
    }
    private void SetSpeechAmoo()
    {
        int state = (Random.Range(0, 2) == 0 ? AnimConsts.Amoo_Harf_Zadan : AnimConsts.Amoo_Harf_Zadan2);
        AmooChar.SetState(state);
    }

    #endregion

    #region Interactive Actions 

    public void AmooInteactive()
    {
        Debug.Log("Amoo interactive");
        if (currentState == DoneState)
        {
            frame6 = speechAmooTime;
            SetSpeechAmoo();
            currentState = AmooSpeech;
        }
    }
    public void ShahreFarangInteractive()
    {
        if (currentState == DoneState)
        {
            if (ShahreFarangDevice.CurrentState == AnimConsts.ShahrFarang_Idle)
            {
                Debug.Log("Shahre farang interactive");
            }
        }
    }
    public void MahoorInteractive()
    {
        if (currentState == DoneState)
        {
            if (MahoorChar.CurrentState == AnimConsts.MahoorChar_Idle)
            {
                Debug.Log("Mahoor interactive");
                MahoorChar.SetState(AnimConsts.MahoorChar_Eshare);
            }
        }
    }
    public void NavaInteracive()
    {
        if (currentState == DoneState)
        {
            if (NavaChar.CurrentState == AnimConsts.NavaChar_Idle)
            {
                Debug.Log("Nava interactive");
                NavaChar.SetState(AnimConsts.NavaChar_Eshare);
            }
        }
    }
    public void AsalInteractive()
    {
        if (currentState == DoneState)
        {
            if (AsalChar.CurrentState == AnimConsts.Asal_Idle)
            {
                Debug.Log("Asal interactive");
                //AsalChar.SetState(AnimConsts);
            }
        }
    }
    public void FiroozInteractive()
    {
        if (currentState == DoneState)
        {
            if (FiroozChar.CurrentState == AnimConsts.Firooz_Idle)
            {
                Debug.Log("Firooz interactive");
                //FiroozChar.SetState(Firooz.TalkState);
            }
        }
    }
    public void NavidInteractive()
    {
        if (currentState == DoneState)
        {
            if (NavidChar.CurrentState == AnimConsts.Navid_Idle)
            {
                Debug.Log("Navid interactive");
                //NavidChar.SetState(Navid.TalkState);
            }
        }
    }
    public void ParsaInteractive()
    {
        if (currentState == DoneState)
        {
            if (ParsaChar.CurrentState == AnimConsts.Parsa_Idle)
            {
                Debug.Log("Parsa interactive");
                //ParsaChar.SetState(Parsa.TalkState);
            }
        }
    }
    public void AbsharInteractive()
    {
        Debug.Log("Abshar interactive");
        //Play sound
        //...
    }

    #endregion
}
