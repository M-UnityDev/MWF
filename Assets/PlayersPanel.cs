using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
public class PlayersPanel : MonoBehaviour
{
    private int PlayerCount;
    private Image Image;
    public void Awake() 
    { 
        Image = GetComponent<Image>();
        Image.color = new Color(0.5f,0,0);
    }
    public void OnJoined(PlayerInput input)
    {
        PlayerCount++;
        print(PlayerCount);
        switch (PlayerCount)
        {
            case 1:
                Image.DOColor(new Color(0, 1, 0, 0.5f), 1);
                break;
            case 2:
                Image.DOColor(new Color(1, 0, 0, 0), 1);
                break;

        }
    }
}
