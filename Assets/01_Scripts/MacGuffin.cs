using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacGuffin : MonoBehaviour{

    public GameObject wareWolf;
    public Transform destinaion;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (GamaManager.Instance.SceneManager.scenes[1].isDone)
            return;
        if(collision.tag == "Player") {
            wareWolf.SetActive(true);
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void FixedUpdate() {
        if (wareWolf.activeSelf) {
            wareWolf.transform.position = Vector3.MoveTowards(wareWolf.transform.position, destinaion.position, 0.3f);
            if(Vector2.Distance(wareWolf.transform.position, destinaion.position)< 2) {
                wareWolf.SetActive(false);
            }
        }
    }
}
