using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameStates gameState;
    public Vector3 size;
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

    private void OnEnable()
    {
        EventManager.ChangeGameState += ChangeGameState;
    }

    private void OnDisable()
    {
        EventManager.ChangeGameState -= ChangeGameState;
    }

    private void ChangeGameState(GameStates obj)
    {
        gameState = obj;
    }

    public void RemoveObject()
    {
        if (placed)
        {
            placed = false;
            transform.parent = shoppingCardParent;
            transform.DOLocalJump(shoppingCardPosition,2,1,.5f);
            transform.DORotateQuaternion(shoppingCardRotation,.5f);
            EventManager.ObjectRemoved(this);
        }
    }


    private void Start()
    {
        shoppingCardParent = transform.parent;
        shoppingCardPosition = transform.localPosition;
        shoppingCardRotation = transform.rotation;
    }
}