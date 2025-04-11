using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class IntroDirector : MonoBehaviour
{
    [SerializeField] private Sprite Logo1;
    [SerializeField] private Sprite Logo2;
    [SerializeField] private RectTransform Green;
    [SerializeField] private RectTransform Red;
    [SerializeField] private GameObject Intro;
    [SerializeField] private Image LogoImage;
    [SerializeField] private SpriteAnimation SPRA;
    private void Awake()
    {
        StartCoroutine(nameof(Anima));
    }
    private IEnumerator Anima()
    {
        yield return new WaitForSeconds(1);
        LogoImage.sprite = Logo1;
        yield return new WaitForSeconds(1.6f);
        LogoImage.sprite = Logo2;
        yield return new WaitForSeconds(3.4f);
        Green.DOSizeDelta(new Vector2(-960,0),0.25f).SetEase(Ease.OutCubic).OnComplete(()=>Red.DOSizeDelta(new Vector2(-960,1080),0.25f).SetDelay(0.1f).SetEase(Ease.OutCubic).OnComplete(()=>SPRA.StartAnimation()));
        yield return new WaitForSeconds(1);
        Destroy(Intro.GetComponent<SpriteRenderer>());
        Destroy(Green.gameObject);
        Destroy(Red.gameObject);
        Destroy(LogoImage.gameObject);
    }
}
