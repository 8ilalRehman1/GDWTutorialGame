using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carcollision : MonoBehaviour
{

    Rigidbody2D rb;

    CarMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<CarMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Barrier")
        {
            rb.velocity = rb.velocity * 2;
            Debug.Log("Car hit");
        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Barrier")
        {
            Debug.Log("Car left");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "boostPad")
        {
            movement.ApplySpeedBoost();
        }
    }
}
