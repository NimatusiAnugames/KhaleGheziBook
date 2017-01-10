using UnityEngine;

public class Plan4Part1  :  MapBase
{
    #region Plan States 

    private const int TranslateCameraState = 0;
    private const int AmooSpeech1State = 1;
    private const int ChildrenCommingState = 2;
    private const int AmooSpeech2State = 3;
    private const int KhaleCommingState = 4;
    private const int KhaleSpeechState = 5;

    #endregion

    #region Fields 

    //Camera fields
    public CameraChaser CamChaser;
    public float CamZoomSpeed = 1;
    public float CamMoveSpeed = 1;
    private Transform CameraTarget;
    private Vector3 cameraFromPos;
    private Vector3 cameraToPos;
    private float fromZoom;
    public float ToZoom;
    public float StartWait = 1;

    //Foregrounds fields
    public Animator Fore3Animator, Fore4Animator;

    //Amoo fields
    private float speech1Frame = 2, speech2Frame = 1.5f;

    //Frame params for lerps
    private float frame1, frame2;

    #endregion

    protected override void Start()
    {
        base.Start();

        //Camera
        CameraTarget = CamChaser.Target;
        cameraFromPos = CameraTarget.position;
        cameraToPos = new Vector3(2, 0, 0);
        fromZoom = CamChaser.Zoom;

        //Foregrounds
        Fore3Animator.enabled = false;
        Fore4Animator.enabled = false;

        currentState = TranslateCameraState;
    }

    protected override void Update()
    {
        base.Update();

        #region Map Animations 

        #endregion

        #region States 

        switch (currentState)
        {
            case TranslateCameraState:
                TranslateCamAction();
                break;
            case AmooSpeech1State:
                AmooSpeech1Action();
                break;
            case ChildrenCommingState:
                ChildrenCommingAction();
                break;
            case AmooSpeech2State:
                AmooSpeech2Action();
                break;
            case KhaleCommingState:
                KhaleCommingAction();
                break;
            case KhaleSpeechState:
                KhaleSpeechAction();
                break;
        }

        #endregion
    }

    #region State Actions

    private void TranslateCamAction()
    {
        StartWait -= Time.deltaTime;
        if (StartWait > 0)
            return;

        frame1 += Time.deltaTime * CamMoveSpeed;
        frame2 += Time.deltaTime * CamZoomSpeed;
        CameraTarget.position = Vector3.Lerp(cameraFromPos, cameraToPos, frame1);
        CamChaser.Zoom = Mathf.Lerp(fromZoom, ToZoom, frame2);

        if (Fore4Animator.enabled == false)
            Fore4Animator.enabled = true;

        if (Fore3Animator.enabled == false)
            Fore3Animator.enabled = true;

        if(frame2 >= 1)
        {
            CameraChaser.Instance.Fores[3].Layer.gameObject.SetActive(false);
            CameraChaser.Instance.Fores[4].Layer.gameObject.SetActive(false);
        }

        if (frame1 >= 1)
        {
            currentState = AmooSpeech1State;
        }
    }
    private void AmooSpeech1Action()
    {
        speech1Frame -= Time.deltaTime;
        if(speech1Frame <= 0)
        {
            currentState = ChildrenCommingState;
        }
    }
    private void ChildrenCommingAction()
    {

    }
    private void AmooSpeech2Action()
    {
        speech2Frame -= Time.deltaTime;
        if (speech2Frame <= 0)
        {
            currentState = KhaleCommingState;
        }
    }
    private void KhaleCommingAction()
    {

    }
    private void KhaleSpeechAction()
    {

    }

    #endregion

    #region Interactive Actions 



    #endregion
}
