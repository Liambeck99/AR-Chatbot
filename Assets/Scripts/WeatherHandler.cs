using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherHandler : MonoBehaviour
{
	private Coords currentLocation;
	private WeatherResponse currentWeather;
    private string weatherData;

    public string openWeatherAPIKey;

    private bool finishedSearch;

    [Serializable]
    public class WeatherResponse
    {
        public Coords coord;
        public List<WeatherInfo> weather;
        public string @base;
        public MainWeatherData main;
        public int visibility;
        public Wind wind;
        public Clouds cloud;
        public Sys sys;
        public int timezone;
        public int id;
        public string name;
        public string cod;
    }

    [Serializable]
    public class WeatherInfo
    {
        public int id;
        public string main;
        public string description;
        public string icon;
    }

    [Serializable]
    public class MainWeatherData
    {
        public float temp;
        public float feels_like;
        public float temp_min;
        public float temp_max;
        public float pressure;
        public int humidity;
    }

    [Serializable]
    public class Wind
    {
        public float speed;
        public int degrees;
        public float gust;
    }

    [Serializable]
    public class Clouds
    {
        public int all;
    }

    [Serializable]
    public class Sys
    {
        public string type;
        public int id;
        public string country;
        public string sunrise;
        public string sunset;
    }

    public bool GetWeatherInfo()
    {
        currentLocation = new Coords();

        int maxSecondsWait = 5;

        DateTime startTime = DateTime.Now;

        bool locationFailed = false;

        int secondsDifference;

        Input.location.Start();
        
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            secondsDifference = (int)DateTime.Now.Subtract(startTime).TotalSeconds;

            if (secondsDifference > 5)
                locationFailed = false;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxSecondsWait < 1)
            locationFailed = false;

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
            locationFailed = false;

        currentLocation = new Coords();
        currentLocation.lat = Input.location.lastData.latitude;
        currentLocation.lon = Input.location.lastData.longitude;

        Input.location.Stop();

        if(!locationFailed || (currentLocation.lat == 0 && currentLocation.lon == 0))
        {
            currentLocation.lat = 53.80718685164538f;
            currentLocation.lon = -1.554964758234633f;
        }

        string request = "api.openweathermap.org/data/2.5/weather?lat=" + currentLocation.lat + "&lon=" + currentLocation.lon + "&appid=" + openWeatherAPIKey;

        finishedSearch = false;

        StartCoroutine(GetRequest(request));

        return true;
    }

    private IEnumerator GetRequest(string uri)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        finishedSearch = true;

        if (www.isNetworkError || www.isHttpError)
        {

        }
        else
        {
            weatherData = www.downloadHandler.text;
        }

        currentWeather = JsonUtility.FromJson<WeatherResponse>(weatherData);

        finishedSearch = true;

        yield break;
    }

    public float GetWindSpeed()
    {
        return currentWeather.wind.speed;
    }

    public string GetWeatherType()
    {
        return currentWeather.weather[0].main;
    }

    public Coords GetCurrentLocation()
    {
        return currentLocation;
    }

    public bool HasFinishedSearch()
    {
        return finishedSearch;
    }

}

[Serializable]
public class Coords
{
    public float lon;
    public float lat;
}
