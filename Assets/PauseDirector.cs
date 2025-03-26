using UnityEngine;
using Game.Input;
using DG.Tweening;
public class PauseDirector : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
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
            PausePanel.GetComponent<CanvasGroup>().DOFade(1, 1).SetUpdate(true);
        }
        else
        {
            PausePanel.GetComponent<CanvasGroup>().DOFade(0, 1).SetUpdate(true).OnComplete(() => {PausePanel.SetActive(Pause);});
        }
    }
    void Update()
    {
        if (InputHandler.Inputs.Player.Pause.WasReleasedThisFrame())
            PauseButton();
    }
}
