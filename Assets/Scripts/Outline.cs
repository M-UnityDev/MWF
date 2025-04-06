using UnityEngine;
public class Outline : MonoBehaviour
{
    [SerializeField] private Transform player;
    public Transform PlayerF {set => player = value;}
    [SerializeField] private GameObject outline;
    public GameObject OutlineF {set => outline = value;}
    [SerializeField] private LayerMask Walls;
    public void viewObstructed() => outline.SetActive(Physics.RaycastAll(transform.position, player.position - transform.position, Vector3.Distance(transform.position, player.transform.position), Walls).Length > 0);
}
