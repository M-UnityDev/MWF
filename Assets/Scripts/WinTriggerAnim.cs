using System.Collections;
using UnityEngine;
using DG.Tweening;
public class WinTriggerAnim : MonoBehaviour
{
    private void Awake() => StartCoroutine(nameof(WinTrigAnimation));
    private IEnumerator WinTrigAnimation()
    {
        while (transform != null)
        {
            transform.DOMoveY(1,0.5f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.5f);
            transform.DOMoveY(2,0.5f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
