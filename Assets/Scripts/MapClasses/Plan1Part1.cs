using UnityEngine;
using System.Collections.Generic;

public class Plan1Part1  :  MapBase
{
    #region Plan States 

    private const int ShahreFarangIntroState = 0;
    private const int LandAndComingChildrenState = 1;
    private const int MahoorQuestionState = 2;
    private const int ResponseAmooState = 3;
    private const int DoneState = 4;

    #endregion

    #region Fields 
    //Shaher farang fields
    public ShahreFarang ShahreFarangDevice;
    public Transform ShahreFarangPlaceHodler;
    private Vector3 shahrefarangFromPos;

    //Amoo fields
    public Amoo AmooCharacter;
    private float speechAmooTime = 5;

    //Mahoor fieldsa
    public Mahoor MahoorCharacter;
    public Transform MahoorPlaceHolder;
    private Vector3 mahoorFromPos;

    //Nava fields
    public Nava NavaCharacter;
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
        ShahreFarangDevice.StartAction = new System.Action<int>((id) =>
        {
            if (id == ShahreFarang.IdleState)
            {
                ShahreFarangDevice.IsInterctiveMode = false;
                ShahreFarangDevice.SetState(ShahreFarang.IdleState);
            }
        });
        ShahreFarangDevice.EndAction = new System.Action<int>((id) => 
        {
            if(currentState == LandAndComingChildrenState)
            {
                AmooCharacter.gameObject.SetActive(true);
            }
        });

        //Mahoor character
        mahoorFromPos = MahoorCharacter.transform.position - new Vector3(7, 0, 0);
        MahoorCharacter.transform.position -= new Vector3(7, 0, 0);
        MahoorCharacter.StartAction = new System.Action<int>((id) =>
        {
            if(id == Mahoor.IdleState)
            {
                MahoorCharacter.IsInterctiveMode = false;
                MahoorCharacter.SetState(Mahoor.IdleState);
            }
        });
        MahoorCharacter.EndAction = new System.Action<int>((id) => 
        {
            if(id == Mahoor.TalkState && currentState == MahoorQuestionState)
            {
                Debug.Log("Response amoo State");
                MahoorCharacter.SetState(Mahoor.IdleState);
                frame4 = speechAmooTime;
                currentState = ResponseAmooState;
                SetResponseAmoo();
            }
        });

        //Nava characters
        navaFromPos = NavaCharacter.transform.position - new Vector3(8, 0, 0);
        NavaCharacter.transform.position -= new Vector3(8, 0, 0);
        NavaCharacter.StartAction = new System.Action<int>((id) => 
        {
            if(id == Nava.IdleState)
            {
                NavaCharacter.IsInterctiveMode = false;
                NavaCharacter.SetState(Nava.IdleState);
            }
        });

        //Amoo
        AmooCharacter.gameObject.SetActive(false);
        AmooCharacter.EndAction = new System.Action<int>((id) => 
        {
            if(currentState == ResponseAmooState)
            {
                SetResponseAmoo();
            }
        });

        Debug.Log("Shahre farang intro State");
        currentState = ShahreFarangIntroState;
    }

    protected override void Update()
    {
        base.Update();

        #region Map Animations 

        #endregion

        #region States 

        switch (currentState)
        {
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
            ShahreFarangDevice.SetState(ShahreFarang.AmooLandState);
            MahoorCharacter.SetState(Mahoor.RunState);
            NavaCharacter.SetState(Nava.RunState);
        }
    }
    private void LandAndComingChildrenAction()
    {
        //Mahoor
        LerpItem(MahoorCharacter.transform, mahoorFromPos, MahoorPlaceHolder.position,
            ref frame2, MahoorCharacter.MoveSpeed, new System.Action(() => MahoorCharacter.SetState(Mahoor.IdleState)));

        //Nava
        LerpItem(NavaCharacter.transform, navaFromPos, NavaPlaceHolder.position, ref frame3, NavaCharacter.MoveSpeed,
            new System.Action(() =>
            {
                Debug.Log("Mahoor question State");
                NavaCharacter.SetState(Nava.IdleState);
                currentState = MahoorQuestionState;
                MahoorCharacter.SetState(Mahoor.TalkState);
            }));
    }
    private void ResponseAmooAction()
    {
        frame4 -= Time.deltaTime;
        if(frame4 <= 0)
        {
            Debug.Log("Done State");
            currentState = DoneState;
            AmooCharacter.SetState(Amoo.IdleState);
        }
    }

    //Set random animation for amoo speech
    private void SetResponseAmoo()
    {
        int state = (Random.Range(0, 2) == 0 ? Amoo.Speech1 : Amoo.Speech2);
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
            if (!ShahreFarangDevice.IsInterctiveMode)
            {
                Debug.Log("Shahre farang interactive");
                ShahreFarangDevice.IsInterctiveMode = true;
                ShahreFarangDevice.SetState(ShahreFarang.ActState);
                //Play sound
                //...
            }
        }
    }
    public void MahoorInteractive()
    {
        if(currentState == DoneState)
        {
            if (!MahoorCharacter.IsInterctiveMode)
            {
                Debug.Log("Mahoor interactive");
                MahoorCharacter.IsInterctiveMode = true;
                MahoorCharacter.SetState(Mahoor.IdleSurprisedState);
            }
        }
    }
    public void NavaInteractive()
    {
        if(currentState > 1)
        {
            if (!NavaCharacter.IsInterctiveMode)
            {
                Debug.Log("Nava interactive");
                NavaCharacter.IsInterctiveMode = true;
                NavaCharacter.SetState(Nava.TalkState);
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
