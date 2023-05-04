using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Vector3 size;
    public int volume;
    public Collider collider;
    
    private void OnValidate()
    {
        
        volume = (int)size.x * (int)size.y * (int)size.z;
    }
}
