using UnityEngine;
using DG.Tweening;
public class MoveItem : MonoBehaviour, IItem
{
    [SerializeField] private Transform ObjectToMove;
    [SerializeField] private bool IsCollectable;
    [SerializeField] private bool IsIncrease;
    [SerializeField] private Vector3 CoordinateToMove;
    [SerializeField] private Ease EaseForMove;
    [SerializeField] private float TimeForMove;
    [SerializeField] private GameObject AnotherMoveItem;
    private bool IsRunning;
    private void Awake()
    {
        if (!transform.parent.GetComponent<BoxCollider>().enabled)
            GetComponent<MeshRenderer>().material.color = Color.blue*0.5f;
    }
    public void StartAction(GameObject Player)
    {
        if(!IsRunning)
        {
            IsRunning = true;
            transform.parent.GetComponent<BoxCollider>().enabled = false;
            if (TryGetComponent(out MeshRenderer mesh))mesh.material.DOColor(new Color(0,0,0,0),TimeForMove).SetEase(EaseForMove);
            ObjectToMove.DOLocalMove(IsIncrease ? ObjectToMove.localPosition + CoordinateToMove : CoordinateToMove,TimeForMove).SetEase(EaseForMove).OnComplete(()=>{
                if(IsCollectable) Destroy(gameObject.transform.parent.gameObject);
                if (!AnotherMoveItem.Equals(null))
                {
                    AnotherMoveItem.transform.parent.GetComponent<BoxCollider>().enabled = true;
                    AnotherMoveItem.GetComponent<MeshRenderer>().material.DOColor(Color.blue,1).SetEase(EaseForMove);
                }
            });
        }
    }
}
