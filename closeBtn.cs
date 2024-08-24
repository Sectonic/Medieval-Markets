using UnityEngine;

public class CloseBtn: MonoBehaviour {

    private void OnMouseDown() {

        if (!Input.GetMouseButton(0)) {
            return;
        }

        if (gameObject.name.IndexOf("Popup") != -1) {
            transform.parent.gameObject.SetActive(false);
        } else {
            utilitiesManager.Instance.SetMarketStorageOff();
        }
        
    }


}