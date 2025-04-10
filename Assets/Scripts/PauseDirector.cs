using UnityEngine;
using Game.Input;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
public class PauseDirector : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private MainUIDirector UIDirector;
    [SerializeField] private DarkDirector Dark;
    private bool Pause;
    public void PauseButton()
    {
        Pause = !Pause;
        Time.timeScale = Pause ? 0 : 1;
        Cursor.lockState = Pause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = Pause;
        if (Pause)
        {
            PausePanel.SetActive(Pause);
            UIDirector.HUGEASSFUCK.Pause();
            PausePanel.GetComponent<CanvasGroup>().DOFade(1, 1).SetUpdate(true);
        }
        else
        {
            UIDirector.HUGEASSFUCK.UnPause();
            PausePanel.GetComponent<CanvasGroup>().DOFade(0, 1).SetUpdate(true).OnComplete(() => {PausePanel.SetActive(Pause);});
        }
    }
    public void Restart()
    {
        Dark.Dark();
        StartCoroutine(nameof(Reatart));
    }
    private IEnumerator Reatart()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        Dark.Dark();
        StartCoroutine(nameof(Eit));
    }
    private IEnumerator Eit()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("MainMenu");
    }
    void Update()
    {
        if (InputHandler.Inputs.Player.Pause.WasReleasedThisFrame())
            PauseButton();
    }
}
