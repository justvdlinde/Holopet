using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Minigame;

public class PathfinderMinigame : MonoBehaviour {

    [SerializeField] private BezierSpline spline;
    [SerializeField] public PetHead walker;
    [SerializeField] private float minDistanceForExtraSplinePoint = 3f;
    [SerializeField] private float middleSplineLengthModifierMax = 0.4f;
    [SerializeField] private float middleSplineAngleEmotionHappyModifier = 30;

    private Animator animator;

    public void SetNewRandomResourceTarget() {
        Resource rndResource = Minigame.Resources.GetRandomDependingOnIncentive(PetMinigame.Instance.IncentiveToCollectHappyResource, walker.transform.position);

        if (rndResource) {
            SetNewTargetPosition(rndResource.transform.position);
        } else {
            Vector3 rndPos = Common.GetRandomPositionWithinRange(ResourceSpawner.Instance.range, ResourceSpawner.Instance.transform.position);
            SetNewTargetPosition(rndPos);
        }
    }

    public void SetNewTargetPosition(Vector3 pos) {
        walker.enabled = true;
        spline.endPoints[spline.Count - 1].position = walker.transform.position;
        BezierPoint p = spline.InsertNewPointAtWorldPosition(spline.Count, pos);
        p.precedingControlPointPosition *= 1.5f;
        Quaternion lookRot = Quaternion.LookRotation(p.position - walker.transform.position, Vector3.forward);
        Vector3 conversion = new Vector3(0, lookRot.eulerAngles.y - 90, 0);
        p.rotation = Quaternion.Euler(conversion);

        spline.RemovePointAt(0);
        if (spline.Count > 2)
            spline.RemovePointAt(0);

        if (Vector3.Distance(walker.transform.position, p.position) > minDistanceForExtraSplinePoint) {
            p = spline.InsertNewPointAt(1);
            float splineLength = spline.Length * middleSplineLengthModifierMax;
            p.precedingControlPointLocalPosition = new Vector3(splineLength, p.precedingControlPointLocalPosition.y, p.precedingControlPointLocalPosition.z);
            lookRot = Quaternion.LookRotation(p.position - walker.transform.position, Vector3.forward);
            float angleModifier = 0;
            if (PetMinigame.Instance.lastEmotionCollected == Emotion.Happy)
                angleModifier = middleSplineAngleEmotionHappyModifier;
            conversion = new Vector3(0, (lookRot.eulerAngles.y + 90) + angleModifier, 0);
            p.rotation = Quaternion.Euler(conversion);
        }
        walker.ResetProgress();
    }

    public void Stop() {
        walker.enabled = false;
   
    }

    //public void SetNewTargetAnticipatingObjectPosition(Transform target, Vector3 direction, float speed) {
    //    float travelTime = spline.Length / walker.MovementSpeed;
    //    Vector3 futurePosition = target.transform.position + (direction * (speed * travelTime));
    //    SetNewTargetPosition(futurePosition);
    //}

    private GameObject CreateVisualMesh(Vector3 pos) {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pos.y = -0.3f;
        g.GetComponent<Renderer>().material.color = Color.black;
        g.transform.position = pos;
        g.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(g.GetComponent<Collider>());
        return g;
    }

    public const float MAX_RAY_DISTANCE = 100000f;

    public Vector3 GetMousePositionInScene(float y = 0) {
        Plane plane = new Plane(Vector3.up, y);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance = MAX_RAY_DISTANCE;
        if (plane.Raycast(ray, out rayDistance)) {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            return hitPoint;
        }
        return Vector3.zero;
    }
}
