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

    public int objectIndex;
    public bool isSelected;
    public ObjectController currentObject;
    private int layer_mask;
    public Vector3 overlapPos;

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

            if (objectIndex < allObjects.Count)
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
            timer = 0;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                var space = hit.transform.GetComponent<BasketSpace>();
                if (space.basketController.isSelected)
                {
                    EventManager.EnableColliders();


                    var offset = space.transform.parent.rotation * new Vector3(0,
                        (currentObject.collider.bounds.size.y)/2 -
                        space.collider.bounds.size.y, 0);
                    overlapPos = space.transform.position + offset;
                    List<Collider> hitColliders = Physics.OverlapBox(overlapPos,
                        (currentObject.collider.bounds.size*.9f ) / 2, space.transform.rotation,
                        layer_mask).ToList();
                    Debug.Log(hitColliders.Count);
                    if (CheckIfObjectCanFit(hitColliders))
                    {
                        Vector3 middlePoint = Vector3.zero;


                        foreach (var hitCollider in hitColliders)
                        {
                            middlePoint += hitCollider.GetComponent<BasketSpace>().placePos.position;
                            hitCollider.GetComponent<BasketSpace>().SpaceFilled(currentObject);
                        }


                        middlePoint /= hitColliders.Count;
                        //middlePoint.y = space.placePos.position.y;
                        //currentObject.transform.position = middlePoint;
                        currentObject.transform.DOJump(middlePoint, 2, 1, .5f);
                        currentObject.GetComponent<ObjectController>().placed = true;
                        //currentObject.transform.rotation = space.transform.parent.rotation;
                        currentObject.transform.DORotateQuaternion(space.transform.rotation, .5f);
                        currentObject.transform.parent = space.transform;
                        EventManager.DisableColliders(hitColliders);
                        allObjects.Remove(currentObject);
                        placedObjects.Add(currentObject);
                        UpdateCurrentObject();
                        EventManager.ObjectPlaced(space.GetComponent<BasketSpace>().basketController);
                    }
                    else
                    {
                        var text = Instantiate(cantFitText, hit.point, quaternion.identity);
                        Destroy(text,1);
                        EventManager.DisableColliders(hitColliders);
                    }
                }
            }
        }
    }


    public void UpdateCurrentObject()
    {
        if (objectIndex < allObjects.Count)
        {
            currentObject = allObjects.First();
        }
    }

    public bool CheckIfObjectCanFit(List<Collider> spaces)
    {
        float spaceVolume = 0;
        foreach (var space in spaces)
        {
            if (!space.GetComponent<BasketSpace>().filled)
            {
                spaceVolume += 1;
            }
            else
            {
                return false;
            }
            
        }
        Debug.Log(spaceVolume);
        Debug.Log(currentObject.volume);

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