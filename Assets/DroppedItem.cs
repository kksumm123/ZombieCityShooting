using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DropItemType
{
    Gold,
    Point,
    Item,
}
public class DroppedItem : MonoBehaviour
{
    [SerializeField] DropItemType type;
    [SerializeField] int amount;
    [SerializeField] int itemID;
    [SerializeField] Color testColor = Color.white;
    bool isAttaced = false;
    void OnTriggerEnter(Collider other)
    {
        if (isAttaced == true)
            return;
        if (other.CompareTag("Player"))
        {
            isAttaced = true;
            switch (type)
            {
                case DropItemType.Gold:
                    StageManager.Instance.AddGold(amount);
                    break;
                case DropItemType.Point:
                    break;
                case DropItemType.Item:
                    break;
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
