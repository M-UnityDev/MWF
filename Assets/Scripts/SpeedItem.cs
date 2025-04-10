using UnityEngine;
public class SpeedItem : MonoBehaviour, IItem
{
    [SerializeField] private int NewSpeed;
    public void StartAction(GameObject Player)
    {
        Player.GetComponent<Movement>().BaseSpeedFuckYou = NewSpeed;
    }
}
