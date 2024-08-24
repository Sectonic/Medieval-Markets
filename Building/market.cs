using UnityEngine;

public class Market : MonoBehaviour
{
    // Start is called before the first frame update
    public int goldAmount = 0;
    public int woodAmount = 0;
    public int pumpkinAmount = 0;

    void OnMouseDown() {

        if (!Input.GetMouseButton(0)) {
            return;
        }

        if (!utilitiesManager.Instance.GetConnectMode()) {
            utilitiesManager.Instance.SetMarketStorageOn(gameObject);
        }
        
    }

}
