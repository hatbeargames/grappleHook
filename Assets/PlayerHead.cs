using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    //Performs the rotation of the head to face the mouse. Also flips the body when facing left or right.
    
    [SerializeField] float angle;

    GameObject playerObject;
    private void Start()
    {
        playerObject = transform.parent.gameObject;
    }
    void Update()
    {

        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        //Ta Daaa
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        if (angle! > -90 && angle < 90)
        {
            //{
            //    //isFacingRight = !isFacingRight;
            //    //Debug.Log("Face Left");
            //    if (transform.localScale.y != 1)
            //    {
            //        Vector3 localScale = transform.localScale;
            //        localScale.y *= -1f;
            //        localScale.y *= -1f;
            //        transform.localScale = localScale;
            //    }
            //}
            //else
            //{
            //    if (transform.localScale.y != -1)
            //    {
            //        Vector3 localScale = transform.localScale;
            //        localScale.y *= -1f;
            //        transform.localScale = localScale;
            //    }
            //}
            if(playerObject.transform.localScale.x != -1)
            {
                Vector3 localScale = playerObject.transform.localScale;
                Vector3 playerHeadLocalScale = playerObject.transform.localScale;
                localScale.x *= -1f;
                if(playerHeadLocalScale.y != -1 && playerHeadLocalScale.x != -1)
                {
                    playerHeadLocalScale.y = 1f;
                    playerHeadLocalScale.x = 1f;
                    //Debug.Log("transforminging head facing left" + playerHeadLocalScale);
                    transform.localScale = playerHeadLocalScale;
                }
                playerObject.transform.localScale = localScale;
            }
        }
        else
        {
            if (playerObject.transform.localScale.x != 1)
            {
                Vector3 localScale = playerObject.transform.localScale;
                Vector3 playerHeadLocalScale = playerObject.transform.localScale;
                localScale.x *= -1f;
                if (playerHeadLocalScale.y != -1)
                {
                    playerHeadLocalScale.y = -1f;
                    //Debug.Log("transforminging head facing right" + playerHeadLocalScale);
                    transform.localScale = playerHeadLocalScale;
                }
                playerObject.transform.localScale = localScale;
            }
        }
    }
        float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
}
