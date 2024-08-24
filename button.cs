using UnityEngine;

public class Button: MonoBehaviour {
	
    public Sprite normalSprite;
    public Sprite clickSprite;

    private SpriteRenderer spriteRenderer;

    public GameObject[] otherButtons;
    public GameObject selectionMenu;
    private Transform icon;

    private bool clicked = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        icon = transform.GetChild(0);
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
    }

    public void TurnOffOtherBtn() {

        if (clicked) {

            clicked = false;
            spriteRenderer.sprite = normalSprite;
            icon.position += new Vector3(0f,.075f,0f);
            
            if (selectionMenu != null) {
                selectionMenu.SetActive(false);
            }

        }

    }

    private void HandleButtonClick() {

        spriteRenderer.sprite = clickSprite;
        icon.position += new Vector3(0f, -0.075f, 0f);

        if (selectionMenu != null) {

            string otherName = transform.name == "Market" ? "Production" : "Market";
            GameObject otherSelection = GameObject.Find($"{otherName} Selection");
            GameObject building = GameObject.Find("Build Mode");

            if (otherSelection != null && building != null) {
                DeselectAllItems(otherSelection);
            }

            selectionMenu.SetActive(true);

        }

        if (otherButtons != null && otherButtons.Length > 0) {
            foreach (GameObject btn in otherButtons) {
                Button btnScript = btn.GetComponent<Button>();
                btnScript.TurnOffOtherBtn();
            }
        }
    }

    private void HandleButtonUnclick() {
        spriteRenderer.sprite = normalSprite;
        icon.position += new Vector3(0f, 0.075f, 0f);

        if (otherButtons.Length > 0) {

            GameObject building = GameObject.Find("Build Mode");

            if (building != null) {
                DeselectAllItems(selectionMenu);
            }

        }

        if (selectionMenu != null) {
            selectionMenu.SetActive(false);
        }

    }

    private void DeselectAllItems(GameObject selection) {

        Transform itemsTransform = selection.transform.Find("Items");
        Transform[] itemChildren = itemsTransform.GetComponentsInChildren<Transform>();

        if (itemChildren != null && itemChildren.Length > 0) {
            foreach (Transform child in itemChildren) {
                buildingSelection btnScript = child.gameObject.GetComponent<buildingSelection>();
                if (btnScript != null) {
                    btnScript.TurnOffOtherBtn();
                }
            }
        }

        utilitiesManager.Instance.setBuildMode(false);

    }

}