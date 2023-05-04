using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasketSpace : MonoBehaviour
{
    private BasketController basketController;
    public BasketSpace topSpace;
    public bool filled;
    public ObjectController spaceObject;
    public Collider collider;
    public Transform placePos;
    private void Start()
    {
        basketController = GetComponentInParent<BasketController>();
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.up, 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log(hits[i].transform.name);
        }
        
        Debug.Log(collider.bounds.size.y);
    }

    private void OnEnable()
    {
        EventManager.EnableColliders += EnableColliders;
    }

  

    private void OnDisable()
    {
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