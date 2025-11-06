using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNumbers : MonoBehaviour{
    public int mapNumber;

    public GameObject[] toOff;
    public GameObject[] toOn;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && GamaManager.Instance.currentStageID != mapNumber) {
            Debug.Log("맵 번호가 " + mapNumber + "로 설정되었습니다.");
            GamaManager.Instance.currentStageID = mapNumber;
            // [수정] ItemDataManager의 맵 ID도 즉시 동기화시켜주어야 합니다.
            GamaManager.Instance.ItemDataManager.CurrentMapID = mapNumber;
            GamaManager.Instance.BGOn(mapNumber);
            AudioManager.Instance.PlayBGM(mapNumber, true, 3f);

            // [수정] 새 맵의 데이터를 명시적으로 로드하도록 로직 추가
            MapItems newMap = null;
            foreach (Transform child in GamaManager.Instance.ItemDataManager.transform) {
                MapItems mapItems = child.GetComponent<MapItems>();
                if (mapItems != null && mapItems.MapID == mapNumber) {
                    newMap = mapItems;
                    break;
                }
            }

            if (newMap != null) {
                newMap.LoadMapData(); // 새 맵의 아이템 'isHave' 상태를 PlayerPrefs에서 로드
            }

            // 이제 최신 데이터로 카운트를 재계산하고 UI를 갱신
            GamaManager.Instance.ItemDataManager.UpdateItemOwnershipArray();
            GamaManager.Instance.UIManager.UpdateItemCount(GamaManager.Instance.ItemDataManager.itemCount, GamaManager.Instance.ItemDataManager.bools.Length);
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
