using System.Collections;
using System.Collections.Generic;
using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

public class LeverNeko : MonoBehaviour
{
    public bool isCleard;
    public GameObject Gate;
    public GameObject backDoor;
    public GameObject Lever;

    public Sprite clearedSprite;
    public SoundSO doorSound;
    Animator anim;
    private readonly PlaySFXEvent _playSFXEvent = new PlaySFXEvent();
    
    void Start()
    {
        if(AchiveManager.instance.IsAchiveCleared("LostCone"))
        {
            backDoor.GetComponent<SpriteRenderer>().sprite = clearedSprite;
        }


        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            Debug.Log("Lever Activated");
            anim.SetTrigger("ActiveLever");
            Gate.GetComponent<Animator>().SetBool("Close", true);
            Lever.GetComponent<Animator>().SetBool("Close", true);
            backDoor.GetComponent<Animator>().SetBool("Close", true);
            GameEventBus.RaiseEvent(_playSFXEvent.Initialize(doorSound, Gate.transform.position));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            Debug.Log("Lever Activated");
            anim.SetTrigger("ActiveLever");
            Gate.GetComponent<Animator>().SetBool("Close", false);
            Lever.GetComponent<Animator>().SetBool("Close", false);
            backDoor.GetComponent<Animator>().SetBool("Close", false);
            GameEventBus.RaiseEvent(_playSFXEvent.Initialize(doorSound, Gate.transform.position));
        }
    }


    public void Cleard()
    {
        Gate.GetComponent<Animator>().SetBool("Close", false);
        Lever.GetComponent<Animator>().SetBool("Close", false);
        backDoor.GetComponent<Animator>().SetBool("Close", false);

        //just deactivate components
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GameEventBus.RaiseEvent(_playSFXEvent.Initialize(doorSound, Gate.transform.position));
    }

}
