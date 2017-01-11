using UnityEngine;
using System.Collections.Generic;

public class Plan1Part1  :  MapBase
{
    #region Plan States 

    private const int InitState = -2;
    private const int ShahreFarangIntroState = 0;
    private const int LandAndComingChildrenState = 1;
    private const int MahoorQuestionState = 2;
    private const int ResponseAmooState = 3;
    private const int DoneState = 4;

    #endregion

    #region Fields 
    //Shaher farang fields
    public AnimationItem ShahreFarangDevice;
    public Transform ShahreFarangPlaceHodler;
    private Vector3 shahrefarangFromPos;

    //Amoo fields
    public AnimationItem AmooCharacter;
    private float speechAmooTime = 5;

    //Mahoor fieldsa
    public AnimationItem MahoorCharacter;
    public Transform MahoorPlaceHolder;
    private Vector3 mahoorFromPos;

    //Nava fields
    public AnimationItem NavaCharacter;
    public Transform NavaPlaceHolder;
    private Vector3 navaFromPos;

    //These frames used in lerps
    private float frame1, frame2, frame3, frame4; 
    #endregion

    protected override void Start()
    {
        base.Start();

        //Shahre farang
        shahrefarangFromPos = ShahreFarangDevice.transform.position - new Vector3(9, 0, 0);
        ShahreFarangDevice.transform.position -= new Vector3(9, 0, 0);
        ShahreFarangDevice.EndAction = new System.Action<int>((id) => 
        {
            if(id == AnimConsts.ShahrFarang_Piade_Shodan)
            {
                AmooCharacter.gameObject.SetActive(true);
            }
        });

        //Mahoor character
        mahoorFromPos = MahoorCharacter.transform.position - new Vector3(7, 0, 0);
        MahoorCharacter.transform.position -= new Vector3(7, 0, 0);
        MahoorCharacter.EndAction = new System.Action<int>((id) => 
        {
            if(id == AnimConsts.MahoorChar_Eshare && currentState == MahoorQuestionState)
            {
                Debug.Log("Response amoo State");
                MahoorCharacter.SetState(AnimConsts.MahoorChar_Idle);
                frame4 = speechAmooTime;
                currentState = ResponseAmooState;
                SetResponseAmoo();
            }
        });

        //Nava characters
        navaFromPos = NavaCharacter.transform.position - new Vector3(8, 0, 0);
        NavaCharacter.transform.position -= new Vector3(8, 0, 0);

        //Amoo
        AmooCharacter.EndAction = new System.Action<int>((id) => 
        {
            if(currentState == ResponseAmooState)
            {
                SetResponseAmoo();
            }
        });

        Debug.Log("Shahre farang intro State");
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
            case ShahreFarangIntroState:
                ShahreFarangIntroAction();
                break;
            case LandAndComingChildrenState:
                LandAndComingChildrenAction();
                break;
            case ResponseAmooState:
                ResponseAmooAction();
                break;
        }

        #endregion
    }

    #region State Actions
    private void InitAction()
    {
        ShahreFarangDevice.SetAnimation(AnimConsts.ShahrFarang_Rekab_ZadanClip);
        MahoorCharacter.SetAnimation(AnimConsts.MahoorChar_DavidanClip);
        NavaCharacter.SetAnimation(AnimConsts.NavaChar_DavidanClip);
        AmooCharacter.gameObject.SetActive(false);
        currentState = ShahreFarangIntroState;
    }
    private void ShahreFarangIntroAction()
    {
        frame1 += Time.deltaTime * ShahreFarangDevice.MoveSpeed;
        ShahreFarangDevice.transform.position =
            Vector3.Lerp(shahrefarangFromPos, ShahreFarangPlaceHodler.transform.position, frame1);
        if(frame1 >= 1)
        {
            Debug.Log("Land and coming children State");
            frame2 = 0;
            currentState = LandAndComingChildrenState;
            ShahreFarangDevice.SetState(AnimConsts.ShahrFarang_Piade_Shodan);
            MahoorCharacter.SetState(AnimConsts.MahoorChar_Davidan);
            NavaCharacter.SetState(AnimConsts.NavaChar_Davidan);
        }
    }
    private void LandAndComingChildrenAction()
    {
        //Mahoor
        LerpItem(MahoorCharacter.transform, mahoorFromPos, MahoorPlaceHolder.position,
            ref frame2, MahoorCharacter.MoveSpeed, new System.Action(() => MahoorCharacter.SetState(AnimConsts.MahoorChar_Idle)));

        //Nava
        LerpItem(NavaCharacter.transform, navaFromPos, NavaPlaceHolder.position, ref frame3, NavaCharacter.MoveSpeed,
            new System.Action(() =>
            {
                Debug.Log("Mahoor question State");
                NavaCharacter.SetState(AnimConsts.NavaChar_Idle);
                currentState = MahoorQuestionState;
                MahoorCharacter.SetState(AnimConsts.MahoorChar_Eshare);
            }));
    }
    private void ResponseAmooAction()
    {
        frame4 -= Time.deltaTime;
        if(frame4 <= 0)
        {
            Debug.Log("Done State");
            currentState = DoneState;
            AmooCharacter.SetState(AnimConsts.Amoo_Idle);
        }
    }

    //Set random animation for amoo speech
    private void SetResponseAmoo()
    {
        int state = (Random.Range(0, 2) == 0 ? AnimConsts.Amoo_Harf_Zadan : AnimConsts.Amoo_Harf_Zadan2);
        AmooCharacter.SetState(state);
    }
    #endregion

    #region Interactive Actions 

    public void AmooInteractive()
    {
        if (currentState == DoneState)
        {
            Debug.Log("Amoo interactive");
            frame4 = speechAmooTime;
            currentState = ResponseAmooState;
            SetResponseAmoo();
        }
    }
    public void ShahreFarangInteractive()
    {
        if(currentState > 1)
        {
            if (ShahreFarangDevice.CurrentState == AnimConsts.ShahrFarang_Idle)
            {
                Debug.Log("Shahre farang interactive");
                ShahreFarangDevice.SetState(AnimConsts.ShahrFarang_Akt);
                //Play sound
                //...
            }
        }
    }
    public void MahoorInteractive()
    {
        if(currentState == DoneState)
        {
            if (MahoorCharacter.CurrentState == AnimConsts.MahoorChar_Idle)
            {
                Debug.Log("Mahoor interactive");
                MahoorCharacter.SetState(AnimConsts.MahoorChar_Idle_Moteajeb);
            }
        }
    }
    public void NavaInteractive()
    {
        if(currentState > LandAndComingChildrenState)
        {
            if (NavaCharacter.CurrentState == AnimConsts.NavaChar_Idle)
            {
                Debug.Log("Nava interactive");
                NavaCharacter.SetState(AnimConsts.NavaChar_Eshare);
            }
        }
    }
    public void AbsharInteractive()
    {
        Debug.Log("Abshar interactive");
        //Play sound
        //...
    }
    public void ParvaneInteractive()
    {
        Debug.Log("Parvane interactive");
    }

    #endregion
}
