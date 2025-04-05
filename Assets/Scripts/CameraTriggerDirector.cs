using UnityEngine;
using DG.Tweening;
public class CameraTriggerDirector : MonoBehaviour
{
    [SerializeField] private LayerMask TriggersLayer;
    private void FixedUpdate()
    {
        foreach (Collider Item in Physics.OverlapBox(transform.position, Vector3.one * 5, Quaternion.identity, TriggersLayer))
        {
            switch (Item.CompareTag("Rotate90"), Item.CompareTag("Rotate0"), Item.CompareTag("Rotate180"), Item.CompareTag("RotateY90"))
            {
                case (true,false,false,false):
                    transform.DORotate(Vector3.right*90,1);
                    PlayerPrefs.SetInt("Invert",0);
                    return;
                case (false,true,false,false):
                    transform.DORotate(Vector3.zero,1);
                    PlayerPrefs.SetInt("Invert",0);
                    return;
                case (false,false,true,false):
                    transform.DORotate(Vector3.right*180,1);
                    PlayerPrefs.SetInt("Invert",0);
                    return;
                case (false,false,false,true):
                    transform.DORotate(Vector3.up*90,1);
                    PlayerPrefs.SetInt("Invert",1);
                    return;
            }
        }
    }
}
