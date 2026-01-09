using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverNeko : MonoBehaviour
{
    public bool isCleard;
    public GameObject Gate;
    public GameObject Lever;

    Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            Debug.Log("Lever Activated");
            anim.SetTrigger("ActiveLever");
            Gate.GetComponent<Animator>().SetBool("Close", true);
            Lever.GetComponent<Animator>().SetBool("Close", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            Debug.Log("Lever Activated");
            anim.SetTrigger("ActiveLever");
            Gate.GetComponent<Animator>().SetBool("Close", false);
            Lever.GetComponent<Animator>().SetBool("Close", false);
        }
    }


    public void Cleard()
    {
        Gate.GetComponent<Animator>().SetBool("Close", false);
        Lever.GetComponent<Animator>().SetBool("Close", false);

        //just deactivate components
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

    }

}
