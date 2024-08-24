using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class utilitiesManager : MonoBehaviour
{
    public static utilitiesManager Instance;
    public GameObject buildBackground;
    public GameObject buildMode;

    public Transform cameraTransform;

    public TextMeshProUGUI LivePlayerMoney;
    public TextMeshProUGUI goldCount;
    public TextMeshProUGUI woodCount;
    public TextMeshProUGUI foodCount;
    public GameObject StorageMenu;

    public building buildScript;
    public SpriteRenderer buildRenderer;
    public GameObject connectingBuilding;
    public GameObject currentMarketStorage;

    public float playerMoney = 30f;
    public bool connectionMode = false;

    void Start() {
        LivePlayerMoney.text = playerMoney.ToString();
    }


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void transparentBuildings(bool on) {

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building")
            .Concat(GameObject.FindGameObjectsWithTag("market"))
            .Concat(GameObject.FindGameObjectsWithTag("production"))
            .ToArray();

        Color newColor = on ? new Color(1f, 1f, 1f, .5f) : new Color(1f, 1f, 1f, 1f);
        
        foreach (GameObject building in buildings) {

            SpriteRenderer buildingSpriteRenderer = building.GetComponent<SpriteRenderer>();
            buildingSpriteRenderer.color = newColor;

            GameObject buildingBase = building.transform.GetChild(0).gameObject;
            buildingBase.SetActive(on);
        }

    }

    public void setBuildMode(bool on, Sprite sprite = null, bool halfPoints = false) {

        if (on) {
            buildScript.halfPoints = halfPoints; 
            buildScript.activationTime = Time.time;
            buildRenderer.sprite = sprite;

        } else {
            buildRenderer.sprite = null;
        }

        transparentBuildings(on);
        buildMode.SetActive(on);

    }

    public void SetMarketStorageOn(GameObject market) {

        currentMarketStorage = market;
        Market currentStorage = market.GetComponent<Market>();
        goldCount.text = currentStorage.goldAmount.ToString();
        woodCount.text = currentStorage.woodAmount.ToString();
        foodCount.text = currentStorage.pumpkinAmount.ToString();

        StorageMenu.SetActive(true);

    }

    public void SetMarketStorgeText(GameObject market){

        if (currentMarketStorage != null && currentMarketStorage == market) {
            Market currentStorage = market.GetComponent<Market>();
            goldCount.text = currentStorage.goldAmount.ToString();
            woodCount.text = currentStorage.woodAmount.ToString();
            foodCount.text = currentStorage.pumpkinAmount.ToString();
        }

    }

    public void SetMarketStorageOff() {
        currentMarketStorage = null;
        StorageMenu.SetActive(false);
    }

    public bool IsMarketStorageActive() {
        return StorageMenu.activeInHierarchy ;
    }

    public void NegatePlayerMoney(float sub){
        playerMoney -= sub;
        LivePlayerMoney.text = playerMoney.ToString();

        if (playerMoney < 0f) {
            SceneManager.LoadScene("Lose");
        }

    }

    public bool GetConnectMode() {
        return connectionMode;
    }

    public void SetConnectingBuilding(GameObject newConnectingBuilding) {
        connectingBuilding = newConnectingBuilding;  
    }

    public void ToggleConnectionMode(bool activateMode)
    {
        GameObject[] allUserBuilds = GameObject.FindGameObjectsWithTag("production")
            .Concat(GameObject.FindGameObjectsWithTag("market"))
            .ToArray();

        foreach (GameObject build in allUserBuilds)
        {
            if (build.tag == "production") {
                SpriteRenderer buildRenderer = build.GetComponent<SpriteRenderer>();
                buildRenderer.sortingLayerName = activateMode ? "UI Base" : "User Builds";
                buildRenderer.sortingOrder = activateMode ? 0 : 
                    Mathf.RoundToInt(-(build.transform.position.y - buildRenderer.bounds.size.y / 2) * 100f);
            }

            GameObject secondChild = build.transform.GetChild(1).gameObject;
            secondChild.SetActive(activateMode);
        }

        connectionMode = activateMode;

    }

    public void WinGameCheck(string value){
        if (value == "food"){
            playerMoney += 2f;
        } else if (value == "wood"){
            playerMoney += 5f;
        } else if (value == "gold") {
            playerMoney += 10f;
        }

        LivePlayerMoney.text = Math.Truncate(playerMoney).ToString();

        if (playerMoney >= 200f){

            float ptime = Time.timeSinceLevelLoad;
            PlayerPrefs.SetFloat("ptime", ptime);
            SceneManager.LoadScene("Win");

        }
    }

}
