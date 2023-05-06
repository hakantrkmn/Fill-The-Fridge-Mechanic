using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    private GameStates gameState;
    private int objectLayer;
    private int shoppingCardLayer;

    private void OnEnable()
    {
        EventManager.ChangeGameState += ChangeGameState;
        EventManager.RemoveButtonClicked += RemoveButtonClicked;
        EventManager.DoneButtonClicked += DoneButtonClicked;
    }

    private void DoneButtonClicked()
    {
        EventManager.ChangeGameState(GameStates.ChooseBasket);
    }

    private void ChangeGameState(GameStates obj)
    {
        gameState = obj;
    }

    private void OnDisable()
    {
        EventManager.DoneButtonClicked -= DoneButtonClicked;
        EventManager.ChangeGameState -= ChangeGameState;
        EventManager.RemoveButtonClicked -= RemoveButtonClicked;
    }

    private void RemoveButtonClicked()
    {
        if (gameState==GameStates.PlaceObject)
        {
            EventManager.ChangeGameState(GameStates.RemoveObject);
        }
        else
        {
            EventManager.ChangeGameState(GameStates.PlaceObject);

        }
    }

    private void Start()
    {
        objectLayer = LayerMask.GetMask("Object");
        shoppingCardLayer = LayerMask.GetMask("ShoppingCard");

    }

    private void Update()
    {
        if (gameState==GameStates.RemoveObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity,objectLayer))
                {
                    hit.transform.GetComponentInParent<ObjectController>().RemoveObject();
                }
            }
        }
        else if (gameState==GameStates.ChooseBasket || gameState==GameStates.PlaceObject )
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity,shoppingCardLayer))
                {
                    hit.transform.GetComponent<ShoppingCarController>().ChooseBasket();
                }
            }
        }
    }
}
