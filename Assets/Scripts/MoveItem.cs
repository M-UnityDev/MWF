using UnityEngine;
using DG.Tweening;
public class MoveItem : MonoBehaviour, IItem
{
    [SerializeField] private Transform ObjectToMove;
    [SerializeField] private Vector3 CoordinateToMove;
    [SerializeField] private Ease EaseForMove;
    [SerializeField] private float TimeForMove;
    public void StartAction(GameObject Player)
    {
        ObjectToMove.DOLocalMove(CoordinateToMove,TimeForMove).SetEase(EaseForMove);
    }
}
