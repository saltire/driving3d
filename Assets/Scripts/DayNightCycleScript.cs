using UnityEngine;
using System.Collections;
 
public class DayNightCycleScript : MonoBehaviour {
 
    private int dayLength;   //in minutes
    private int dayStart;
    private int nightStart;   //also in minutes
    private int currentTime;
    public float cycleSpeed;
    private bool isDay;
    private Vector3 sunPosition;
    Light sunlight;
    public float sunriseDuration = 5f;
    float currentLerpTime = 0;

    float targetSunriseValue = 1;
    float targetSunsetValue = 0;

    bool sunriseStarted = true;
    bool sunsetStarted = true;
 
    void Start() {
        dayLength = 1440;
        dayStart = 300;
        nightStart = 1200;
        currentTime = 1200;
        StartCoroutine (TimeOfDay());

        sunlight = GetComponent<Light>();
    }
 
    void Update() {
 
        if (currentTime > 0 && currentTime < dayStart) 
        {
            isDay =false;
        } 
        else if (currentTime >= dayStart && currentTime < nightStart && sunriseStarted == true) 
        {
            sunsetStarted = true;
            isDay = true;
            StartCoroutine(LerpSunriseFunction(targetSunriseValue, 5));
            sunriseStarted = false;
        } 
        else if (currentTime >= nightStart && currentTime < dayLength && sunsetStarted == true) 
        {
            sunriseStarted = true;
            isDay = false;
            StartCoroutine(LerpSunsetFunction(targetSunsetValue, 5));
            sunsetStarted = false;
        } 
        else if (currentTime >= dayLength) 
        {
            currentTime = 0;
        }
        float currentTimeF = currentTime;
        float dayLengthF = dayLength;  
    }
 
    IEnumerator TimeOfDay(){
        while (true) {
            currentTime += 1;
            int hours = Mathf.RoundToInt( currentTime / 60);
            int minutes = currentTime % 60;
            //Debug.Log (hours+":"+minutes);
            yield return new WaitForSeconds(1f/cycleSpeed);
        }
    }

    IEnumerator LerpSunriseFunction(float endValue, float duration)
    {
        float timer = 0;
        float startValue = 0;

        while (timer < duration)
        {
            sunlight.intensity = Mathf.Lerp(startValue, endValue, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        sunlight.intensity = endValue;
        Debug.Log(timer); 
    }    

    IEnumerator LerpSunsetFunction(float endValue, float duration)
    {
        float timer = 0;
        float startValue = 1;

        while (timer < duration)
        {
            sunlight.intensity = Mathf.Lerp(startValue, endValue, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        sunlight.intensity = endValue;
        Debug.Log(timer); 
    }       
} 