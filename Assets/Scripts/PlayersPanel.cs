using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayersPanel : MonoBehaviour
{
    private int PlayerCount;
    private VibrationDirector Vibrator;
    private Image Image;
    private CanvasGroup CanvasGroup;
    public void Awake() 
    { 
        Vibrator = FindFirstObjectByType<VibrationDirector>();
        Image = GetComponent<Image>();
        CanvasGroup = GetComponent<CanvasGroup>();
        CanvasGroup.alpha = 1;
        Image.color = new Color(0.5f,0,0,0.9f);
    }
    public void OnJoined()
    {
        Vibrator.VibrateOnce(1,0.3f,0.3f);
        PlayerCount++;
        switch (PlayerCount)
        {
            case 1:
                Image.DOColor(new Color(0,0.5f,0,0.5f),1);
                break;
            case 2:
                CanvasGroup.DOFade(0,1);
                Image.DOColor(new Color(0,1,0,0),1).OnComplete(() => {Destroy(Image);});
                break;
        }
    }
}
