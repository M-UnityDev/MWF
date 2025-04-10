using System.Collections;
using UnityEngine;
using Unity.Burst;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
[BurstCompile]
public class Timer : MonoBehaviour
{
    private int TimeInSeconds = 300;
    [SerializeField] private UnityEngine.Rendering.Volume Volume;
    [SerializeField] private TMP_Text TimeText;
    [SerializeField] private DarkDirector Dark;
    [SerializeField] private CanvasGroup GameOverCanvas;
    [SerializeField] private TMP_Text GameOverText;
    [SerializeField] private Image GameOverButton;
    public void StartTimer(string PlayerName, Color PlayerColor) => StartCoroutine(Time(PlayerName, PlayerColor));
    private IEnumerator Time(string PlayerName, Color PlayerColor)
    {
        TimeText.DOColor(Color.red,TimeInSeconds).SetEase(Ease.Linear);
        while(TimeInSeconds !< 0 || TimeInSeconds != 0)
        {
            yield return new WaitForSeconds(1);
            TimeInSeconds -= 1;
            TimeText.text = (TimeInSeconds/60%60).ToString() + ":" + (TimeInSeconds%60 < 10 ? "0" : null) +(TimeInSeconds%60).ToString();
        }
        StartCoroutine(StartWin(PlayerName, PlayerColor,0));
    }
    public void StartWinScreen(string PlayerName, Color PlayerColor)
    {
        StopAllCoroutines();
        StartCoroutine(StartWin(PlayerName, PlayerColor,3));
    }
    private IEnumerator StartWin(string PlayerName, Color PlayerColor, int Delay)
    {
        yield return new WaitForSeconds(Delay);
        DOTween.To(()=> Volume.weight, x=> Volume.weight = x, 1,1);
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1);
        GameOverCanvas.gameObject.SetActive(true);
        GameOverText.color = PlayerColor;
        GameOverButton.color = PlayerColor;
        GameOverText.text = PlayerName + " WON";
        GameOverCanvas.DOFade(1,0.25f).SetEase(Ease.InOutCubic);
    }
}