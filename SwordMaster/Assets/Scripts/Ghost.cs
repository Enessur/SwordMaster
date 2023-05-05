using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    public GameObject ghost;
    private float ghostDelaySeconds;

    public bool makeGhost = false;

    // Start is called before the first frame update
    void Start()
    {
        ghostDelaySeconds = ghostDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (makeGhost)
        {
            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                bool flipX = GetComponent<SpriteRenderer>().flipX;
                
                if (flipX)
                {
                    Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                    currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                    currentGhost.transform.rotation = new Quaternion(0,180,0,0);
                }
                else
                {
                    Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                    currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                    currentGhost.transform.rotation = new Quaternion(0,0,0,0);
                    
                }
                    
                

                ghostDelaySeconds = ghostDelay;
                Destroy(currentGhost, 1f);
            }
        }
    }
}