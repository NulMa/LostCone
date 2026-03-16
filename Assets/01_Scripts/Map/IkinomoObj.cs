using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkinomoObj : MonoBehaviour
{
    public GameObject Wall;


    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (PlayerPrefs.GetInt("HiddenWall_OnFunction_1", 0) == 1){
            animator.SetTrigger("Cleared");
        }
    }

    public void WallOff(){
        Wall.GetComponent<HiddenWall>().HideWallByFunction();
    }


}
