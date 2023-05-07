using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Vector3 size;
    public float volume;
    public BoxCollider collider;
    [HideInInspector]
    public bool placed;
    [HideInInspector]
    public float currentHeight;
    [HideInInspector]
    public float height;
    public Transform placePosition;
    
    private Transform shoppingCardParent;
    private Vector3 shoppingCardPosition;
    private Quaternion shoppingCardRotation;
    public void RemoveObject()
    {
        if (placed)
        {
            placed = false;
            transform.parent = shoppingCardParent;
            transform.DOLocalJump(shoppingCardPosition, 2, 1, .5f);
            transform.DORotateQuaternion(shoppingCardRotation, .5f);
            EventManager.ObjectRemoved(this);
        }
    }

    public void PlaceObject(Transform hitParent, Vector3 position, Quaternion rotation, float height)
    {
        transform.parent = hitParent;
        transform.position = position;
        transform.rotation = rotation;
        currentHeight = this.height + height;
        placed = true;
    }


    private void Start()
    {
        volume = collider.bounds.size.x * collider.bounds.size.y * collider.bounds.size.z;
        size = new Vector3(collider.bounds.size.x, collider.bounds.size.y, collider.bounds.size.z);
        height = collider.bounds.size.y;
        shoppingCardParent = transform.parent;
        shoppingCardPosition = transform.localPosition;
        shoppingCardRotation = transform.rotation;
    }
}