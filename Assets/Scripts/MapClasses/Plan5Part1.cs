using UnityEngine;
using System.Collections;

public class Plan5Part1 : MapBase
{
    #region Plan States

    private const int AmooSpeech1State = 0;
    private const int ParsaQuestionState = 1;
    private const int KhaleSpeechState = 2;
    private const int ChildrenResponsesState = 3;
    private const int AmooSpeech2State = 4;
    private const int DoneState = 5;

    #endregion

    #region Fields

    //Amoo
    private float amooSpeech1Time = 1, amooSpeech2Time = 1;

    //Characters
    public AnimationItem ManaChar,
                         ParsaChar,
                         AsalChar,
                         NavaChar,
                         MahoorChar,
                         NavidChar,
                         FiroozChar,
                         Khale;

    //Khale
    private float khaleSpeechTime = 5;

    //Children
    private float childrenResponseTime = 2;

    //These frames used in lerps
    private float frame1, frame2, frame3, frame4;

    #endregion

    protected override void Start()
    {
        base.Start();

        //Parsa
        ParsaChar.EndAction = new System.Action<int>((id) =>
        {
            if (id == 0)
            {
                Debug.Log("Khale speech state.");
                frame2 = khaleSpeechTime;
                currentState = KhaleSpeechState;
            }
        });

        Debug.Log("Amoo speech1 state.");
        frame1 = amooSpeech1Time;
        currentState = AmooSpeech1State;
    }

    protected override void Update()
    {
        base.Update();

        #region Map Animations

        #endregion

        #region States

        #endregion
    }

    #region State Actions

    private void AmooSpeech1Action()
    {
        frame1 -= Time.deltaTime;
        if (frame1 <= 0)
        {
            Debug.Log("Parsa quaestion state.");
            currentState = ParsaQuestionState;
        }
    }
    private void AmooSpeech2Action()
    {
        frame4 -= Time.deltaTime;
        if (frame4 <= 0)
        {
            Debug.Log("Done state.");
            currentState = DoneState;
        }
    }
    private void KhaleSpeechAction()
    {
        frame2 -= Time.deltaTime;
        if (frame2 <= 0)
        {
            Debug.Log("Children response state.");
            frame3 = childrenResponseTime;
            currentState = ChildrenResponsesState;
        }
    }
    private void ChildrenResponseAction()
    {
        frame3 -= Time.deltaTime;
        if (frame3 <= 0)
        {
            Debug.Log("Amoo speech2 state.");
            frame4 = amooSpeech2Time;
            currentState = AmooSpeech2State;
        }
    }

    #endregion

    #region Interactive Actions



    #endregion

    #region Helper

    #endregion
}
