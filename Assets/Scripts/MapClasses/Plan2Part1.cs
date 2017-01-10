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
    public ShahreFarang ShahreFarangDevice;
    public Mahoor MahoorChar;
    public Nava NavaChar;

    //Amoo
    public Amoo AmooChar;
    private float speechAmooTime = 5;

    //Asal
    public Asal AsalChar;
    public Transform AsalPlace;
    private Vector3 asalfrom;

    //Firooz
    public Firooz FiroozChar;
    public Transform FiroozPlace;
    private Vector3 firoozfrom;

    //Navid
    public Navid NavidChar;
    public Transform NavidPlace;
    private Vector3 navidfrom;

    //Parsa
    public Parsa ParsaChar;
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

        //Mahoor
        

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
        ShahreFarangDevice.SetAnimation(AnimationItem.IdleClip);
        AmooChar.SetAnimation(AnimationItem.IdleClip);
        MahoorChar.SetAnimation(AnimationItem.IdleClip);
        NavaChar.SetAnimation(AnimationItem.IdleClip);
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
            new System.Action(() => AsalChar.SetState(Asal.IdleState)));

        //Firooz
        LerpItem(FiroozChar.transform, firoozfrom, FiroozPlace.position, ref frame3, FiroozChar.MoveSpeed,
            new System.Action(() =>
            {
                Debug.Log("Amoo Speech");
                FiroozChar.SetState(Firooz.IdleState);
                frame6 = speechAmooTime;
                SetSpeechAmoo();
                currentState = AmooSpeech;
            }));

        //Navid
        LerpItem(NavidChar.transform, navidfrom, NavidPlace.position, ref frame4, NavidChar.MoveSpeed,
            new System.Action(() => NavidChar.SetState(Navid.IdleState)));

        //Parsa
        LerpItem(ParsaChar.transform, parsafrom, ParsaPlace.position, ref frame5, ParsaChar.MoveSpeed,
            new System.Action(() => ParsaChar.SetState(Parsa.IdleState)));
    }
    private void AmooSpeechAction()
    {
        frame6 -= Time.deltaTime;
        if (frame6 <= 0)
        {
            Debug.Log("Done State");
            currentState = DoneState;
            AmooChar.SetState(Amoo.IdleState);
        }
    }
    private void SetSpeechAmoo()
    {
        int state = (Random.Range(0, 2) == 0 ? Amoo.Speech1 : Amoo.Speech2);
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
            if (!ShahreFarangDevice.IsInterctiveMode)
            {
                Debug.Log("Shahre farang interactive");
                ShahreFarangDevice.IsInterctiveMode = true;
            }
        }
    }
    public void MahoorInteractive()
    {
        if (currentState == DoneState)
        {
            if (!MahoorChar.IsInterctiveMode)
            {
                Debug.Log("Mahoor interactive");
                MahoorChar.IsInterctiveMode = true;
                MahoorChar.SetState(Mahoor.TalkState);
            }
        }
    }
    public void NavaInteracive()
    {
        if (currentState == DoneState)
        {
            if (!NavaChar.IsInterctiveMode)
            {
                Debug.Log("Nava interactive");
                NavaChar.IsInterctiveMode = true;
                NavaChar.SetState(Nava.TalkState);
            }
        }
    }
    public void AsalInteractive()
    {
        if (currentState == DoneState)
        {
            if (!AsalChar.IsInterctiveMode)
            {
                Debug.Log("Asal interactive");
                AsalChar.IsInterctiveMode = true;
                AsalChar.SetState(Asal.TalkState);
            }
        }
    }
    public void FiroozInteractive()
    {
        if (currentState == DoneState)
        {
            if (!FiroozChar.IsInterctiveMode)
            {
                Debug.Log("Firooz interactive");
                FiroozChar.IsInterctiveMode = true;
                FiroozChar.SetState(Firooz.TalkState);
            }
        }
    }
    public void NavidInteractive()
    {
        if (currentState == DoneState)
        {
            if (!NavidChar.IsInterctiveMode)
            {
                Debug.Log("Navid interactive");
                NavidChar.IsInterctiveMode = true;
                NavidChar.SetState(Navid.TalkState);
            }
        }
    }
    public void ParsaInteractive()
    {
        if (currentState == DoneState)
        {
            if (!ParsaChar.IsInterctiveMode)
            {
                Debug.Log("Parsa interactive");
                ParsaChar.IsInterctiveMode = true;
                ParsaChar.SetState(Parsa.TalkState);
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
