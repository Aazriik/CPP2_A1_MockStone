using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(BeholderController))]
public class BeholderSplinePatrol : MonoBehaviour
{
    // Follow settings
    public float followRange = 15f;
    public float stopDistance = 4f;
    public float moveSpeed = 3.5f;
    public float turnSpeed = 8f;

    // Spline settings
    public SplineContainer splineContainer;
    public float splineSpeed = 3f;
    public bool loopSpline = true;

    // Return blend settings
    public float returnDuration = 0.35f;
    public int nearestSamples = 120;

    BeholderController core;

    float splineDistance = 0f;
    bool wasInRange = false;

    bool returning = false;
    float returnTimer = 0f;
    Vector3 returnStartPos;
    Quaternion returnStartRot;
    Vector3 returnTargetPos;
    Quaternion returnTargetRot;

    void Awake()
    {
        core = GetComponent<BeholderController>();
    }

    void Start()
    {
        if (!splineContainer) return;

        float bestT = FindNearestT();
        SetDistanceFromT(bestT);

        GetSplinePose(bestT, out var pos, out var rot);
        transform.position = pos;
        transform.rotation = rot;
    }

    void Update()
    {
        if (core.IsDead) return;

        bool inRange = core.player && Vector3.Distance(transform.position, core.player.position) <= followRange;
        core.SetHasTarget(inRange);

        if (returning)
        {
            UpdateReturnBlend();
            wasInRange = inRange;
            return;
        }

        if (wasInRange && !inRange)
        {
            BeginReturnToSpline();
            wasInRange = inRange;
            return;
        }

        if (inRange) FollowPlayer();
        else FollowSpline();

        wasInRange = inRange;
    }

    void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, core.player.position);

        FaceTarget(core.player.position);

        if (dist > stopDistance)
            MoveForward();
    }

    void FollowSpline()
    {
        if (!splineContainer) return;

        var spline = splineContainer.Spline;
        float length = spline.GetLength();
        if (length <= 0.001f) return;

        splineDistance += splineSpeed * Time.deltaTime;

        if (loopSpline) splineDistance %= length;
        else splineDistance = Mathf.Min(splineDistance, length);

        float t = splineDistance / length;

        GetSplinePose(t, out var pos, out var rot);
        transform.position = pos;
        transform.rotation = rot;
    }

    // Return to spline logic when losing target
    void BeginReturnToSpline()
    {
        if (!splineContainer) return;

        float bestT = FindNearestT();
        SetDistanceFromT(bestT);

        GetSplinePose(bestT, out returnTargetPos, out returnTargetRot);

        returning = true;
        returnTimer = 0f;
        returnStartPos = transform.position;
        returnStartRot = transform.rotation;
    }

    void UpdateReturnBlend()
    {
        returnTimer += Time.deltaTime;
        float t = (returnDuration <= 0.001f) ? 1f : Mathf.Clamp01(returnTimer / returnDuration);
        float eased = t * t * (3f - 2f * t);

        transform.position = Vector3.Lerp(returnStartPos, returnTargetPos, eased);
        transform.rotation = Quaternion.Slerp(returnStartRot, returnTargetRot, eased);

        if (t >= 1f) returning = false;
    }

    // ---------- Helpers ----------
    float FindNearestT()
    {
        var spline = splineContainer.Spline;

        float bestT = 0f;
        float bestDist = float.PositiveInfinity;
        int samples = Mathf.Max(10, nearestSamples);

        for (int i = 0; i <= samples; i++)
        {
            float t = i / (float)samples;

            Vector3 localPos = (Vector3)spline.EvaluatePosition(t);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);

            float d = (transform.position - worldPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                bestT = t;
            }
        }

        return bestT;
    }

    void SetDistanceFromT(float t)
    {
        float length = splineContainer.Spline.GetLength();
        splineDistance = Mathf.Clamp01(t) * length;
    }

    void GetSplinePose(float t, out Vector3 worldPos, out Quaternion worldRot)
    {
        var spline = splineContainer.Spline;

        Vector3 localPos = (Vector3)spline.EvaluatePosition(t);
        Vector3 localTan = (Vector3)spline.EvaluateTangent(t);

        worldPos = splineContainer.transform.TransformPoint(localPos);

        Vector3 worldTan = splineContainer.transform.TransformDirection(localTan);
        if (worldTan.sqrMagnitude < 0.0001f) worldTan = transform.forward;

        worldRot = Quaternion.LookRotation(worldTan.normalized);
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    void MoveForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        transform.position += forward.normalized * moveSpeed * Time.deltaTime;
    }
}

