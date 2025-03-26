using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    [SerializeField] private DarkDirector Dark;
    private IEnumerator LoadSceneAnimated(string scene)
    {
        Dark.Dark();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
    }
    private IEnumerator ExitAnimated()
    {
        Dark.Dark();
        yield return new WaitForSeconds(1);
        Application.Quit();
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneAnimated(scene));
    }
    public void Exit()
    {
        StartCoroutine(nameof(ExitAnimated));
    }
}
