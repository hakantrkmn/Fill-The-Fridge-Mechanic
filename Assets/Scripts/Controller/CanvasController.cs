using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject doneButton;
    public GameObject removeButton;


    private void OnEnable()
    {
        EventManager.DoneButtonClicked += DoneButtonClicked;
        EventManager.BasketSelected += BasketSelected;
    }

    private void DoneButtonClicked()
    {
        doneButton.SetActive(false);
        removeButton.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.DoneButtonClicked -= DoneButtonClicked;
        EventManager.BasketSelected -= BasketSelected;
    }

    private void BasketSelected(BasketController obj)
    {
        doneButton.SetActive(true);
        removeButton.SetActive(true);

    }
}
