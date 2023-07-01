using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataLoadException : Exception
{
    public DataLoadException(string msg) : base(msg) { }
}

public class DataIsEmptyException : Exception
{
    public DataIsEmptyException(string msg) : base(msg) { }
}

public class APIBridge : MonoBehaviour
{
    public enum API
    {
        Yandex,
    }

    public API api;
    public float timeInSToWaitServerResponse;
    public static event Action OnAdvertisementOpen;
    public static event Action<bool> OnAdvertisementClose;
    public static APIBridge Instance { get; private set; }


    public void SaveData(Database data)
    {
        switch (Instance.api)
        {
            case API.Yandex:
                YandexAPI._SaveData(JsonConvert.SerializeObject(data, Formatting.Indented));
                break;
        }
    }

    public Database LoadData()
    {
        bool hasEnded = false;
        Database data = null;
        string jsonData = null;

        switch (Instance.api)
        {
            case API.Yandex:
                jsonData = YandexAPI._LoadData(ref hasEnded);
                break;
        }
        while (hasEnded == false) { };

        if (jsonData == null)
            throw new DataLoadException("Error while loading data from server");
        else if (jsonData == "")
            throw new DataIsEmptyException("Loaded data is empty");

        try
        {
            data = JsonConvert.DeserializeObject<Database>(jsonData);
        }
        catch (JsonException) { }

        return data;
    }

    public void ShowRewardedAdv()
    {
        switch (Instance.api)
        {
            case API.Yandex:
                YandexAPI._ShowRewardedAdv();
                break;
        }
    }

    public void RaiseOnAdvertisementOpenEvent()
    {
        OnAdvertisementOpen?.Invoke();
    }

    public void RaiseOnAdvertisementCloseEvent()
    {
        OnAdvertisementClose?.Invoke(false);
    }

    public void RaiseOnRewardGotEvent()
    {
        OnAdvertisementClose?.Invoke(true);
    }

}
