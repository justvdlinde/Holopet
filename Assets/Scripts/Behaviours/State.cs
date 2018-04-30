using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {

    protected PetController pet;

    public State(PetController pet) {
        this.pet = pet;
    }

    public abstract void Update();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}
