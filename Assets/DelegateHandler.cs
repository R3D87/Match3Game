using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateHandler : MonoBehaviour
{
    public delegate void AnimationEventDelegate();
    public static AnimationEventDelegate animationEventDelegate;

    public void OnButtonClick()
    {
        animationEventDelegate();
    }

}
