using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRotationHack : MonoBehaviour {

    [SerializeField]
    private Vector3 wantedRotation = new Vector3(180, 90, 90);

	void Update () {
        transform.localEulerAngles = wantedRotation;
	}

    private void LateUpdate() {
        transform.localEulerAngles = wantedRotation;
    }
}
