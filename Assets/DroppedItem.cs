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
    public enum GetMethodType
    {
        TriggerEnter,
        KeyDown,
    }
    [SerializeField] GetMethodType getMethod;
    [SerializeField] KeyCode keyCode = KeyCode.E;
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
            switch (getMethod)
            {
                case GetMethodType.TriggerEnter:
                    GetItem();
                    break;
                case GetMethodType.KeyDown:
                    enabled = true;
                    break;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            enabled = false;
    }
    void Awake()
    {
        enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            enabled = false;
            GetItem();
        }
    }

    private void GetItem()
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
