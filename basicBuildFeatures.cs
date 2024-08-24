using UnityEngine;

public class basicBuildFeatures : MonoBehaviour
{

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float bottomY = transform.position.y - spriteRenderer.bounds.size.y / 2;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-bottomY * 100f);
    }

}
