using System.Collections;
using System;
using UnityEngine;

public class PetAnimationController : MonoBehaviour {

    public PetHead walker;
    public Animator anim;

    private int lastAnimationPlayedHash;
    private bool isPlayingAnimation;

    public Action OnAnimationComplete;

    private void Start() {
        walker = GetComponent<PetHead>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public void Update() {
        if (isPlayingAnimation && anim.GetCurrentAnimatorStateInfo(0).fullPathHash != lastAnimationPlayedHash)
            OnAnimationCompleteFunction();
    }

    private void OnAnimationCompleteFunction() {
        if (OnAnimationComplete != null)
            OnAnimationComplete.Invoke();
        isPlayingAnimation = false;
        walker.enabled = true;
    }

    public void PlayAnimationOnce(string animationName) {
        isPlayingAnimation = true;
        walker.enabled = false;
        anim.CrossFade(animationName, 1, 0);
        lastAnimationPlayedHash = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
    }

    public void PlayAnimationLooping(string animationName) {
        walker.enabled = false;
        anim.CrossFade(animationName, 1, 0);
        lastAnimationPlayedHash = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
    }
}
