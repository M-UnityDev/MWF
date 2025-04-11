using UnityEngine;
using DG.Tweening;
public class SpeedItem : MonoBehaviour, IItem
{
    [SerializeField] private int NewSpeed;
    private bool IsRunning;
    public void StartAction(GameObject Player)
    {
        if(!IsRunning)
        {
            IsRunning = true;
            transform.parent.GetComponent<BoxCollider>().enabled = false;
            Player.GetComponent<Movement>().BaseSpeedFuckYou = NewSpeed;
            if (TryGetComponent(out MeshRenderer mesh))mesh.material.DOColor(new Color(0,0,0,0),1).SetEase(Ease.InOutCubic);
        }
    }
}
