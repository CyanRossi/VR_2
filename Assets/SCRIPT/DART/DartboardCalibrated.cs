using UnityEngine;

public class DartboardCalibrated : MonoBehaviour
{
    [Header("Place these on the board face, exactly on the ring circumferences")]
    public Transform innerBull;  // edge of 50
    public Transform outerBull;  // edge of 25
    public Transform tripleIn;
    public Transform tripleOut;
    public Transform doubleIn;
    public Transform doubleOut;

    [Tooltip("Which plane the board lies on (normal points toward player).")]
    public enum BoardPlane { XY, XZ, YZ }
    public BoardPlane plane = BoardPlane.XY;

    // Sector values clockwise with 20 at top
    static readonly int[] Sectors = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    // Measured radii (meters) filled at runtime
    float rIB, rOB, rTIn, rTOut, rDIn, rDOut;

    // Axis helpers for angle calc
    Vector3 axisRight, axisUp, normal;

    void Awake()
    {
        // Define board plane axes from transform
        switch (plane)
        {
            case BoardPlane.XZ: normal = transform.up; axisRight = transform.right; axisUp = transform.forward; break;
            case BoardPlane.YZ: normal = transform.right; axisRight = transform.forward; axisUp = transform.up; break;
            default: normal = transform.forward; axisRight = transform.right; axisUp = transform.up; break;
        }

        // Measure radii from this object's origin (center). If your origin isn't at bull, it still works—
        // the script measures from transform.position which you should place at the bull if possible.
        Vector3 c = transform.position;
        rIB = DistOnPlane(c, innerBull.position);
        rOB = DistOnPlane(c, outerBull.position);
        rTIn = DistOnPlane(c, tripleIn.position);
        rTOut = DistOnPlane(c, tripleOut.position);
        rDIn = DistOnPlane(c, doubleIn.position);
        rDOut = DistOnPlane(c, doubleOut.position);

        // Basic validation
        if (!(rIB > 0 && rOB > rIB && rTOut > rTIn && rDOut > rDIn))
            Debug.LogWarning("DartboardCalibrated: Check your marker placements.");
    }

    float DistOnPlane(Vector3 center, Vector3 p)
    {
        // Project delta onto board plane before measuring radius
        Vector3 d = p - center;
        Vector3 dProj = Vector3.ProjectOnPlane(d, normal);
        return dProj.magnitude;
    }

    float AngleOnPlane(Vector3 center, Vector3 p)
    {
        // 2D angle in board plane using local right/up axes.
        Vector3 d = p - center;
        Vector3 dProj = Vector3.ProjectOnPlane(d, normal).normalized;
        float x = Vector3.Dot(dProj, axisRight);
        float y = Vector3.Dot(dProj, axisUp);
        float ang = Mathf.Atan2(y, x) * Mathf.Rad2Deg; // -180..180
        if (ang < 0) ang += 360f;

        // Align so 20 is at the top (axisUp direction). Each sector is 18° wide; offset by 9° to center.
        ang = (ang + 9f) % 360f;
        return ang;
    }

    public int GetScoreAtPoint(Vector3 worldPoint)
    {
        Vector3 center = transform.position;
        float r = DistOnPlane(center, worldPoint);

        // Bulls
        if (r <= rIB) return 50;
        if (r <= rOB) return 25;

        // Miss if outside board scoring edge
        if (r > rDOut + 0.001f) return 0;

        // Sector angle
        float ang = AngleOnPlane(center, worldPoint);
        int sectorIndex = Mathf.FloorToInt(ang / 18f); // 0..19
        int baseVal = Sectors[Mathf.Clamp(sectorIndex, 0, 19)];

        // Rings
        if (r >= rTIn && r <= rTOut) return baseVal * 3;
        if (r >= rDIn && r <= rDOut) return baseVal * 2;

        // Singles (between OB→TIn and TOut→DIn)
        if ((r > rOB && r < rTIn) || (r > rTOut && r < rDIn)) return baseVal;

        // Between lines (wires) → 0 (optional: treat as nearest ring)
        return 0;
    }
    // Put inside DartboardCalibrated
    public void DebugResolveHit(Vector3 worldPoint, out DartRing ring, out int sectorOut)
    {
        Vector3 center = transform.position;
        float r = Mathf.Abs(Vector3.ProjectOnPlane(worldPoint - center, normal).magnitude);

        if (r <= rIB) { ring = DartRing.InnerBull; sectorOut = 0; return; }
        if (r <= rOB) { ring = DartRing.OuterBull; sectorOut = 0; return; }
        if (r > rDOut) { ring = DartRing.Miss; sectorOut = 0; return; }
        if (r >= rTIn && r <= rTOut) ring = DartRing.Triple;
        else if (r >= rDIn && r <= rDOut) ring = DartRing.Double;
        else if ((r > rOB && r < rTIn) || (r > rTOut && r < rDIn)) ring = DartRing.Single;
        else ring = DartRing.Miss;

        float ang = AngleOnPlane(center, worldPoint);
        int idx = Mathf.FloorToInt(ang / 18f);
        sectorOut = Sectors[Mathf.Clamp(idx, 0, 19)];
    }


#if UNITY_EDITOR
    // Visualize your measured rings in Scene view
    void OnDrawGizmos()
    {
        if (!(innerBull && outerBull && tripleIn && tripleOut && doubleIn && doubleOut)) return;

        // Recompute in edit-time for live feedback
        Awake();

        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        UnityEditor.Handles.color = new Color(0, 0.7f, 1f, 0.9f);
        DrawDisc(rDOut); DrawDisc(rDIn); DrawDisc(rTOut); DrawDisc(rTIn); DrawDisc(rOB); DrawDisc(rIB);

        // Label measured diameter (use outer double)
        UnityEditor.Handles.Label(transform.position + axisUp * (rDOut + 0.05f),
            $"Measured Double-Out Ø ≈ {(rDOut * 2f):0.000} m");
    }

    void DrawDisc(float r)
    {
        UnityEditor.Handles.DrawWireDisc(transform.position, normal, r);
    }
#endif
}
