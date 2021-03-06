using UnityEngine;
using System.Collections.Generic;

//This class is a base class for all animation items incluse Characters, Map segments and so on
public class AnimationItem : MonoBehaviour
{
    #region States

    protected const string AnimStateParam = "State";
    public const int NonState = -1;
    private int curState = -1;
    public int CurrentState
    {
        get { return curState; }
        set
        {
            curState = value;
            SetState(curState);
        }
    }

    //Names
    public const string IdleClip = "Idle";

    #endregion

    //Animator instance
    protected Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        //AnimStateBehaviour[] anims = animator.GetBehaviours<AnimStateBehaviour>();
        //for (int i = 0; i < anims.Length; i++)
        //{
        //    anims[i].animationitem = this;
        //}
    }

    //This method set current state for item
    public virtual void SetState(int state)
    {
        animator.SetInteger(AnimStateParam, state);
    }

    //This method change animation clip
    public void SetAnimation(string name)
    {
        animator.Play(name);
    }

    //This methods call in AnimationStateBehaviour class
    public virtual void StartState(int id)
    {
        if (StartAction != null)
            StartAction(id);
    }
    public virtual void EndState(int id)
    {
        if (EndAction != null)
            EndAction(id);
    }

    //This delegates implement in Map class for item behaviour
    public System.Action<int> StartAction;
    public System.Action<int> EndAction;

    //Animator state parameter value
    public int AnimatorState
    {
        get
        {
            return animator.GetInteger(AnimStateParam);
        }
    }

    //Animator playback speed
    public float AnimationSpeed
    {
        get { return animator.speed; }
        set { animator.speed = value; }
    }

    //Speed of transfer position
    public float MoveSpeed = 1;
}
