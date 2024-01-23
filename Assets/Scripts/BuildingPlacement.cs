using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingPlacement : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyBulldozering;

    private BuildingPreset curBuildingPreset;

    private float indicatorUpdateRate = 0.05f;
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;

    public GameObject placementIndicator;
    public GameObject bulldozerIndicator;
    
    //Reference to Player Input System
    private PlayerInputActions playerInputActions;

    private void Awake()
    {        
        playerInputActions = new PlayerInputActions();
        //Link the Cancel_Building Action to the CancelBuildingPlacement function
        playerInputActions.Player.Cancel_Building.performed += e => CancelPlacement();
        //Link the Left Mouse Click to the PlaceBuilding function
        playerInputActions.Player.PlaceDelete_Building.performed += e => PlaceDeleteBuilding();
        //Rotate Building
        playerInputActions.Player.Rotate_Building.performed += e => RotateBuilding();


        //Enable playerInput
        playerInputActions.Enable();
    }


    void RotateBuilding()
    {
        if (currentlyPlacing)
        {
            placementIndicator.transform.Rotate(Vector3.up * 90);
        }
    }

    /// <summary>
    /// Place down the currenly selected building
    /// </summary>
    private void PlaceDeleteBuilding()
    {
        //Avoid to create multiples buildings , check if not hovering in Canvas
        if (!EventSystem.current.IsPointerOverGameObject()) {

            //first check if in the current position is there another building for create or remove 
            Building ExistingBuilding = City.Instance.buildings.Find(x => x.transform.position == curIndicatorPos);

            if (currentlyPlacing && ExistingBuilding == null)
            {            
                GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curIndicatorPos, placementIndicator.transform.rotation);
                City.Instance.OnPlaceBuilding(buildingObj.GetComponent<Building>());
            }
            else if (currentlyBulldozering && ExistingBuilding != null)
            {
                   City.Instance.OnRemoveBuilding(ExistingBuilding);
            }
        }
    }

    /// <summary>
    /// Called when we press a building UI button
    /// </summary>
    /// <param name="preset"></param>
    public void BeginNewBuildingPlacement (BuildingPreset preset)
    {
        //TODO make sure we have enough money
        if (!currentlyBulldozering) { 
            currentlyPlacing = true;
            curBuildingPreset = preset;
            placementIndicator.GetComponentInChildren<MeshFilter>().mesh = preset.prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
            placementIndicator.GetComponentInChildren<Transform>().localScale = preset.prefab.transform.GetChild(0).localScale;
            placementIndicator.SetActive (true);            

            placementIndicator.transform.position = new Vector3(0, -99, 0);
        }
    }    

    void CancelPlacement()
    {
        if (currentlyPlacing) { 
            currentlyPlacing = false;
            placementIndicator.SetActive(false);
        }else if (currentlyBulldozering)
        {
            currentlyBulldozering = false;
            bulldozerIndicator.SetActive(false);
        }
    }

    public void BeginNewBulldozer()
    {
        if (!currentlyPlacing) { 
            currentlyBulldozering = true;
            bulldozerIndicator.SetActive (true);
            bulldozerIndicator.transform.position  = new Vector3(0, -99, 0);
        }
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime > indicatorUpdateRate)
        {
            lastUpdateTime = Time.time;

            curIndicatorPos = Selector.Instance.GetCurTilePosition();
            

            if (currentlyPlacing) 
                placementIndicator.transform.position = curIndicatorPos;
            else if (currentlyBulldozering)
                bulldozerIndicator.transform.position = curIndicatorPos;
        }
        
    }

}
