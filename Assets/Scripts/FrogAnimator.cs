using System.Collections.Generic;
using UnityEngine;

// Povezuje kretanje žabice s Animatorom modela (ako model ima Animator).
// Parametri koje Animator Controller može imati (svaki je neobavezan):
//   Speed (float)  – vodoravna brzina žabice
//   Grounded (bool) – stoji li na tlu
//   Jump (trigger) – upravo je skočila
//   Tongue (bool)  – jezik je vani
[RequireComponent(typeof(FrogController))]
public class FrogAnimator : MonoBehaviour
{
    static readonly int SpeedId = Animator.StringToHash("Speed");
    static readonly int GroundedId = Animator.StringToHash("Grounded");
    static readonly int JumpId = Animator.StringToHash("Jump");
    static readonly int TongueId = Animator.StringToHash("Tongue");

    Animator anim;
    FrogController frog;
    TongueGrapple tongue;
    readonly HashSet<int> has = new HashSet<int>();

    void Start()
    {
        frog = GetComponent<FrogController>();
        tongue = GetComponent<TongueGrapple>();
        anim = GetComponentInChildren<Animator>();
        if (anim == null || anim.runtimeAnimatorController == null) { anim = null; return; }

        anim.applyRootMotion = false;   // žabicu pomiče FrogController, ne animacija
        foreach (var p in anim.parameters) has.Add(p.nameHash);
        frog.Jumped += OnJump;
    }

    void OnDestroy()
    {
        if (frog) frog.Jumped -= OnJump;
    }

    void OnJump()
    {
        if (anim && has.Contains(JumpId)) anim.SetTrigger(JumpId);
    }

    void Update()
    {
        if (!anim) return;
        Vector3 v = frog.Velocity;
        v.y = 0f;
        if (has.Contains(SpeedId)) anim.SetFloat(SpeedId, v.magnitude, 0.1f, Time.deltaTime);
        if (has.Contains(GroundedId)) anim.SetBool(GroundedId, frog.IsGrounded);
        if (has.Contains(TongueId)) anim.SetBool(TongueId, tongue && tongue.IsBusy);
    }
}
