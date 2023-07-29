using System.Collections;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    LineRenderer line;

    [SerializeField] LayerMask grappable;
    [SerializeField] LayerMask wall;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10;
    [SerializeField] float grappleShootSpeed = 20f;
    [SerializeField] Sprite openMouth;
    [SerializeField] Sprite closeMouth;
    SpriteRenderer playerHeadSprite;

    [Header("General Settings:")]
    [SerializeField] private int percision = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    float moveTime = 0;

    bool isGrappling = false;
    [HideInInspector] public bool retracting = false;

    Vector2 target;

    private void Start()
    {
        //line = GetComponent<LineRenderer>();
        playerHeadSprite = transform.Find("player_head").GetComponent<SpriteRenderer>();
        //line = GetComponentInChildren<LineRenderer>();
        line = transform.Find("player_head/mouth/firepoint/tongue").GetComponent<LineRenderer>();

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !isGrappling)
        {
            playerHeadSprite.sprite = openMouth;
            StartGrapple();
        }

        if (retracting)
        {
            Vector2 grapplePos = Vector2.Lerp(transform.position, target, grappleSpeed * Time.deltaTime);
            transform.position = grapplePos;

            line.SetPosition(0, transform.position);
            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                retracting = false;
                isGrappling = false;
                line.enabled = false;
                playerHeadSprite.sprite = closeMouth;
            }
        }
    }

    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappable);
        if(hit.collider == null) { 
            hit = Physics2D.Raycast(transform.position, direction, maxDistance, wall); 
        }
        if (hit.collider != null)
        {
            isGrappling = true;
            target = hit.point;
            line.enabled = true;
            line.positionCount = 2;

            StartCoroutine(Grapple());
        }
        else { playerHeadSprite.sprite = closeMouth; }
    }
    IEnumerator Grapple()
    {
        float t = 0;
        float time = 10;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 newPos;

        for(; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(transform.position, target, t / time);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);
            yield return null;
        }
        line.SetPosition(1, target);
        retracting = true;
    }
}
