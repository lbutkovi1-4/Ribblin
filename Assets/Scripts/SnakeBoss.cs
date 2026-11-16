using System.Collections.Generic;
using UnityEngine;

// BOSS 2 – Venomaw, the Swamp Serpent (zmija iz močvare).
// Vijuga oko žabice. Ima dva napada:
//  1) propne se i zaleti glavom – ako promaši, zabije glavu u tlo i ostane zaglavljena
//     (tad joj skoči na glavu ili je jezikom uhvati za glavu),
//  2) podigne rep i udari njime o tlo – od repa se širi otrovni val koji treba preskočiti.
public class SnakeBoss : BossBase
{
    public Transform head;           // model glave (dijete ovog objekta)
    public Transform[] segments;     // dijelovi tijela (djeca ovog objekta), [0] je najbliži glavi
    public GrapplePoint headPoint;   // točka na glavi za jezik – aktivna samo dok je zaglavljena
    public GameObject stunFx;        // zvjezdice iznad glave
    public float spacing = 1.1f;     // razmak između dijelova tijela
    public float slitherSpeed = 6f;
    public float strikeSpeed = 20f;
    public float stuckTime = 3.5f;
    public float circleRadius = 8f;  // koliko daleko kruži oko žabice

    enum S { Slither, Rear, Strike, Stuck, Recover, TailUp, TailSlam }
    S s = S.Slither;
    readonly List<Vector3> trail = new List<Vector3>();   // put kojim je prošla glava
    float orbitAngle, wiggle, headLift, tailLift, speedMul = 1f, hintCooldown;
    int wavesLeft;
    bool tailHintShown;
    float strikeLeft;
    float grace;   // kratko nakon pogotka tijelo ne ozljeđuje (žabica je baš na glavi)
    int attackCount;
    bool rearForStrike;
    Vector3 strikeDir;

    float GroundY => arenaCenter.y;

    protected override void Start()
    {
        base.Start();
        // početni trag: glava pa redom svi dijelovi tijela (zmija je na početku sklupčana)
        trail.Add(transform.position);
        if (segments != null) foreach (var seg in segments) trail.Add(seg.position);
        if (headPoint)
        {
            headPoint.targetable = false;
            if (headPoint.visual) headPoint.visual.gameObject.SetActive(false);
            headPoint.Grappled += _ => { if (s == S.Stuck) StuckHit(); };
        }
        if (stunFx) stunFx.SetActive(false);
        orbitAngle = Random.value * Mathf.PI * 2f;
    }

    protected override void Update()
    {
        base.Update();
        // kuglica za jezik vidi se samo dok je glava zaglavljena
        if (headPoint && headPoint.visual)
        {
            bool show = s == S.Stuck && !dead;
            if (headPoint.visual.gameObject.activeSelf != show) headPoint.visual.gameObject.SetActive(show);
        }
    }

    protected override void OnActivate()
    {
        s = S.Slither;
        stateTimer = 2.5f;
    }

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        hintCooldown -= Time.deltaTime;
        grace -= Time.deltaTime;

        // skok na glavu dok nije zaglavljena ne pomaže
        if (s != S.Stuck && IsStomped(2.0f, GroundY + 1.6f + headLift))
        {
            frog.Bounce(3.5f);
            if (hintCooldown <= 0f) { GameManager.ShowMessage("Her scales are too tough! Make her strike and miss.", 3f); hintCooldown = 4f; }
        }

        switch (s)
        {
            case S.Slither:
                {
                    // kruži oko žabice i vijuga
                    orbitAngle += 0.6f * speedMul * Time.deltaTime;
                    Vector3 goal = PlayerPos + new Vector3(Mathf.Cos(orbitAngle), 0, Mathf.Sin(orbitAngle)) * circleRadius;
                    goal = ClampToArena(goal, 2.5f);
                    MoveTowards(goal, slitherSpeed * speedMul, true);
                    headLift = Mathf.MoveTowards(headLift, 0.6f, 3f * Time.deltaTime);
                    if (stateTimer <= 0f) StartRear();
                    break;
                }

            case S.Rear:
                FaceTowards(PlayerPos, 8f);
                headLift = Mathf.MoveTowards(headLift, 2.8f, 8f * Time.deltaTime);
                if (head) head.localPosition = new Vector3(Random.Range(-0.05f, 0.05f), headLift, 0);
                if (stateTimer <= 0f) StartStrike();
                break;

            case S.TailUp:
                // rep se diže i trese – upozorenje prije udarca
                tailLift = Mathf.MoveTowards(tailLift, 3.2f, 5f * Time.deltaTime);
                FaceTowards(PlayerPos, 3f);
                if (stateTimer <= 0f) { s = S.TailSlam; stateTimer = 0.12f; }
                break;

            case S.TailSlam:
                tailLift = Mathf.MoveTowards(tailLift, 0f, 40f * Time.deltaTime);
                if (stateTimer <= 0f && tailLift <= 0.01f)
                {
                    SlamWave();
                    wavesLeft--;
                    if (wavesLeft > 0) { s = S.TailUp; stateTimer = 0.55f; }
                    else { s = S.Recover; stateTimer = 0.9f; }
                }
                break;

            case S.Strike:
                {
                    headLift = Mathf.MoveTowards(headLift, 0.2f, 12f * Time.deltaTime);
                    float step = strikeSpeed * speedMul * Time.deltaTime;
                    transform.position += strikeDir * step;
                    strikeLeft -= step;
                    RecordTrail();
                    bool edge = Flat(transform.position, arenaCenter) > arenaRadius - 2f;
                    if (strikeLeft <= 0f || edge || stateTimer <= 0f)
                    {
                        transform.position = ClampToArena(transform.position, 2f);
                        GetStuck();
                    }
                    break;
                }

            case S.Stuck:
                headLift = Mathf.MoveTowards(headLift, 0.1f, 6f * Time.deltaTime);
                if (stunFx) stunFx.transform.Rotate(0, 200f * Time.deltaTime, 0);
                if (stateTimer < stuckTime - 0.5f && IsStomped(2.2f, GroundY + 1.6f))
                {
                    frog.Bounce(5f);
                    StuckHit();
                }
                else if (stateTimer <= 0f) EndStuck();
                break;

            case S.Recover:
                headLift = Mathf.MoveTowards(headLift, 0.6f, 4f * Time.deltaTime);
                FaceTowards(PlayerPos, 4f);
                tailLift = Mathf.MoveTowards(tailLift, 0f, 6f * Time.deltaTime);
                if (stateTimer <= 0f) { s = S.Slither; stateTimer = Random.Range(3f, 4f) / speedMul; }
                break;
        }

        if (head && s != S.Rear) head.localPosition = new Vector3(0, headLift, 0);
        PlaceBody();
        if (s != S.Stuck && grace <= 0f) BodyDamage();
    }

    void StartRear()
    {
        attackCount++;
        rearForStrike = attackCount % 2 == 1;   // naizmjence: zalet glavom, pa udarac repom
        Attack(0.9f);   // siktanje
        if (rearForStrike)
        {
            s = S.Rear;
            stateTimer = 1.1f;   // jasno upozorenje prije zaleta
        }
        else
        {
            s = S.TailUp;
            stateTimer = 1.0f;
            wavesLeft = health <= 1 ? 3 : 2;   // kad je slaba, udari tri puta
            if (!tailHintShown) { GameManager.ShowMessage("Her tail! Jump over the venom wave!", 2.5f); tailHintShown = true; }
        }
    }

    // Udarac repom o tlo: otrovni val se širi od repa kroz cijelu arenu.
    void SlamWave()
    {
        Vector3 tailPos = segments != null && segments.Length > 0 ? segments[segments.Length - 1].position : transform.position;
        tailPos.y = GroundY + 0.05f;
        Shockwave.Spawn(tailPos, 8f + speedMul * 1.5f, arenaRadius * 2f, "Venom", 0.7f);
        Sfx.Play("snake_spit", 0.9f);
        Sfx.Play("stomp", 0.7f);
        ThirdPersonCamera.Shake(0.35f);
        Fx.Puff(tailPos + Vector3.up * 0.3f, "Venom", 2.5f);
        Fx.Burst(tailPos + Vector3.up * 0.3f, "Venom", 8, 0.8f);
    }

    void StartStrike()
    {
        strikeDir = PlayerPos - transform.position;
        strikeDir.y = 0;
        if (strikeDir.sqrMagnitude < 0.01f) strikeDir = transform.forward;
        float dist = strikeDir.magnitude;
        strikeDir /= dist;
        strikeLeft = dist + 3f;      // zaleti se malo preko mjesta gdje je žabica bila
        transform.rotation = Quaternion.LookRotation(strikeDir);
        s = S.Strike;
        stateTimer = 1.5f;
    }

    void GetStuck()
    {
        s = S.Stuck;
        stateTimer = stuckTime;
        Sfx.Play("stomp", 0.8f);
        ThirdPersonCamera.Shake(0.5f);
        Fx.Puff(transform.position + transform.forward * 1.5f, "Dust", 3f);
        if (stunFx) stunFx.SetActive(true);
        if (headPoint)
        {
            headPoint.targetable = true;
            if (headPoint.visual) headPoint.visual.gameObject.SetActive(true);
        }
        GameManager.ShowMessage("Her head is stuck! Jump on it or grab it with your tongue!", 2.5f);
    }

    void EndStuck()
    {
        if (stunFx) stunFx.SetActive(false);
        if (headPoint)
        {
            headPoint.targetable = false;
            if (headPoint.visual) headPoint.visual.gameObject.SetActive(false);
        }
        s = S.Recover;
        stateTimer = 0.8f;
    }

    void StuckHit()
    {
        grace = 1.5f;
        EndStuck();
        Hit();
    }

    protected override void OnHit()
    {
        speedMul *= 1.15f;   // sa svakim pogotkom brža
    }

    void MoveTowards(Vector3 goal, float speed, bool wiggleOn)
    {
        Vector3 to = goal - transform.position;
        to.y = 0;
        if (to.magnitude < 0.3f) { RecordTrail(); return; }
        Vector3 dir = to.normalized;
        if (wiggleOn)
        {
            wiggle += Time.deltaTime * 5f;
            dir = (dir + Vector3.Cross(Vector3.up, dir) * Mathf.Sin(wiggle) * 0.5f).normalized;
        }
        transform.position += dir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, GroundY, transform.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
        RecordTrail();
    }

    void RecordTrail()
    {
        if (trail.Count == 0 || Vector3.Distance(trail[0], transform.position) > 0.1f)
        {
            trail.Insert(0, transform.position);
            if (trail.Count > 600) trail.RemoveAt(trail.Count - 1);
        }
        else trail[0] = transform.position;
    }

    // Dijelovi tijela prate trag glave; prvih nekoliko se podiže zajedno s glavom.
    void PlaceBody()
    {
        if (segments == null) return;
        Vector3 prev = transform.position;
        for (int i = 0; i < segments.Length; i++)
        {
            Vector3 p = PointOnTrail(spacing * (i + 1));
            float lift = i < 4 ? headLift * (1f - (i + 1) / 5f) : 0f;
            int fromTail = segments.Length - 1 - i;   // 0 = vrh repa
            if (fromTail < 5) lift += tailLift * (1f - fromTail / 5f) + (s == S.TailUp ? Random.Range(-0.05f, 0.05f) : 0f);
            segments[i].position = new Vector3(p.x, GroundY + lift, p.z);
            Vector3 look = prev - segments[i].position;
            look.y = 0;
            if (look.sqrMagnitude > 0.0001f) segments[i].rotation = Quaternion.LookRotation(look);
            prev = segments[i].position;
        }
    }

    Vector3 PointOnTrail(float distance)
    {
        float acc = 0f;
        for (int j = 1; j < trail.Count; j++)
        {
            float d = Vector3.Distance(trail[j - 1], trail[j]);
            if (acc + d >= distance) return Vector3.Lerp(trail[j - 1], trail[j], (distance - acc) / Mathf.Max(d, 0.0001f));
            acc += d;
        }
        return trail[trail.Count - 1];
    }

    // Dodir s glavom ili tijelom = -1 srce
    void BodyDamage()
    {
        if (!frog) return;
        Vector3 fp = frog.transform.position;
        if (Flat(fp, transform.position) < 1.6f && fp.y < GroundY + 1.0f + headLift)
        {
            PlayerHealth.Instance?.Damage(1, transform.position);
            return;
        }
        if (segments == null) return;
        for (int i = 0; i < segments.Length; i++)
        {
            float r = Mathf.Lerp(0.9f, 0.4f, i / (float)segments.Length);
            Vector3 sp = segments[i].position;
            if (Flat(fp, sp) < r + 0.4f && fp.y < sp.y + r * 1.6f && fp.y > sp.y - 1f)
            {
                PlayerHealth.Instance?.Damage(1, sp);
                return;
            }
        }
    }
}
