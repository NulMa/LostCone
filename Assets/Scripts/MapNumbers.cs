using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNumbers : MonoBehaviour{
    public int mapNumber;

    public GameObject[] toOff;
    public GameObject[] toOn;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && GamaManager.Instance.currentStageID != mapNumber) {
            Debug.Log("�� ��ȣ�� " + mapNumber + "�� ����Ǿ����ϴ�.");
            GamaManager.Instance.currentStageID = mapNumber;
            GamaManager.Instance.BGOn(mapNumber);
        }
    }

    public void OnOffAll() {
        Debug.Log("OnOffAll() ȣ���");

        foreach (GameObject obj in toOff) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in toOn) {
            obj.SetActive(true);
        }
    }
}
