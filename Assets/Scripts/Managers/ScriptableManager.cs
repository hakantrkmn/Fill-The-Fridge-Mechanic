using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ScriptableManager : MonoBehaviour
{
    [SerializeField] GameData gameData;
    [SerializeField] PlayerMovementSettings PlayerMovementSettings;
    public ObjectData objData;

    //-------------------------------------------------------------------
    void Awake()
    {
        SaveManager.LoadGameData(gameData);

        Scriptable.GameData = GetGameData;
        Scriptable.PlayerSettings = GetPlayerMovementSettings;
        Scriptable.GetObjectData = GetObjData;

    }


    //-------------------------------------------------------------------
    GameData GetGameData() => gameData;
    ObjectData GetObjData() => objData;

    //-------------------------------------------------------------------
    PlayerMovementSettings GetPlayerMovementSettings() => PlayerMovementSettings;

}



public static class Scriptable
{
    public static Func<GameData> GameData;
    public static Func<ObjectData> GetObjectData;
    public static Func<PlayerMovementSettings> PlayerSettings;
}