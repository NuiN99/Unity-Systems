using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    // ReSharper disable Unity.PerformanceAnalysis
    public static Coroutine DoAfter(this MonoBehaviour behaviour, float seconds, Action onComplete)
    {
        return behaviour.StartCoroutine(DoAfterCoroutine(seconds, onComplete));
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public static Coroutine DoWhen(this MonoBehaviour behaviour, Func<bool> condition, Action onComplete)
    {
        return behaviour.StartCoroutine(DoWhenCoroutine(condition, onComplete));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    static IEnumerator DoAfterCoroutine(float seconds, Action onComplete)
    {
        yield return new WaitForSeconds(seconds);
        onComplete?.Invoke();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    static IEnumerator DoWhenCoroutine(Func<bool> condition, Action onComplete)
    {
        yield return new WaitUntil(condition);
        onComplete?.Invoke();
    }
}
