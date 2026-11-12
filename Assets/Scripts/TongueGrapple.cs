using UnityEngine;
using UnityEngine.InputSystem;

// Jezik: lijevi klik na rotkvicu = povuci se do nje. Lijevi klik na neprijatelja u blizini = pojedi ga.
[RequireComponent(typeof(FrogController))]
public class TongueGrapple : MonoBehaviour
{
    public float range = 25f;
    public float aimAngle = 18f;
    public float lashRange = 9f;
    public float tongueSpeed = 80f;
    public float pullSpeed = 22f;
    public float releaseDistance = 1.6f;
    public float maxPullTime = 2f;
    public float releaseUpBoost = 7f;
    public Material tongueMaterial;

    FrogController frog;
    CharacterController controller;
    LineRenderer line;
    Camera cam;

    GrapplePoint aimed, current;
    Enemy aimedEnemy, currentEnemy;
    enum State { Idle, Shooting, Pulling, Lash, Retract }
    State state = State.Idle;
    float tongueLength;
    float pullTimer;
    Vector3 lashTip;

    public bool HasTarget => aimed != null || aimedEnemy != null;
    Vector3 Mouth => transform.position + Vector3.up * 0.55f + transform.forward * 0.45f;

    void Awake()
    {
        frog = GetComponent<FrogController>();
        controller = GetComponent<CharacterController>();
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.2f;
        line.endWidth = 0.14f;
        line.numCapVertices = 4;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.enabled = false;
    }

    void Start()
    {
        cam = Camera.main;
        line.material = tongueMaterial ? tongueMaterial : Fx.Mat("Tongue");
    }

    void Update()
    {
        if (GameManager.Paused) return;
        if (cam == null) cam = Camera.main;
        UpdateAim();

        var mouse = Mouse.current;
        var kb = Keyboard.current;
        bool click = mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState == CursorLockMode.Locked && GameManager.InputEnabled;
        bool jump = kb != null && kb.spaceKey.wasPressedThisFrame;

        switch (state)
        {
            case State.Idle:
                if (click && aimed != null) StartTongue(aimed);
                else if (click && aimedEnemy != null) StartLash(aimedEnemy);
                break;

            case State.Shooting:
                if (current == null || !current.isActiveAndEnabled) { Cancel(); break; }
                tongueLength += tongueSpeed * Time.deltaTime;
                float dist = Vector3.Distance(Mouth, current.transform.position);
                DrawTongue(Mouth + (current.transform.position - Mouth).normalized * Mathf.Min(tongueLength, dist));
                if (tongueLength >= dist)
                {
                    state = State.Pulling;
                    pullTimer = 0f;
                    frog.IsGrappling = true;
                }
                break;

            case State.Pulling:
                if (current == null || !current.isActiveAndEnabled) { Cancel(); break; }
                Pull(jump);
                break;

            case State.Lash:
                tongueLength += tongueSpeed * Time.deltaTime;
                if (currentEnemy == null || !currentEnemy.Alive)
                {
                    lashTip = Mouth + transform.forward * tongueLength;
                    state = State.Retract;
                    break;
                }
                Vector3 enemyPos = currentEnemy.Center;
                float ed = Vector3.Distance(Mouth, enemyPos);
                Vector3 tip = Mouth + (enemyPos - Mouth).normalized * Mathf.Min(tongueLength, ed);
                DrawTongue(tip);
                if (tongueLength >= ed)
                {
                    currentEnemy.Eat();
                    lashTip = tip;
                    tongueLength = ed;
                    state = State.Retract;
                }
                break;

            case State.Retract:
                tongueLength -= tongueSpeed * 1.5f * Time.deltaTime;
                if (tongueLength <= 0f) { line.enabled = false; state = State.Idle; break; }
                DrawTongue(Mouth + (lashTip - Mouth).normalized * tongueLength);
                break;
        }
    }

    void UpdateAim()
    {
        GrapplePoint best = null;
        Enemy bestEnemy = null;
        if (state == State.Idle && cam != null)
        {
            float bestScore = float.MaxValue;
            foreach (var p in GrapplePoint.All)
            {
                if (!p.CanTarget) continue;
                float d = Vector3.Distance(p.transform.position, transform.position);
                if (d > range || d < 2f) continue;
                float angle = Vector3.Angle(cam.transform.forward, p.transform.position - cam.transform.position);
                if (angle > aimAngle) continue;
                if (Blocked(p.transform.position, p.transform)) continue;
                float score = angle + d * 0.2f;
                if (score < bestScore) { bestScore = score; best = p; }
            }

            if (best == null)
            {
                bestScore = float.MaxValue;
                foreach (var s in Enemy.All)
                {
                    if (!s.Alive) continue;
                    float d = Vector3.Distance(s.Center, transform.position);
                    if (d > lashRange) continue;
                    float angle = Vector3.Angle(cam.transform.forward, s.Center - cam.transform.position);
                    if (angle > aimAngle + 8f) continue;
                    if (Blocked(s.Center, s.transform)) continue;
                    float score = angle + d * 0.5f;
                    if (score < bestScore) { bestScore = score; bestEnemy = s; }
                }
            }
        }

        if (aimed != best)
        {
            if (aimed) aimed.SetHighlighted(false);
            aimed = best;
            if (aimed) aimed.SetHighlighted(true);
        }
        aimedEnemy = bestEnemy;
    }

    bool Blocked(Vector3 point, Transform owner)
    {
        Vector3 from = Mouth;
        Vector3 dir = point - from;
        foreach (var hit in Physics.RaycastAll(from, dir.normalized, dir.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.IsChildOf(transform) || hit.transform.IsChildOf(owner)) continue;
            if (hit.distance < dir.magnitude - 1.5f) return true;
        }
        return false;
    }

    void StartTongue(GrapplePoint target)
    {
        current = target;
        state = State.Shooting;
        tongueLength = 0f;
        line.enabled = true;
        FaceTowards(target.transform.position);
        Sfx.Play("tongue", 0.5f);
    }

    void StartLash(Enemy enemy)
    {
        currentEnemy = enemy;
        state = State.Lash;
        tongueLength = 0f;
        line.enabled = true;
        FaceTowards(enemy.transform.position);
        Sfx.Play("tongue", 0.5f);
    }

    void FaceTowards(Vector3 p)
    {
        Vector3 flat = p - transform.position;
        flat.y = 0;
        if (flat.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(flat);
    }

    void Pull(bool jumpPressed)
    {
        pullTimer += Time.deltaTime;
        Vector3 target = current.transform.position - Vector3.up * 0.8f;
        Vector3 toTarget = target - transform.position;
        float d = toTarget.magnitude;
        Vector3 dir = toTarget / Mathf.Max(d, 0.001f);

        Vector3 before = transform.position;
        controller.Move(dir * pullSpeed * Time.deltaTime);
        float moved = (transform.position - before).magnitude;
        DrawTongue(current.transform.position);

        bool stuck = pullTimer > 0.2f && moved < pullSpeed * Time.deltaTime * 0.2f;
        if (d < releaseDistance || pullTimer > maxPullTime || stuck || jumpPressed)
        {
            Vector3 v = dir * pullSpeed * 0.55f;
            v.y = Mathf.Max(v.y, 0f) + releaseUpBoost + (jumpPressed ? 3f : 0f);
            frog.Velocity = v;
            var point = current;
            bool arrived = d < 3.5f;
            Cancel();
            if (arrived && point) point.NotifyGrappled();
        }
    }

    void DrawTongue(Vector3 tip)
    {
        line.SetPosition(0, Mouth);
        line.SetPosition(1, tip);
    }

    public void Cancel()
    {
        state = State.Idle;
        current = null;
        currentEnemy = null;
        if (line) line.enabled = false;
        if (frog) frog.IsGrappling = false;
    }
}
