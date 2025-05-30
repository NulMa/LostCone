using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNumbers : MonoBehaviour{
    public int mapNumber;

    public GameObject[] toOff;
    public GameObject[] toOn;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && GamaManager.Instance.currentStageID != mapNumber) {
            Debug.Log("맵 번호가 " + mapNumber + "로 변경되었습니다.");
            GamaManager.Instance.currentStageID = mapNumber;
            GamaManager.Instance.BGOn(mapNumber);
        }
    }

    public void OnOffAll() {
        Debug.Log("OnOffAll() 호출됨");

        foreach (GameObject obj in toOff) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in toOn) {
            obj.SetActive(true);
        }
    }
}
