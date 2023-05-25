using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const string DefaultName = "Manager";

    private static GameManager instance;
    private static DataManager dataManager;

    public static GameManager Instance { get { return instance; } }
    public static DataManager Data { get { return dataManager; } }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        InitManagers();
    }

    private void InitManagers()
    {
        GameObject dataObj = new GameObject() { name = "DataManager" };
        dataObj.transform.SetParent(transform);
        dataManager = dataObj.AddComponent<DataManager>();
    }


}
