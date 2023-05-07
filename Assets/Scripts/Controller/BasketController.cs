using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class BasketController : SerializedMonoBehaviour
{
    public Vector3 size;
    [HideInInspector]
    public bool isSelected;
    public float percent;
    [HideInInspector]
    public float maxHeight;
    public float volume;
    public List<ObjectController> insideObjects;
    
    private Vector3 firstPosition;
    private Quaternion firstRotation;
    private void OnEnable()
    {
        EventManager.ObjectRemoved += ObjectRemoved;
        EventManager.DoneButtonClicked += DoneButtonClicked;
        EventManager.BasketSelected += BasketSelected;
    }

    private void ObjectRemoved(ObjectController obj)
    {
        if (insideObjects.Contains(obj))
        {
            insideObjects.Remove(obj);
        }
    }

    public void CalculatePercent()
    {
        float filledSpaceAmount=0;
        foreach (var obj in insideObjects)
        {
            filledSpaceAmount += obj.volume;
        }

        
        percent = (100*filledSpaceAmount)/volume;
    }
  

    private void OnDisable()
    {
        EventManager.ObjectRemoved -= ObjectRemoved;
        EventManager.DoneButtonClicked -= DoneButtonClicked;
        EventManager.BasketSelected -= BasketSelected;
        
    }


    private void DoneButtonClicked()
    {
        isSelected = false;
        transform.DOMove(firstPosition, .2f);
        transform.DORotateQuaternion(firstRotation, .2f);
    }

    private void BasketSelected(BasketController obj)
    {
        if (obj!=this)
        {
            isSelected = false;
            transform.DOMove(firstPosition, .2f);
            transform.DORotateQuaternion(firstRotation, .2f);
        }
    }

    private void OnMouseDown()
    {
        if (!isSelected)
        {
            EventManager.ChangeGameState(GameStates.PlaceObject);
            isSelected = true;
            var pos = EventManager.GetSelectedTranform();
            transform.DOMove(pos.position, .2f);
            transform.DORotateQuaternion(pos.rotation, .2f);
            EventManager.BasketSelected(this);
        }
        
    }


    private void Start()
    {
        volume = size.x * size.y * size.z;
        maxHeight = size.y;
        firstRotation = transform.rotation;
        firstPosition = transform.position;
      
    }
    

}
