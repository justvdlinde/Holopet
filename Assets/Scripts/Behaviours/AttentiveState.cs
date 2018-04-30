using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentiveState : State {

    private IEnumerator waitTimer;

    public AttentiveState(PetController pet) : base(pet) { }

    public override void Update() { }

    public override void OnStateEnter() {
        pet.walker.enabled = false;
        ArduinoInput.OnMotionDetected += MotionDetected;
    }

    public override void OnStateExit() {
        pet.walker.enabled = true;
        ArduinoInput.OnMotionDetected -= MotionDetected;
    }

    public void MotionDetected(Direction direction) {
        // rotate towards direction
    }
}
