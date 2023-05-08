using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingCardManager : MonoBehaviour
{
    public float percent;
    public List<ShoppingCarController> shoppingCards;


    private void Start()
    {
        SetCards();
    }

    void SetCards()
    {
        foreach (var card in GetComponentsInChildren<ShoppingCarController>())
        {
            shoppingCards.Add(card);
        }
        var gap = shoppingCards[0].groundObj.GetComponent<Collider>().bounds.size.x;
        var size = gap * shoppingCards.Count;
        for (int i = 0; i < shoppingCards.Count; i++)
        {
            shoppingCards[i].transform.localPosition = new Vector3(-((size/2)-2)+(i*gap),0,0);
        }
        Debug.Log(-((size / 2) - 2));
        transform.GetComponent<HorizontalMovement>().playerSettings.defaultMinXClampValue = -((size / 2) - 2);
        transform.GetComponent<HorizontalMovement>().playerSettings.defaultMaxXClampValue = ((size / 2) - 2);

    }


    private void OnEnable()
    {
        EventManager.ObjectRemoved += ObjectRemoved;
        EventManager.ObjectPlaced += ObjectPlaced;
    }

    private void ObjectRemoved(ObjectController obj)
    {
        foreach (var card in shoppingCards)
        {
            card.ObjectRemoved(obj);
        }
        CalculatePercent();
    }

    private void OnDisable()
    {
        EventManager.ObjectRemoved += ObjectRemoved;
        EventManager.ObjectPlaced -= ObjectPlaced;
    }

    void CalculatePercent()
    {
        float tempPercent = 0;
        foreach (var card in shoppingCards)
        {
            card.CalculatePercent();
            tempPercent += card.percent;
        }

        percent = (100 * tempPercent) / (shoppingCards.Count * 100);
        EventManager.UpdateUIPercent(percent);
    }

    private void ObjectPlaced(BasketController arg1, ObjectController arg2)
    {
     CalculatePercent();  
    }
}
