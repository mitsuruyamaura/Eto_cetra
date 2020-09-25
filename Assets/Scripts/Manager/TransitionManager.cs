using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    public CanvasGroup canvasGroup;
    public float duration;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadePanel(float alpha)
    {
        canvasGroup.DOFade(alpha, duration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(duration);
    }
}
