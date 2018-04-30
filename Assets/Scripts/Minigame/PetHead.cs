using System.Collections;
using System;
using UnityEngine;

public class PetHead : MonoBehaviour {

    [SerializeField] private bool useJoints;

    public float MovementSpeed { get; set; }
    public float movementStartingSpeed = 3f;

    public enum TravelMode { Once, Loop, PingPong };
    public BezierSpline spline;
    public TravelMode travelMode;

    public float NormalizedT {
        get {
            return progress;
        } set {
            progress = value;
        }
    }

    [Range(0f, 0.06f)]
    public float relaxationAtEndPoints = 0.01f;
    public float movementLerpModifier = 10f;
    public float rotationLerpModifier = 10f;
    public bool lookForward = true;
    public Action OnTargetReached;
    [SerializeField]
    private bool targetReachedFlag;
    private bool isGoingForward = true;
    private Quaternion initialRotation;
    [SerializeField]
    private float progress = 0f;
    private Transform cachedTransform;

    void Awake() {
        cachedTransform = transform;
        MovementSpeed = movementStartingSpeed;
    }

    public void SlowDown() {
        MovementSpeed *= 0.5f;
    }

    private void Start() {
        initialRotation = transform.parent.rotation;
        PetLimb[] limbs = transform.parent.GetComponentsInChildren<PetLimb>();
        for (int i = 0; i < limbs.Length; i++) {
            // head:
            if(i == 0)
                limbs[i].Init(transform.GetChild(0));
            else 
                limbs[i].Init(limbs[i - 1].transform);

            limbs[i].gameObject.GetComponent<PetLimb>().enabled = !useJoints;
            limbs[i].GetComponent<Rigidbody>().isKinematic = !useJoints;    
        }

        transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(90, 0, 90));
    }

    void Update() {
        float absSpeed = Mathf.Abs(MovementSpeed);
        float targetSpeed = (isGoingForward) ? MovementSpeed : -MovementSpeed;

        Vector3 targetPos;
        if (absSpeed <= 2f)
            targetPos = spline.MoveAlongSpline(ref progress, targetSpeed * Time.deltaTime, maximumError: 0f);
        else if (absSpeed >= 40f)
            targetPos = spline.MoveAlongSpline(ref progress, targetSpeed * Time.deltaTime, increasedAccuracy: true);
        else
            targetPos = spline.MoveAlongSpline(ref progress, targetSpeed * Time.deltaTime);

        //cachedTransform.position = targetPos;
        cachedTransform.position = Vector3.Lerp( cachedTransform.position, targetPos, movementLerpModifier * Time.deltaTime );

        bool movingForward = (MovementSpeed > 0f) == isGoingForward;

        if (lookForward) {
            Quaternion targetRotation;
            if (movingForward) {
                Vector3 lookRotation = spline.GetTangent(progress);
                //lookRotation.x -= 90;
                targetRotation = Quaternion.LookRotation(lookRotation);
            } else {
                targetRotation = Quaternion.LookRotation(-spline.GetTangent(progress));
            }
            cachedTransform.rotation = targetRotation;
            //cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.rotation, targetRotation, rotationLerpModifier * Time.deltaTime);
            //transform.localRotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0));
        }

        if (movingForward) {
            if (progress >= 1f - relaxationAtEndPoints) {

                if (travelMode == TravelMode.Once)
                    progress = 1f;
                else if (travelMode == TravelMode.Loop)
                    progress -= 1f;
                else {
                    progress = 2f - progress;
                    isGoingForward = !isGoingForward;
                }
                if (OnTargetReached != null && !targetReachedFlag) {
                    OnTargetReached.Invoke();
                    targetReachedFlag = true;
                }

            } else {
                targetReachedFlag = false;
            } 
        } else {
            if (progress <= relaxationAtEndPoints) {
                if (travelMode == TravelMode.Once)
                    progress = 0f;
                else if (travelMode == TravelMode.Loop)
                    progress += 1f;
                else {
                    progress = -progress;
                    isGoingForward = !isGoingForward;
                }
            } 
        }
    }

    public void ResetProgress() {
        progress = 0f;
    }
}
