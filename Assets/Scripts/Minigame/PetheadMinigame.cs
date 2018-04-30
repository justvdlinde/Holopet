using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame {

    public class PetheadMinigame : PetHead {

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Resource>()) {
                PetMinigame.Instance.OnResourceCollected(other.GetComponent<Resource>());
            }
        }
    }
}