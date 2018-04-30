using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    public Vector3 swimRange = new Vector3(5, 7, 5);

    [SerializeField] private BezierSpline spline;
    [SerializeField] public PetHead walker;

    [SerializeField] private Transform right, left, front;

    public void SetNewRandomTarget() {
        Vector3 rndPos = Common.GetRandomPositionWithinRange(swimRange, transform.position);
        SetNewTargetPosition(rndPos);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, swimRange);
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

        walker.ResetProgress();
    }

    public void SetNewTargetPositionLookAt(Vector3 pos, Vector3 forward) {
        walker.enabled = true;
        spline.endPoints[spline.Count - 1].position = walker.transform.position;
        BezierPoint p = spline.InsertNewPointAtWorldPosition(spline.Count, pos);
        p.precedingControlPointPosition *= 1.5f;

        if (forward == Vector3.right)
            p.precedingControlPointPosition = new Vector3(-1, 0, 0);
        else if(forward == Vector3.left)
            p.precedingControlPointPosition = new Vector3(1, 0, 0);
        else
            p.precedingControlPointPosition = new Vector3(0, 0, 1);

        spline.RemovePointAt(0);
        if (spline.Count > 2)
            spline.RemovePointAt(0);

        walker.ResetProgress();
    }

    public void LookAtRealWorldPosition(Direction direction) {
        if(direction == Direction.Right)
            SetNewTargetPositionLookAt(right.position, Vector3.right);
        else if (direction == Direction.Left)
            SetNewTargetPositionLookAt(left.position, Vector3.left);
        else if (direction == Direction.Front)
            SetNewTargetPositionLookAt(front.position, Vector3.forward);
    }

    public void Stop() {
        walker.enabled = false;
    }
}
