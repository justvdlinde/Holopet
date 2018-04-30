using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State {

    private float waitTime = 8f;
    private float timer;
    private Coroutine waitCoroutine;

    public IdleState(PetController pet) : base(pet) { }

    public override void Update() { }

    public override void OnStateEnter() {
        pet.pathFinder.SetNewRandomTarget();
        pet.walker.enabled = true;
        pet.walker.OnTargetReached += pet.pathFinder.SetNewRandomTarget;
        ArduinoInput.OnMotionDetected += MotionDetected;
    }

    public override void OnStateExit() {
        pet.walker.OnTargetReached -= pet.pathFinder.SetNewRandomTarget;
        ArduinoInput.OnMotionDetected -= MotionDetected;
    }

    public void MotionDetected(Direction direction) {
        pet.walker.OnTargetReached -= pet.pathFinder.SetNewRandomTarget;
        pet.animController.PlayAnimationLooping("Looking");
        if (waitCoroutine != null)
            timer = 0;
        else
            waitCoroutine = pet.StartCoroutine(WaitTimerAfterMotionDetected());
        pet.pathFinder.LookAtRealWorldPosition(direction);
    }

    private IEnumerator WaitTimerAfterMotionDetected() {
        pet.animController.PlayAnimationOnce("Default");
        timer = 0;
        while (timer < waitTime) {
            timer += Time.deltaTime;
            yield return null;
        }
        pet.walker.OnTargetReached += pet.pathFinder.SetNewRandomTarget;
    }
}
