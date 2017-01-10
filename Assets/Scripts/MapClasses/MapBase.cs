using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

//This class a base for all maaps(plans)
//Incluse base and  common behavior
public class MapBase : MonoBehaviour
{
    #region Plan States 

    protected const int NoneState = -1;
    protected int currentState;

    #endregion

    #region Fields 



    #endregion

    //Fade in and out animation parameters
    #region Fade in and out animation

    public Image blackImage;
    public float FadeinSpeed = 2, FadeoutSpeed = 2;
    private float fadeFrame;
    private bool inNavigateMode;
    private bool fadeIn;
    private System.Action FadeInDone;

    public void FadeIn(System.Action Done = null)
    {
        InputManager.Instance.NoAllowInput = true;
        inNavigateMode = true;
        fadeIn = true;
        fadeFrame = 0;
        FadeInDone = Done;
        blackImage.gameObject.SetActive(true);
        blackImage.color = new Color(0, 0, 0, 0);
    }
    public void FadeOut()
    {
        InputManager.Instance.NoAllowInput = true;
        inNavigateMode = true;
        fadeIn = false;
        fadeFrame = 1;
        blackImage.gameObject.SetActive(true);
        blackImage.color = new Color(1, 1, 1, 1);
    }

    #endregion

    protected virtual void Start()
    {
        FadeOut();
        currentState = NoneState;
    }

    protected virtual void Update()
    {
        #region Navigate Animation 
        if(inNavigateMode)
        {
            fadeFrame += Time.deltaTime * (fadeIn ? FadeinSpeed : -FadeoutSpeed);
            blackImage.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeFrame);
            if(fadeIn && fadeFrame >= 1)
            {
                if (FadeInDone != null)
                    FadeInDone();
            }
            else if(!fadeIn && fadeFrame <= 0)
            {
                blackImage.gameObject.SetActive(false);
                inNavigateMode = false;
                InputManager.Instance.NoAllowInput = false;
            }
        }
        #endregion

        #region Map Animations 

        #endregion

        #region States 

        #endregion
    }

    #region State Actions

    #endregion

    #region Interactive Actions 



    #endregion

    #region Helper

    protected void LerpItem(Transform item, Vector3 from, Vector3 to,
        ref float frame, float speed, System.Action complete = null)
    {
        if (frame < 1)
        {
            frame += Time.deltaTime * speed;
            item.position =
                Vector3.Lerp(from, to, frame);
            if (frame >= 1)
            {
                if (complete != null)
                    complete();
            }
        }
    }

    #endregion
}
