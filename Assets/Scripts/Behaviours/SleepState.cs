using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState : State {

    public SleepState(PetController pet) : base(pet) { }

    public override void Update() { }

    public override void OnStateEnter() {
        pet.animController.PlayAnimationLooping("Sleeping");
    }

    public override void OnStateExit() {
        pet.animController.PlayAnimationOnce("Default");
    }
}
