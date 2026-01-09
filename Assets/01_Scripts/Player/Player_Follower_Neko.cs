    using UnityEngine;

public class Player_Follower_Neko : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float walkDistance = 2f;
    public float jumpDistance = 1f;
    public float farDistance = 10f;
    public float followSpeed = 2f;

    [Header("Detection")]
    public Vector2 rayOffset;
    public float rayDistance = 1f;
    public LayerMask detectLayer;

    public Transform player;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public enum State
    {
        Idle,
        Run,
        Jumping
    }

    public State currentState = State.Idle;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (detectLayer == 0)
        {
            detectLayer = LayerMask.GetMask("Default", "Tilemap");
        }
    }


    public float jumpDelay = 2f;
    public float cooldownTimer = 0f;

    private void FixedUpdate()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }

        //rigid.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

        anim.Play(currentState.ToString());

        float curDistance = Vector2.Distance(player.position, transform.position);

        if (farDistance < curDistance)
        {
            // Teleport to player if too far
            int direction = player.position.x > transform.position.x ? 1 : -1;
            transform.position = player.position + new Vector3(direction * 3, 0f, 0f);
            currentState = State.Idle;
            return;
        }

        if(walkDistance > curDistance)
        {
            // Idle if close enough
            rigid.linearVelocity = Vector2.zero;
            currentState = State.Idle;

            return;
        }

        // Calculate distance to player
        float distanceX = player.position.x - transform.position.x;

        if (distanceX < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        distanceX = Mathf.Abs(distanceX);

        if (distanceX > walkDistance)
        {
            int direction = player.position.x > transform.position.x ? 1 : -1;

            // Raycast Detection
            Vector2 origin = (Vector2)transform.position + new Vector2(rayOffset.x * direction, rayOffset.y);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rayDistance, detectLayer);

            if (hit.collider != null)
            {
                // Detection logic here (currently just debug)
                Debug.Log($"Detected: {hit.collider.name}");
                if(currentState == State.Run && cooldownTimer <= 0){
                    //jump
                    rigid.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    cooldownTimer = jumpDelay;
                }
            }
            
            // Interpolate speed: 0 at walkDistance, 1.5 at farDistance
            float t = Mathf.Clamp01((distanceX - walkDistance) / ((farDistance/2) - walkDistance));
            followSpeed = Mathf.Lerp(0.5f, 1.5f, t);

            rigid.position += new Vector2(direction * speed * followSpeed * Time.fixedDeltaTime, 0);
            currentState = State.Run;
        }

    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            int direction = player.position.x > transform.position.x ? 1 : -1;
            Vector2 origin = (Vector2)transform.position + new Vector2(rayOffset.x * direction, rayOffset.y);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + Vector2.right * direction * rayDistance);
        }
    }

}
