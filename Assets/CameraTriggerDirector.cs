using UnityEngine;
using DG.Tweening;
public class CameraTriggerDirector : MonoBehaviour
{
    private Collider[] Colliders;
    void FixedUpdate()
    {
        Colliders = Physics.OverlapBox(transform.position, Vector3.one * 20, Quaternion.identity);
        foreach (Collider Item in Colliders)
        {
            if (Item.CompareTag("Rotate90"))
            {
                transform.DORotate(Vector3.right*90,1);
                return;
            }
            else if (Item.CompareTag("Rotate0"))
            {
                transform.DORotate(Vector3.zero,1);
                return;
            }
            else if (Item.CompareTag("Rotate180"))
            {
                transform.DORotate(Vector3.right*180,1);
                return;
            }
        }
    }
}
