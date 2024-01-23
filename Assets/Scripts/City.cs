using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    [Header("Time")]
    public float curDayTime;
    private float dayTime = 24;
    private float minutes;
    public float speedFactor = 1;
    private float speedFactor_temp;

    public TextMeshProUGUI timeText;

    public GameObject sun;


    public int money;
    public int day;
    public int curPopulation;
    public int curJobs;
    public int curFood;
    public int maxPopulation;
    public int maxJobs;
    public int incomePerJob;

    public TextMeshProUGUI statsText;

    public List<Building> buildings = new List<Building>();

    public GameObject buttonSelected;
    public Color selectedColor;

    public static City Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buttonSelected.GetComponent<Image>().color = selectedColor;        
        UpdateStatsText();
    }

    private void FixedUpdate()
    {
        DayCycle();
    }

    private void DayCycle()
    {
        curDayTime += Time.deltaTime * speedFactor;
        //NextDay
        if (curDayTime >= dayTime) 
        {
            curDayTime = 0;
            NextDay();
        }
        
        int hour = (int)curDayTime;
        minutes += speedFactor * (Time.deltaTime * 6) * 10;
        int minutesInt = (int)minutes;

        if (minutesInt >= 60) 
            minutes = 0;

        string hourString = hour.ToString("00");
        string minutesString = minutesInt.ToString("00"); 
        timeText.text = hourString + ":" + minutesString;

        //Rotate Sun
        //Rotate the light with the current day time / day time multiplied by 360 degrees
        sun.transform.rotation = Quaternion.Euler(((curDayTime - 6) / dayTime) * 360, 0f, 0f);

        //Move the skybox with the current time
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 2);

    }

    public void NextDay()
    {
        day++;

        CalculateMoney();
        CalculatePopulation();
        CalculateJobs();
        CalculateFood();

        UpdateStatsText();
    }

    private void CalculateFood()
    {
        curFood = 0;

        foreach (Building building in buildings)
            curFood += building.preset.food;
    }

    private void CalculateJobs()
    {
        curJobs = Mathf.Min(curPopulation, maxJobs);
    }

    private void CalculatePopulation()
    {
        if (curFood >= curPopulation && curPopulation < maxPopulation)
        {
            curFood -= curPopulation / 4;
            curPopulation = Mathf.Min(curPopulation + (curFood / 4), maxPopulation);
        }else if (curFood < curPopulation)
        {
            curPopulation = curFood;
        }
    }

    private void CalculateMoney()
    {
        money += curJobs * incomePerJob;

        foreach (Building building in buildings)
            money -= building.preset.costPerTurn;
    }


    /// <summary>
    /// Called when we place down a building
    /// </summary>
    /// <param name="building"></param>
    public void OnPlaceBuilding(Building building)
    {
        money -= building.preset.cost;
        maxPopulation += building.preset.population;
        maxJobs += building.preset.jobs;

        buildings.Add(building);

        UpdateStatsText();
    }

    /// <summary>
    /// Called when we bulldoze a building
    /// </summary>
    /// <param name="building"></param>
    public void OnRemoveBuilding(Building building)
    {
        maxPopulation -= building.preset.population;
        maxJobs -= building.preset.jobs;
        
        //Remove from List
        buildings.Remove(building);
        
        //Remove from Scene
        Destroy(building.gameObject);

        UpdateStatsText();
    }

    private void UpdateStatsText()
    {
        statsText.text = string.Format("DAY:{0} MONEY:{1}€ POPULATION:{2}/{3} JOBS:{4}/{5} FOOD:{6}", new object[7] {day,money,curPopulation,maxPopulation,curJobs,maxJobs,curFood});
    }

    public void SpeedDayCycle(int factor)
    {
        speedFactor = factor;
    }

    public void ChangeColor(GameObject button)
    {
        buttonSelected.GetComponent<Image>().color = Color.white;
        buttonSelected = button;
        buttonSelected.GetComponent<Image>().color = selectedColor;
    }


}
