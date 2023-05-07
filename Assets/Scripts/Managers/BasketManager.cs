using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public Transform basketsParent;
    public Transform selectedPos;
    public List<BasketController> baskets;
    public List<BasketVariables> basketStats;
    public float percent;

    private void OnEnable()
    {
        EventManager.UpdatePercent += UpdatePercent;
        EventManager.ObjectPlaced += ObjectPlaced;
        EventManager.GetSelectedTranform += () => selectedPos;
    }

    private void OnDisable()
    {
        EventManager.UpdatePercent -= UpdatePercent;
        EventManager.ObjectPlaced -= ObjectPlaced;
    }

    private void UpdatePercent(BasketController basket)
    {
        basket.CalculatePercent();
        CalculatePercent();
    }

    public void CalculatePercent()
    {
        float fillBasketAmount = 0;
        foreach (var basketController in baskets)
        {
            fillBasketAmount += basketController.percent;
        }

        percent = (100 * fillBasketAmount) / (baskets.Count * 100);
    }

    private void ObjectPlaced(BasketController basket, ObjectController obj)
    {
        basket.objects.Add(obj);
        basket.CalculatePercent();
        CalculatePercent();
    }

    void GetBaskets()
    {
        foreach (var basket in basketsParent.GetComponentsInChildren<BasketController>())
        {
            baskets.Add(basket);
        }

        foreach (var basket in baskets)
        {
            var temp = new BasketVariables();
            temp.basket = basket;
            temp.percent = 0;
            basketStats.Add(temp);
        }
    }

    private void Start()
    {
        GetBaskets();
    }
}