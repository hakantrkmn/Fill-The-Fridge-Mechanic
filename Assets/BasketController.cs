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
    public Transform startPos;
    public GameObject spacePrefab;
    public List<List<BasketSpace>> spaceColumns;
    public bool isSelected;
    private Vector3 firstPosition;
    private Quaternion firstRotation;
    public float percent;
    private void OnEnable()
    {
        EventManager.DoneButtonClicked += DoneButtonClicked;
        EventManager.BasketSelected += BasketSelected;
        EventManager.DisableColliders += DisableColliders;
    }

    public void CalculatePercent()
    {
        int filledSpaceAmount=0;
        foreach (var column in spaceColumns)
        {
            foreach (var space in column)
            {
                if (space.filled)
                {
                    filledSpaceAmount++;
                }
            }
        }

        float maxAmount = size.x * size.y * size.z;
        percent = (100*filledSpaceAmount)/maxAmount;
    }
  

    private void OnDisable()
    {
        EventManager.DoneButtonClicked -= DoneButtonClicked;
        EventManager.BasketSelected -= BasketSelected;
        EventManager.DisableColliders -= DisableColliders;
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

    
    private void DisableColliders(List<Collider> obj)
    {
        SetBaseSpaces();
    }

    private void Start()
    {
        firstRotation = transform.rotation;
        firstPosition = transform.position;
        for (int i = 0; i < (int)size.x * (int)size.z; i++)
        {
            spaceColumns.Add(new List<BasketSpace>());
        }
        CreateSpaces();
        SetBaseSpaces();
    }

    public void CreateSpaces()
    {
        int listIndex=0;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    var pos = startPos.position + new Vector3(i * 1f, k * 1f, j*1f);
                    var space = Instantiate(spacePrefab, pos, quaternion.identity,startPos);
                    spaceColumns[listIndex].Add(space.GetComponent<BasketSpace>());
                }

                listIndex++;
            }
        }
    }

    public void SetBaseSpaces()
    {
        bool baseChoosed = false;
        for (int i = 0; i < spaceColumns.Count; i++)
        {
            for (int j = 0; j < spaceColumns[i].Count; j++)
            {
                if (spaceColumns[i][j].filled)
                {
                    spaceColumns[i][j].CloseSpace();
                }
                else if(!spaceColumns[i][j].filled && !baseChoosed)
                {
                    spaceColumns[i][j].OpenSpace();
                    baseChoosed = true;
                }
                else if(!spaceColumns[i][j].filled && baseChoosed)
                {
                    spaceColumns[i][j].CloseSpace();
                }
            }

            baseChoosed = false;
        }
    }
    

}
