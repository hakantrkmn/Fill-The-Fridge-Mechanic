using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasketSpace : MonoBehaviour
{
    public BasketController basketController;
    public BasketSpace topSpace;
    public bool filled;
    public ObjectController spaceObject;
    public Collider collider;
    public Transform placePos;
    public float volume;
    private void Start()
    {
        basketController = GetComponentInParent<BasketController>();

        

    }

    private void OnEnable()
    {
        EventManager.ObjectRemoved += ObjectRemoved;
        EventManager.EnableColliders += EnableColliders;
    }

    private void ObjectRemoved(ObjectController obj)
    {
        if (spaceObject==obj)
        {
            spaceObject = null;
            filled = false;
            EventManager.UpdatePercent(basketController);
            basketController.SetBaseSpaces();
        }
    }


    private void OnDisable()
    {
        EventManager.ObjectRemoved -= ObjectRemoved;
        EventManager.EnableColliders -= EnableColliders;
    }

    private void EnableColliders()
    {
        if (!filled)
        {
            GetComponent<Collider>().enabled = true;
        }
    }


    public void SpaceFilled(ObjectController placedObject)
    {
        filled = true;
        spaceObject = placedObject;
        GetComponent<Collider>().enabled = false;
        if (topSpace != null)
        {
            topSpace.OpenSpace();
        }
    }

    public void OpenSpace()
    {
        filled = false;
        GetComponent<Collider>().enabled = true;
    }
    public void CloseSpace()
    {
        GetComponent<Collider>().enabled = false;
    }

    public void CheckIfObjectFit()
    {
    }
}