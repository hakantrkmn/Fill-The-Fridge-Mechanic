using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class ShoppingCarController : MonoBehaviour
{
    public List<ObjectController> placedObjects;
    public List<ObjectController> allObjects;
    public GameObject cantFitText;
    public List<GameObject> objectList;
    public List<GameObject> basketList;
    public GameObject ground;

    public GameObject groundObj;
    public float percent;
    bool isSelected;
    ObjectController currentObject;
    int place_layer;
    Vector3 overlapPos;
    private float timer;
    private GameStates gameState;


    private void Start()
    {
        place_layer = LayerMask.GetMask("BasketGround", "Basket", "Object");

        foreach (var obj in GetComponentsInChildren<ObjectController>())
        {
            allObjects.Add(obj);
        }
    }
    public void CalculatePercent()
    {
        percent = (100 * (float)placedObjects.Count) / (placedObjects.Count + allObjects.Count);
    }

    public void ChooseBasket()
    {
        if (!isSelected && allObjects.Count != 0)
        {
            isSelected = true;
            currentObject = allObjects.First();
            EventManager.ShoppingCardSelected(this);
        }
    }

    private void OnEnable()
    {
        EventManager.ChangeGameState += ChangeGameState;
        EventManager.ShoppingCardSelected += ShoppingCardSelected;
    }

    public void ObjectRemoved(ObjectController obj)
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

    void PlaceObject()
    {
        if (Input.GetMouseButton(0) && timer > .3f)
        {
            basketList.Clear();
            objectList.Clear();
            ground = null;
            timer = 0;


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, place_layer))
            {
                var basket = hit.transform.GetComponentInParent<BasketController>();
                if (basket != null)
                {
                    if (basket.isSelected)
                    {
                        var offset = basket.transform.rotation *
                                     new Vector3(0, (currentObject.collider.bounds.size.y) / 2, 0);

                        overlapPos = hit.point + offset;

                        if (hit.transform.GetComponent<BasketGround>())
                        {
                            ground = hit.transform.gameObject;
                        }
                        else if (hit.transform.GetComponentInParent<ObjectController>())
                        {
                            objectList.Add(hit.transform.gameObject);
                        }

                        List<Collider> hitColliders = Physics.OverlapBox(overlapPos,
                            (currentObject.collider.bounds.size*.99f) / 2, basket.transform.rotation, place_layer).ToList();
                        foreach (var coll in hitColliders)
                        {
                            if (coll.GetComponent<BasketController>())
                            {
                                basketList.Add(coll.gameObject);
                            }
                            else if (coll.GetComponentInParent<ObjectController>())
                            {
                                if (!objectList.Contains(coll.gameObject))
                                {
                                    objectList.Add(coll.gameObject);
                                }
                            }
                        }

                        var hitType = CheckForPlace();

                        if (hitType == HitTypes.JustBasket)
                        {
                            var text = Instantiate(cantFitText, hit.point, quaternion.identity);
                            Destroy(text, 1);
                        }
                        else
                        {
                            PlaceObjectWithCondition(hitType, basket, hit);
                        }
                    }
                }
            }
        }
    }

    void PlaceObjectWithCondition(HitTypes condition, BasketController basket, RaycastHit hit)
    {
        if (condition == HitTypes.JustGround)
        {
            currentObject.PlaceObject(basket.transform, hit.point, basket.transform.rotation, 0);
            basket.insideObjects.Add(currentObject);
            UpdateCurrentObject();
            EventManager.ObjectPlaced(basket);
        }
        else if (condition == HitTypes.JustObject)
        {
            if (hit.transform.GetComponentInParent<ObjectController>())
            {
                if (objectList.Count < 2)
                {
                    var obj = hit.transform.GetComponentInParent<ObjectController>();
                    if (currentObject.height + obj.currentHeight <= basket.maxHeight)
                    {
                        if (currentObject.size == obj.size)
                        {
                            currentObject.PlaceObject(basket.transform, obj.placePosition.position,
                                basket.transform.rotation,
                                obj.currentHeight);
                        }
                        else
                        {
                            currentObject.PlaceObject(basket.transform, hit.point,
                                basket.transform.rotation,
                                obj.currentHeight);
                        }

                        basket.insideObjects.Add(currentObject);
                        UpdateCurrentObject();
                        EventManager.ObjectPlaced(basket);
                    }
                    else
                    {
                        SpawnCantHitText(hit.point);
                    }
                }
                else
                {
                    SpawnCantHitText(hit.point);

                }
            }
            else
            {
                SpawnCantHitText(hit.point);
            }
        }
        else if (condition == HitTypes.GroundAndObject)
        {
            SpawnCantHitText(hit.point);
        }
    }

    private void SpawnCantHitText(Vector3 position)
    {
        var text = Instantiate(cantFitText, position, quaternion.identity);
        Destroy(text, 1);
    }

    HitTypes CheckForPlace()
    {
        if (basketList.Count != 0)
        {
            return HitTypes.JustBasket;
        }
        else if (basketList.Count == 0 && objectList.Count == 0 && ground != null)
        {
            return HitTypes.JustGround;
        }
        else if (basketList.Count == 0 && objectList.Count != 0 && ground == null)
        {
            return HitTypes.JustObject;
        }
        else if (ground != null && objectList.Count != 0)
        {
            return HitTypes.GroundAndObject;
        }
        else
        {
            Debug.Log(ground);
            return HitTypes.None;
        }
    }

    void UpdateCurrentObject()
    {
        allObjects.Remove(currentObject);
        placedObjects.Add(currentObject);
        if (allObjects.Count != 0)
        {
            currentObject = allObjects.First();
        }
    }
}