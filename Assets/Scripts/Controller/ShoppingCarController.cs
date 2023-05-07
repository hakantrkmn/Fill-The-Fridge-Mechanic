using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class ShoppingCarController : MonoBehaviour
{
    private GameStates gameState;
    public List<ObjectController> placedObjects;
    public List<ObjectController> allObjects;

    public bool isSelected;
    public ObjectController currentObject;
    private int layer_mask;

    private float timer;
    public GameObject cantFitText;

    private void Start()
    {
        layer_mask = LayerMask.GetMask("BasketSpace");
        foreach (var obj in GetComponentsInChildren<ObjectController>())
        {
            allObjects.Add(obj);
        }
    }


    public void ChooseBasket()
    {
        if (!isSelected)
        {
            isSelected = true;
            currentObject = allObjects.First();
            EventManager.ShoppingCardSelected(this);
        }
    }

    private void OnEnable()
    {
        EventManager.ObjectRemoved += ObjectRemoved;
        EventManager.ChangeGameState += ChangeGameState;
        EventManager.ShoppingCardSelected += ShoppingCardSelected;
    }

    private void ObjectRemoved(ObjectController obj)
    {
        if (placedObjects.Contains(obj))
        {
            placedObjects.Remove(obj);
            allObjects.Add(obj);
            currentObject = allObjects.First();
        }
    }

    private void ChangeGameState(GameStates obj)
    {
        gameState = obj;
    }

    private void OnDisable()
    {
        EventManager.ObjectRemoved -= ObjectRemoved;
        EventManager.ChangeGameState -= ChangeGameState;
        EventManager.ShoppingCardSelected -= ShoppingCardSelected;
    }

    private void ShoppingCardSelected(ShoppingCarController obj)
    {
        if (this != obj)
        {
            isSelected = false;
        }
    }


    private void Update()
    {
        if (isSelected && gameState == GameStates.PlaceObject)
        {
            timer += Time.deltaTime;

            if (allObjects.Count != 0)
            {
                PlaceObject();
            }
            else
            {
                currentObject = null;
            }
        }
    }

    public void PlaceObject()
    {
        if (Input.GetMouseButton(0) && timer > .3f)
        {
            EventManager.PlayerCanControl(false);

            timer = 0;

            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                var space = hit.transform.GetComponent<BasketSpace>();
                if (space.basketController.isSelected)
                {
                    EventManager.EnableColliders();


                    List<Collider> hitColliders = Physics.OverlapBox(space.transform.position,
                        (currentObject.collider.bounds.size ) / 2, space.transform.rotation,
                        layer_mask).ToList();
                    if (CheckIfObjectCanFit(hitColliders))
                    {
                        Vector3 middlePoint = Vector3.zero;


                        foreach (var hitCollider in hitColliders)
                        {
                            middlePoint += hitCollider.GetComponent<BasketSpace>().placePos.position;
                            hitCollider.GetComponent<BasketSpace>().SpaceFilled(currentObject);
                        }


                        middlePoint /= hitColliders.Count;
                        
                        currentObject.PlaceObject(middlePoint,space.transform.rotation,space.transform);
                        
                        EventManager.DisableColliders(hitColliders);
                        
                        UpdateCurrentObject();
                        EventManager.ObjectPlaced(space.GetComponent<BasketSpace>().basketController, currentObject);
                    }
                    else
                    {
                        var text = Instantiate(cantFitText, hit.point, quaternion.identity);
                        Destroy(text, 1);
                        EventManager.DisableColliders(hitColliders);
                    }
                }
            }
        }
    }


    public void UpdateCurrentObject()
    {
        allObjects.Remove(currentObject);
        placedObjects.Add(currentObject);
        if (allObjects.Count!=0)
        {
            currentObject = allObjects.First();
        }
    }

    public bool CheckIfObjectCanFit(List<Collider> spaces)
    {
        List<Collider> tempList = new List<Collider>();
        float spaceVolume = 0;
        foreach (var space in spaces)
        {
            if (!space.GetComponent<BasketSpace>().filled)
            {
                spaceVolume += 1;
            }
            else
            {
                tempList.Add(space);
            }
        }

        foreach (var temp in tempList)
        {
            spaces.Remove(temp);
        }

        if (currentObject.volume <= (spaceVolume))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}