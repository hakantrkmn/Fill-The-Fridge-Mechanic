using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class BasketController : SerializedMonoBehaviour
{
    public Vector3 size;
    public Transform startPos;
    public GameObject spacePrefab;
    public List<List<BasketSpace>> spaceColumns;


    private void OnEnable()
    {
        EventManager.DisableColliders += DisableColliders;
    }


    private void OnDisable()
    {
        EventManager.DisableColliders -= DisableColliders;
    }

    private void DisableColliders(List<Collider> obj)
    {
        SetBaseSpaces();
    }

    private void Start()
    {
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
                    var pos = startPos.position + new Vector3(i * .1f, k * .1f, j*.1f);
                    var space = Instantiate(spacePrefab, pos, quaternion.identity,transform);
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
