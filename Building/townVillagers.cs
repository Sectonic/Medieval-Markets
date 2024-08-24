using UnityEngine;

public class TownVillagers : MonoBehaviour {

    public Sprite[] classOfSprites;
    public GameObject villager;

    public int maxVillagers;
    public int classNumber;

    public void Start() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        classNumber = Random.Range(0, classOfSprites.Length);
        spriteRenderer.sprite = classOfSprites[classNumber];

        for (int i = 0; i < maxVillagers; i++)
        {
            GameObject newVillager = Instantiate(
                villager, 
                transform.position, 
                maxVillagers == 4 ? Quaternion.Euler(0, 0, 9-0) : Quaternion.identity, 
                transform
            );
            newVillager.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

    }

}