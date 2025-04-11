using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] Sprites;
    [SerializeField] private GameObject Intro;

    [SerializeField] private GameObject Green;
    [SerializeField] private GameObject Red;
    private Image Image;
    private void Awake()
    {
        Image = GetComponent<Image>();
        //StartAnimation();
    }
    public void StartAnimation()
    {
        StartCoroutine(nameof(Anim));
    }
    private IEnumerator Anim()
    {
        foreach (Sprite s in Sprites)
        {
            Image.sprite = s;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(Intro.GetComponent<SpriteRenderer>());
        Destroy(Green);
        Destroy(Red);
    }
}
