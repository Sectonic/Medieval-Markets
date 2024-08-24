using UnityEngine;

public class Production : MonoBehaviour
{

    public LayerMask marketMask;
    public GameObject currentMarket;
    public float maxDistance;
    
    private LineRenderer lineRenderer;
    private bool drag = false;

    void Start() {
        InvokeRepeating("GiveToMarket", 0, 2.0f);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = new Color(1f, 1f, 1f, .7f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, .7f);
    }

    Vector2 getProdPosition() {
        Vector2 pos = transform.position;
        if (gameObject.name.IndexOf("Tree") != -1) {
            pos.y -= .75f;
        } else if (gameObject.name.IndexOf("Gold") != -1) {
            pos.y -= .25f;
        }
        return pos;
    }

    Vector2 getMarketPosition() {
        Vector2 pos = currentMarket.transform.position;
        if (currentMarket.name.IndexOf("Small") != -1) {
            pos.y -= .2f;
        }
        return pos;
    }

    void GiveToMarket() {

        if (currentMarket != null) {
            Market marketScript = currentMarket.GetComponent<Market>();
            
            if (gameObject.name.IndexOf("Gold") != -1) {
                marketScript.goldAmount += 1;
            } else if (gameObject.name.IndexOf("Tree") != -1) {
                marketScript.woodAmount += 1;
            } else {
                marketScript.pumpkinAmount += 1;
            }

            utilitiesManager.Instance.SetMarketStorgeText(currentMarket);

        }

    }

    void Update() {

        Vector2 pos = getProdPosition();

        if (utilitiesManager.Instance.GetConnectMode() && drag) {
            
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, pos);
            lineRenderer.SetPosition(1, mouseWorldPos);

            float distance = Vector2.Distance(mouseWorldPos, pos);
            if (distance > maxDistance) {

                lineRenderer.startColor = new Color(0.82f, 0.34f, 0.34f, .9f);
                lineRenderer.endColor = new Color(0.82f, 0.34f, 0.34f, .9f);

            } else {

                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, marketMask);
                if (hit.collider)
                {
                    lineRenderer.startColor = new Color(0.75f, 1f, 0.63f, .9f);
                    lineRenderer.endColor = new Color(0.75f, 1f, 0.63f, .9f);
                } else {
                    lineRenderer.startColor = new Color(1f, 1f, 1f, .7f);
                    lineRenderer.endColor = new Color(1f, 1f, 1f, .7f);
                }

            }

        } else if (utilitiesManager.Instance.GetConnectMode() && currentMarket) {

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, pos);
            lineRenderer.SetPosition(1, getMarketPosition());
            lineRenderer.startColor = new Color(0.75f, 1f, 0.63f, .9f);
            lineRenderer.endColor = new Color(0.75f, 1f, 0.63f, .9f);

        } else {

            lineRenderer.positionCount = 0;

        }

    }

    void OnMouseDown() {

        if (!Input.GetMouseButton(0)) {
            return;
        }

        drag = true;
        
    }

    void OnMouseUp() {

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, marketMask);

        if (hit.collider != null)
        {
            currentMarket = hit.collider.gameObject;
        }

        drag = false;
    }


}
