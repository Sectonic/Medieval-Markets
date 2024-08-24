using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NpcBehavior : MonoBehaviour
{

    public LayerMask path;

    public string state = "idle";
    public float npcSpeed = 0.01f;

    public Color[] classColors;
    public float pumpkinRate = 0f;
    public float woodRate = 0f;
    public float goldRate = 0f;
    public float pumpkin = 1000f;
    public float wood = 1000f;
    public float gold = 1000f;
    private bool lowerNeeds = true;

    Animator animator;
    SpriteRenderer spriteRenderer;
    PathFinding seeker;

    static readonly float[,] classRates = {
        {20f, 10f, 0f},
        {5f, 20f, 5f},
        {0f, 10f, 20f}
    };

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Tilemap pathTilemap = GameObject.Find("Path").GetComponent<Tilemap>();
        seeker = new PathFinding(pathTilemap, path);
        StartCoroutine(StateChange());

        int classNumber = transform.parent.GetComponent<TownVillagers>().classNumber;

        spriteRenderer.color = classColors[classNumber];
        pumpkinRate = classRates[classNumber, 0];
        woodRate = classRates[classNumber, 1];
        goldRate = classRates[classNumber, 2];

        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
    }

    void Update() {

        if (lowerNeeds) {
            // lowers the NPC's needs
            pumpkin -= GetRandomizedValue(pumpkinRate) * Time.deltaTime;
            wood -= GetRandomizedValue(woodRate) * Time.deltaTime;
            gold -= GetRandomizedValue(goldRate) * Time.deltaTime;
        }

    }

    GameObject GetRandomHouse() {
        
        // gets the current house the NPC is under
        Transform parentTransform = transform.parent;
        // gets the town that the current house is under
        Transform grandParentTransform = parentTransform.parent;

        int randomHouseIndex;

        do
        {
            // getes a random index between 0 and the amount of houses in the town
            randomHouseIndex = Random.Range(0, grandParentTransform.childCount);
        } while (randomHouseIndex == parentTransform.GetSiblingIndex());
        // while statement checks if this random index is NOT the current house's index

        return grandParentTransform.GetChild(randomHouseIndex).gameObject;

    }

    IEnumerator MoveToHouse(GameObject destinationHouse)
    {

        // gets the correct target position the NPC needs to go toward
        Vector2 housePosition = GetTargetPosition(destinationHouse.transform);

        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(.25f);

        // starts the new path
        animator.SetFloat("speed", 1);
        List<Vector2> path =  seeker.GetPath(transform.position, destinationHouse.transform.position);

        foreach (Vector2 point in path)
        {

            while (Vector2.Distance(transform.position, point) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, npcSpeed * Time.deltaTime);
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);

                bool isMovingLeft = point.x < transform.position.x;
                spriteRenderer.flipX = isMovingLeft;
                
                yield return null;
            }

            transform.position = point;
        }

        // stops the NPC
        animator.SetFloat("speed", 0);
        transform.SetParent(destinationHouse.transform);

        yield return new WaitForSeconds(.5f);
        spriteRenderer.enabled = false;

    }

    IEnumerator BuyAtMarket(GameObject market) {
        
        // keeps the current house for later
        Transform houseTransform = transform.parent.transform;

        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(.25f);

        // starts the market path
        animator.SetFloat("speed", 1);
        List<Vector2> path =  seeker.GetPath(transform.position, market.transform.position);

        foreach (Vector2 point in path)
        {   

            while (Vector2.Distance(transform.position, point) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, npcSpeed * Time.deltaTime);
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);

                bool isMovingLeft = point.x < transform.position.x;
                spriteRenderer.flipX = isMovingLeft;

                yield return null;
            }

            transform.position = point;
        }

        // stops the NPC
        animator.SetFloat("speed", 0);
        transform.SetParent(market.transform);

        lowerNeeds = false;
        // waits until the NPC is the first in line
        while (transform.GetSiblingIndex() != 2) {
            yield return null;
        }

        lowerNeeds = true;

        Market marketScript = market.GetComponent<Market>();
        // purchasing implementation
        if (gold < 0 && marketScript.goldAmount > 0) {
            gold = 1000f;
            marketScript.goldAmount -= 1;
            utilitiesManager.Instance.WinGameCheck("gold");
        }
        if (wood < 0 && marketScript.woodAmount > 0) {
            wood = 1000f;
            marketScript.woodAmount -= 1;
            utilitiesManager.Instance.WinGameCheck("wood");
        }
        if (pumpkin < 0 && marketScript.pumpkinAmount > 0) {
            pumpkin = 1000f;
            marketScript.pumpkinAmount -= 1;
            utilitiesManager.Instance.WinGameCheck("food");
        }

        utilitiesManager.Instance.SetMarketStorgeText(market);
        yield return new WaitForSeconds(.5f);
        transform.SetParent(houseTransform);

        animator.SetFloat("speed", 1);
        List<Vector2> pathBack =  seeker.GetPath(transform.position, houseTransform.position);

        foreach (Vector2 point in pathBack)
        {

            while (Vector2.Distance(transform.position, point) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, npcSpeed * Time.deltaTime);
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);

                bool isMovingLeft = point.x < transform.position.x;
                spriteRenderer.flipX = isMovingLeft;

                yield return null;
            }

            transform.position = point;
        }

        animator.SetFloat("speed", 0);

        yield return new WaitForSeconds(.5f);
        spriteRenderer.enabled = false;
    }

    IEnumerator StateChange()
    {
        while (true)
        {
            
            bool market = false;
            if (pumpkin < 0 || gold < 0 || wood < 0) {

                GameObject nearestMarket = FindClosestMarket();
                if (nearestMarket != null && getMarketCapacity(nearestMarket.name) > (nearestMarket.transform.childCount - 2)) {
                    
                    state = "market";
                    market = true;
                    yield return BuyAtMarket(nearestMarket);

                }

            } 

            if (!market) {
                state = GetRandomState();

                if (state == "idle") {
                    
                    yield return new WaitForSeconds(5f);

                } else {

                    GameObject randomHouse = GetRandomHouse();
                    yield return MoveToHouse(randomHouse);

                }
            }

            yield return new WaitForSeconds(1f);

        }
    }

    Vector2 GetTargetPosition(Transform houseTransform) {

        if (houseTransform.name.IndexOf("House Right") != -1) {
            // the position is oriented rightward to account for the path
            return new Vector2(houseTransform.position.x + 1.5f, houseTransform.position.y);
        } else if (houseTransform.name.IndexOf("House Left")  != -1) {
            // the position is oriented leftward to account for the path
            return new Vector2(houseTransform.position.x - 1.5f, houseTransform.position.y);
        } else {
            // the position is oriented downward to account for the path
            return new Vector2(houseTransform.position.x, houseTransform.position.y - 1.5f);
        }

    }

    string GetRandomState()
    {
        int randomIndex = Random.Range(0, 2);
        string[] states = {"idle", "moving"};
        return states[randomIndex];
    }

    GameObject FindClosestMarket()
    {
        GameObject[] marketObjects = GameObject.FindGameObjectsWithTag("market");

        if (marketObjects.Length > 0)
        {
            GameObject closestObject = GetClosestColliderInRange(marketObjects);

            if (closestObject != null)
            {
                return closestObject;
            }
        }

        return null;

    }

    GameObject GetClosestColliderInRange(GameObject[] colliders)
    {
        float minDistance = float.MaxValue;
        GameObject closestObject = null;

        foreach (GameObject market in colliders)
        {
            float distance = Vector2.Distance(transform.position, market.transform.position);

            if (distance < minDistance && distance < getMarketRange(market.name))
            {
                minDistance = distance;
                closestObject = market;
            }
        }

        return closestObject;
    }

    float getMarketRange(string name) {
        if (name.IndexOf("Big") != -1) {
            return 25f;
        } else if (name.IndexOf("Medium") != -1) {
            return 15f;
        } else {
            return 10f;
        }
    }

    int getMarketCapacity(string name) {
        if (name.IndexOf("Big") != -1) {
            return 10;
        } else if (name.IndexOf("Medium") != -1) {
            return 5;
        } else {
            return 2;
        }
    }

    float GetRandomizedValue(float rate)
    {
        if (rate == 0f)
        {
            return 0f;
        }
        else
        {
            float randomFactor = Random.Range(-5f, 5f);
            return rate + randomFactor;
        }
    }

}