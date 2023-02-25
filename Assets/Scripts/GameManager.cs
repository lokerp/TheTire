using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataControllable
{
    public Transform tireHolder;
    public TextMeshProUGUI passedDistanceText;

    private GameObject _currentTire;
    private float _passedDistance = 0;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(Database database)
    {
        _currentTire = ItemsManager.PathToPrefab<GameObject>(ItemsManager.Instance.GetItemByType(database.selectedTire).path);
        SpawnTire();
    }

    public void SaveData(ref Database database)
    {

    }

    void SpawnTire()
    {
        Instantiate(_currentTire, tireHolder, false);
        _currentTire.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
                                                           | RigidbodyConstraints.FreezePositionZ
                                                           | RigidbodyConstraints.FreezeRotation;
        _currentTire.AddComponent<PlayerController>();
    }
}
