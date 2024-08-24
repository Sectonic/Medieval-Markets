using UnityEngine;
using System.Collections.Generic;

public class building : MonoBehaviour
{

    public LayerMask outerBoundsLayer;

    public GameObject bigMarket;
    public GameObject mediumMarket;
    public GameObject smallMarket;
    public GameObject pumpkinFarm;
    public GameObject woodFarm;
    public GameObject goldFarm;
    public Dictionary<string, GameObject> buildingPrefabs;

    public GameObject topLeftPoint;
    public GameObject topRightPoint;
    public GameObject bottomLeftPoint;
    public GameObject bottomRightPoint;
    public basicBuildFeatures basicBuildFeaturesScript;
    public Sprite squareSprite;
    public bool halfPoints = false;

    private GameObject topLeftPointInstance;
    private GameObject topRightPointInstance;
    private GameObject bottomLeftPointInstance;
    private GameObject bottomRightPointInstance;
    private GameObject ring;
    private SpriteRenderer spriteRenderer;
    private Sprite previousSprite;
    public float activationTime;

    void Start()
    {

        buildingPrefabs = new Dictionary<string, GameObject>()
        {
            {"Castle_Construction_0", bigMarket},
            {"House_Construction_0", mediumMarket},
            {"Tower_Construction_0", smallMarket},
            {"pumpkin", pumpkinFarm},
            {"tree", woodFarm},
            {"gold", goldFarm},
        };

        spriteRenderer = GetComponent<SpriteRenderer>();
        ring = transform.GetChild(0).gameObject;
        previousSprite = spriteRenderer.sprite;
        activationTime = Time.time;

        setCornerPrefabs(true);

    }

    void setCornerPrefabs(bool starting) {
        Bounds bounds = spriteRenderer.bounds;

        Vector3 center = bounds.center;
        Vector3 bottomLeftOffset = new Vector3(bounds.min.x - center.x, (bounds.min.y - center.y)/1.05f, 0f) + new Vector3(0.2f,0.2f,0f);
        Vector3 bottomRightOffset = new Vector3(bounds.max.x - center.x, (bounds.min.y - center.y)/1.05f, 0f) + new Vector3(-0.2f,0.2f,0f);
        Vector3 topLeftOffset = new Vector3(bounds.min.x - center.x, (bounds.max.y - center.y)/(halfPoints ? 4 : 1), 0f) + new Vector3(0.2f,-0.2f,0f);
        Vector3 topRightOffset = new Vector3(bounds.max.x - center.x, (bounds.max.y - center.y)/(halfPoints ? 4 : 1), 0f) + new Vector3(-0.2f,-0.2f,0f);

        if (starting) {
            topLeftPointInstance = Instantiate(topLeftPoint, transform);
            topRightPointInstance = Instantiate(topRightPoint, transform);
            bottomLeftPointInstance = Instantiate(bottomLeftPoint, transform);
            bottomRightPointInstance = Instantiate(bottomRightPoint, transform);
        }

        SetPointLocalPosition(topLeftPointInstance, topLeftOffset);
        SetPointLocalPosition(topRightPointInstance, topRightOffset);
        SetPointLocalPosition(bottomLeftPointInstance, bottomLeftOffset);
        SetPointLocalPosition(bottomRightPointInstance, bottomRightOffset);
    }

    void SetPointLocalPosition(GameObject point, Vector3 position)
    {
        point.transform.localPosition = position;
    }

    GameObject getActiveSelection() {
        GameObject marketSelection = GameObject.Find("Market Selection");

        if (marketSelection != null) {
            return marketSelection;
        }

        return GameObject.Find("Production Selection");
    }

    void Update()
    {

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;

        transform.position = targetPosition;

        if (spriteRenderer.sprite != previousSprite)
        {
            setCornerPrefabs(false);
            previousSprite = spriteRenderer.sprite;
        }

        if (Input.GetMouseButtonDown(1) && spriteRenderer.sprite != null) {

            utilitiesManager.Instance.transparentBuildings(false);

            spriteRenderer.sprite = null;

            GameObject activeSelection = getActiveSelection();
            Transform itemsTransform = activeSelection.transform.Find("Items");
            Transform[] itemChildren = itemsTransform.GetComponentsInChildren<Transform>();

            if (itemChildren != null && itemChildren.Length > 0) {
                foreach (Transform child in itemChildren) {
                    buildingSelection btnScript = child.gameObject.GetComponent<buildingSelection>();
                    if (btnScript != null) {
                        btnScript.TurnOffOtherBtn();
                    }
                }
            }

            GameObject.Find("Build Mode").SetActive(false);
            
        }

        if (spriteRenderer.sprite != null) {

            float diameterSize = getBuildingDiameter();
            ring.transform.position = transform.position;
            ring.transform.localScale = new Vector2(diameterSize, diameterSize);

            bool clear = checkBuildingPlacement();
            if (Input.GetMouseButtonDown(0) && clear) {
                createNewBuilding(transform.position);
            }
        }

    }

    bool checkBuildingPlacement() {

        RaycastHit2D topHit = Physics2D.Linecast(topLeftPointInstance.transform.position, topRightPointInstance.transform.position, outerBoundsLayer);
        RaycastHit2D rightHit = Physics2D.Linecast(topRightPointInstance.transform.position, bottomRightPointInstance.transform.position, outerBoundsLayer);
        RaycastHit2D bottomHit = Physics2D.Linecast(bottomRightPointInstance.transform.position, bottomLeftPointInstance.transform.position, outerBoundsLayer);
        RaycastHit2D leftHit = Physics2D.Linecast(bottomLeftPointInstance.transform.position, topLeftPointInstance.transform.position, outerBoundsLayer);

        SpriteRenderer ringSpriteRenderer = ring.GetComponent<SpriteRenderer>();

        if (topHit.collider || rightHit.collider || bottomHit.collider || leftHit.collider) {
            
            spriteRenderer.color = new Color(0.82f, 0.34f, 0.34f, 1f);
            ringSpriteRenderer.color = new Color(0.82f, 0.34f, 0.34f, .35f);

            return false;
        }

        ringSpriteRenderer.color = new Color(0.75f, 1f, 0.63f, .35f);
        spriteRenderer.color = Color.white;
        return true;
    }

    void createNewBuilding(Vector3 targetPosition) {

        float elapsedTime = Time.time - activationTime;
        if (Mathf.Approximately(elapsedTime, 0f)) return;

        GameObject buildingsParent = GameObject.Find("User Builds");
        GameObject newObject = Instantiate(buildingPrefabs[spriteRenderer.sprite.name], buildingsParent.transform);

        SpriteRenderer newObjectSpriteRenderer = newObject.GetComponent<SpriteRenderer>();
        newObjectSpriteRenderer.color = new Color(1f, 1f, 1f, .5f);

        newObject.transform.position = targetPosition;

        if (utilitiesManager.Instance.GetConnectMode()) {
            GameObject secondChild = newObject.transform.GetChild(1).gameObject;
            secondChild.SetActive(true);
        }

        utilitiesManager.Instance.NegatePlayerMoney(BuildingPricing(spriteRenderer.sprite.name));
    }

    float getBuildingDiameter() {
        Dictionary<string, float> radii = new Dictionary<string, float>
        {
            { "Castle_Construction_0", 25f },
            { "House_Construction_0", 15f },
            { "Tower_Construction_0", 10f },
            { "pumpkin", 12.5f },
            { "tree", 17.5f },
            { "gold", 27.5f }
        };
        return radii[spriteRenderer.sprite.name];
    }

    float BuildingPricing(string tob){
        Dictionary<string, float> prices = new Dictionary<string, float>
        {
            { "Castle_Construction_0", 60f },
            { "House_Construction_0", 40f },
            { "Tower_Construction_0", 20f },
            { "pumpkin", 10f },
            { "tree", 30f },
            { "gold", 60f }
        };
        return prices[tob];
    }
}
