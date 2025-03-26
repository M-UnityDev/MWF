using UnityEngine; 
using DG.Tweening; 
using Unity.Burst; 
[BurstCompile] public class DarkDirector : MonoBehaviour, IDarkDirector
{
    [SerializeField] private bool IsDarkOnAwake;
    private SpriteRenderer SpriteDark;
    [SerializeField] private Color Startcolor = new Color(0,0,0,1);
    [SerializeField] private Color Endcolor = new Color(0,0,0,0);
    private void Awake()
    {
        SpriteDark = GetComponent<SpriteRenderer>();
        if(IsDarkOnAwake) SpriteDark.color = Startcolor; UnDark();  
    }
    public void Dark() => SpriteDark.DOColor(Startcolor,1).SetEase(Ease.InOutCubic);
    public void UnDark() => SpriteDark.DOColor(Endcolor,1).SetEase(Ease.InOutCubic);
}
