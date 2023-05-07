using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public float volume;
    public BoxCollider collider;
    private Transform shoppingCardParent;
    private Vector3 shoppingCardPosition;
    private Quaternion shoppingCardRotation;
    public bool placed;

    private void OnValidate()
    {
        volume = collider.bounds.size.x * collider.bounds.size.y * collider.bounds.size.z;
    }


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

    public void PlaceObject(Vector3 point, Quaternion rotation, Transform hitParent)
    {
        transform.DOJump(point, 2, 1, .5f);
        placed = true;
        transform.DORotateQuaternion(rotation, .5f);
        transform.parent = hitParent;
    }

    private void Start()
    {
        shoppingCardParent = transform.parent;
        shoppingCardPosition = transform.localPosition;
        shoppingCardRotation = transform.rotation;
    }
}