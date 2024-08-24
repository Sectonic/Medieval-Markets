using UnityEngine;

public class buildingSelection : MonoBehaviour
{

    public Sprite normalSprite;
    public Sprite clickSprite;
    
    public bool halfPoints = false;
        
    private SpriteRenderer spriteRenderer;

    private bool clicked = false;
    private Sprite currentBuilding;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentBuilding = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    private void OnMouseDown() {
        if (!Input.GetMouseButton(0)) {
            return;
        }

        clicked = !clicked;

        if (clicked) {
            HandleButtonClick();
        } else {
            HandleButtonUnclick();
        }

        if (clicked) {
            spriteRenderer.sprite = clickSprite;
            utilitiesManager.Instance.setBuildMode(true, currentBuilding, halfPoints);

        } else {
            spriteRenderer.sprite = normalSprite;
            utilitiesManager.Instance.setBuildMode(false);
        }

    }

    public void TurnOffOtherBtn() {
        if (clicked) {
            spriteRenderer.sprite = normalSprite;
            clicked = false;
        }
    }

    private void HandleButtonClick() {

        Transform[] allButtons = transform.parent.gameObject.GetComponentsInChildren<Transform>();

        if (allButtons != null && allButtons.Length > 0) {
            foreach (Transform btn in allButtons) {
                if (btn != transform) {
                    buildingSelection btnScript = btn.gameObject.GetComponent<buildingSelection>();
                    if (btnScript != null) {
                        btnScript.TurnOffOtherBtn();
                    }
                }
            }
        }

        spriteRenderer.sprite = clickSprite;
        utilitiesManager.Instance.setBuildMode(true, currentBuilding, halfPoints);
    }

    private void HandleButtonUnclick() {

        spriteRenderer.sprite = normalSprite;
        utilitiesManager.Instance.setBuildMode(false);

    }

}
