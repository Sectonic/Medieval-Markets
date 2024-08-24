using UnityEngine;

public class SortingBehavior : MonoBehaviour
{

    public float sortingOffsetPercentage = 0f;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        float yOffset = transform.position.y - spriteRenderer.bounds.size.y / 2;
        float sortingOffset = spriteRenderer.bounds.size.y * sortingOffsetPercentage / 100f;

        if (gameObject.name.IndexOf("Farm") != -1) {
            spriteRenderer.sortingLayerName = "UI Base";
            spriteRenderer.sortingOrder = 0;
        } else {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-(yOffset + sortingOffset) * 100f);
        }
        
    }

}
