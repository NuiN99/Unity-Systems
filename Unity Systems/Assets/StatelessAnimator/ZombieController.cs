using System.Collections;
using System.Collections.Generic;
using NuiN.StatelessAnimator;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField] StatelessAnimator animator;
    [SerializeField] AnimationClip jumpClip;
    [SerializeField] AnimationClip danceClip;

    [SerializeField] float fadeTime = 1f;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.Play(jumpClip, fadeTime);
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.Play(danceClip, fadeTime, WrapMode.Loop, false);
        }
    }
}
