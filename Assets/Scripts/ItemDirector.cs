using UnityEngine;
public class ItemDirector : MonoBehaviour
{
    [SerializeField] private ItemEnum ItemType;
    [SerializeField] private GameObject SpeedItem;
    [SerializeField] private GameObject MoveItem;
    [SerializeField] private GameObject DestroyItem;
    private void Awake()
    {
        if (ItemType.Equals(ItemEnum.Random))
        {
            ItemType = (ItemEnum)Random.Range(1,3);
        }
        SpeedItem.SetActive(ItemType.Equals(ItemEnum.Speed));
        MoveItem.SetActive(ItemType.Equals(ItemEnum.Move));
        DestroyItem.SetActive(ItemType.Equals(ItemEnum.Destroy));
    }
}
