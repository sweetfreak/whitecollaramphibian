using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenBackground : MonoBehaviour
{
    // [SerializeField] private GameObject bg1;
    // [SerializeField] private GameObject bg2;
    private Vector2 startPos;
    private float repeatHeight;
   [SerializeField] private float speed = 38f;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        repeatHeight = GetComponent<BoxCollider2D>().size.y / 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
        
        if (transform.position.y > startPos.y + repeatHeight)
        {
            transform.position = startPos;
        }
        
        // //change location
        // bg1.transform.position = new Vector2(bg1.transform.position.x, bg1.transform.position.y * (Time.deltaTime * 2));
        // bg2.transform.position = new Vector2(bg2.transform.position.x, bg2.transform.position.y * (Time.deltaTime * 2));
        //
        // //when y = ?? move to new location
        // if (bg2.transform.position.y > 33f)
        // {
        //     bg2.transform.position = startPos;
        // }
        // if (bg1.transform.position.y > 33f)
        // {
        //     bg1.transform.position = startPos;
        // }
    }
}
