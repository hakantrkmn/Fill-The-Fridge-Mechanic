using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ShoppingCarController : MonoBehaviour
{
    public List<ObjectController> objects;
    private int objectIndex;
    public bool isSelected;
    public ObjectController currentObject;
    private int layer_mask;

    private void Start()
    {
        layer_mask = LayerMask.GetMask("BasketSpace");
        foreach (var obj in GetComponentsInChildren<ObjectController>())
        {
            objects.Add(obj);
        }
    }

    private void OnMouseDown()
    {
        isSelected = true;
        currentObject = objects.First();
        objectIndex++;
    }

    private Vector3 overlapPos;

    private void Update()
    {
        if (isSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
                {
                    EventManager.EnableColliders();


                    var space = hit.transform.GetComponent<BasketSpace>();

                    var offset = space.transform.parent.rotation * new Vector3(0,
                        currentObject.collider.bounds.size.y/2 -
                        space.collider.bounds.size.y, 0);
                    overlapPos = space.transform.position + offset;
                    List<Collider> hitColliders = Physics.OverlapBox(overlapPos,
                        (currentObject.collider.bounds.size * .9f) / 2, space.transform.rotation,
                        layer_mask).ToList();
                    Debug.Log(hitColliders.Count);
                    if (CheckIfObjectCanFit(hitColliders))
                    {
                        Vector3 middlePoint = Vector3.zero;
//                        middlePoint -= space.transform.parent.rotation *new Vector3(0, space.collider.bounds.size.y, 0);
                        
                        Debug.Log(middlePoint);
                        foreach (var hitCollider in hitColliders)
                        {
                            middlePoint += hitCollider.GetComponent<BasketSpace>().placePos.position;
                            hitCollider.GetComponent<BasketSpace>().SpaceFilled(currentObject);
                        }

                        middlePoint /= hitColliders.Count;
                        middlePoint.y = space.placePos.position.y;
                        currentObject.transform.position = middlePoint;
                        currentObject.transform.rotation = space.transform.parent.rotation;
                        Debug.Log(middlePoint);

                        EventManager.DisableColliders(hitColliders);
                        currentObject = objects[objectIndex];
                        objectIndex++;
                    }
                    else
                    {
                        Debug.Log("koyamazsın");
                        EventManager.DisableColliders(hitColliders);
                    }
                }
            }
        }
    }
    
    public bool CheckIfObjectCanFit(List<Collider> spaces)
    {
        int spaceVolume = 0;
        foreach (var space in spaces)
        {
            if (!space.GetComponent<BasketSpace>().filled)
            {
                spaceVolume++;
            }
        }

        if (currentObject.volume <= spaceVolume)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}