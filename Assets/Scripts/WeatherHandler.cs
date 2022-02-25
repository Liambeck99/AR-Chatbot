// Script that is handles normalising and retrieving weather data for a given location

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherHandler : MonoBehaviour
{
    // Key for accessing the OpenWeather API to retrieve user weather data
    public string openWeatherAPIKey;

    // Location which will be used for returning the weather for
    public float latitudeToUse;
    public float longlitudeToUse;

    // The current location in a coordinate format
    private Coords currentLocation;

    // The response from the openweather API
	private WeatherResponse currentWeather;

    // Flag for whether the handler has finished retrieving the correct weather data from the API
    private bool finishedSearch;

    // The following classes depict the JSON format used in the API response, so is so that the 
    // returned JSON string can be correctly mapped to the following objects
    [Serializable]
    public class WeatherResponse
    {
        public Coords coord;
        public List<WeatherInfo> weather;
        public string @base;
        public MainWeatherData main;
        public int visibility;
        public Wind wind;
        public Clouds clouds;
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
        public int sunrise;
        public int sunset;
    }

    [Serializable]
    public class Coords
    {
        public float lon;
        public float lat;
    }

    // Retrieves the current weather data in Leeds from the OpenWeather API
    public void GetWeatherInfo()
    {
        /*bool locationFailed = false;

        int maxSecondsWait = 5;

        DateTime startTime = DateTime.Now;

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
            currentLocation.lat = latitudeToUse;
            currentLocation.lon = longlitudeToUse;
        }
        */

        // Generates a rest get request to query the API for the weather data in Leeds
        string request = "api.openweathermap.org/data/2.5/weather?lat=" + latitudeToUse + "&lon=" + longlitudeToUse + "&appid=" + openWeatherAPIKey;

        finishedSearch = false;

        // Sends the request and returns once a response is found
        StartCoroutine(GetRequest(request));
    }

    // Sends a request to the OpenWeather API and handles the response
    private IEnumerator GetRequest(string uri)
    {
        // Creates a web request object and sends the request
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        // If the data could not be found, then set the current weather object to null and return
        if (www.isNetworkError || www.isHttpError)
        {
            currentWeather = null;

            finishedSearch = true;

            yield break;
        }

        // Otherwise, a valid response was returned, this JSON response is converted into a 
        // weather object which contains all of the correct values
        currentWeather = JsonUtility.FromJson<WeatherResponse>(www.downloadHandler.text);

        // Search has finished
        finishedSearch = true;

        yield break;
    }

    // Returns the sunset epoch time
    public int GetSunsetEpoch()
    {
        return currentWeather.sys.sunset;
    }

    // Returns the sunrise epoch time
    public int GetSunriseEpoch()
    {
        return currentWeather.sys.sunrise;
    }

    // Returns the wind speed
    public float GetWindSpeed()
    {
        return currentWeather.wind.speed;
    }

    // Returns the weather type
    public string GetWeatherType()
    {
        return currentWeather.weather[0].main;
    }

    // Returns the percentage of clouds in the sky at the current location
    public float GetCloudPercentage()
    {
        return (float)currentWeather.clouds.all / 100.0f;
    }

    // Returns if the current weather request to the API has finished or not
    public bool HasFinishedSearch()
    {
        return finishedSearch;
    }

    // If the current weather object has some data, then it is assumed that the weather data
    // was correctly returned, otherwise return that the weather data was not correctly found
    public bool HasCorrectlyRetrievedWeatherData()
    {
        if (currentWeather == null)
            return false;
        return true;
    }

}
