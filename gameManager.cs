using UnityEngine;

public class gameManager : MonoBehaviour
{

    public Texture2D pointerCursor;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Cursor.SetCursor(pointerCursor, Vector2.zero, CursorMode.Auto);
    }

}
