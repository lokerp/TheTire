using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDataControllable
{
    void SaveData(ref Database database);
    void LoadData(Database database);
    void AfterDataLoaded(Database database);
}
