using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 0.01f;
    public float sortingOffset = 0f;
    public Transform mapTransform;
    public SpriteRenderer mapSpriteRenderer;
    public Animator mapAnimator;

    private bool ofsetFlip = false;
    private bool flipX = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;
    public Vector3 offset = Vector3.zero;
    private Vector3 movement;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f - sortingOffset);

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            moveY += 1f; 
        } 

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            moveX -= 1f; 
            if (ofsetFlip == false){
                offset.x = offset.x*-1;
            }
            ofsetFlip = true;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            moveY -= 1f; 
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            moveX += 1f; 
            if (ofsetFlip == true){
                offset.x = offset.x*-1;
            }
            ofsetFlip = false;
        }

        bool isMoving = moveX != 0f || moveY != 0f;
        flipX = isMoving ? (moveX < 0) : flipX;
        spriteRenderer.flipX = flipX;
        mapSpriteRenderer.flipX = flipX;

        movement = new Vector3(moveX, moveY).normalized;
        animator.SetBool("Running", isMoving);

        mapTransform.position = rb.position + Vector2.down * 30; 
        mapAnimator.SetBool("Running", isMoving);

    }

    public void FixedUpdate() {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}
