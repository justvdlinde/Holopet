using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Common {

    public static bool IsAboveObject(Transform t, Transform target) {
        return (AngleDir(t.forward, target.position - t.position, t.up) == -1) ? true : false;
    }

    public static bool IsRightFromObject(Transform t, Transform target) {
        return (AngleDir(t.right, target.position - t.position, t.up) == 1) ? true : false;
    }

    private static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f) {
            return 1f;
        } else if (dir < 0f) {
            return -1f;
        } else {
            return 0f;
        }
    }

    public static bool IsFromRight(Vector3 pos, Vector3 target) {
        return (Mathf.Abs(target.x) - Mathf.Abs(pos.x) > 0);
    }

    public static Vector3 GetRandomPositionWithinRange(Vector3 range, Vector3 centre) {
        Vector3 rndLocation = centre;
        rndLocation.x += Random.Range(-range.x / 2, range.x / 2);
        rndLocation.y += Random.Range(-range.y / 2, range.y / 2);
        rndLocation.z += Random.Range(-range.z / 2, range.z / 2);
        return rndLocation;
    }

    public static Vector3 GetRandomPositionWithinRangeNonDividing(Vector3 range, Vector3 centre) {
        Vector3 rndLocation = centre;
        rndLocation.x += Random.Range(-range.x, range.x);
        rndLocation.y += Random.Range(-range.y, range.y);
        rndLocation.z += Random.Range(-range.z, range.z);
        return rndLocation;
    }
}
