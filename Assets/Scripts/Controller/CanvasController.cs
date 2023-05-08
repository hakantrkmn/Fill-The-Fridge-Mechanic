using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject doneButton;
    public GameObject removeButton;
    public Image progress;

    private void OnEnable()
    {
        EventManager.UpdateUIPercent += UpdateUIPercent;
        EventManager.DoneButtonClicked += DoneButtonClicked;
        EventManager.BasketSelected += BasketSelected;
    }

    private void UpdateUIPercent(float obj)
    {
        progress.DOFillAmount(obj/100, .1f);
    }

    private void DoneButtonClicked()
    {
        doneButton.SetActive(false);
        removeButton.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.UpdateUIPercent -= UpdateUIPercent;
        EventManager.DoneButtonClicked -= DoneButtonClicked;
        EventManager.BasketSelected -= BasketSelected;
    }

    private void BasketSelected(BasketController obj)
    {
        doneButton.SetActive(true);
        removeButton.SetActive(true);

    }
}
