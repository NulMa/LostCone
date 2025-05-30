using System.Collections.Generic;
using UnityEngine;

public class MapItems : MonoBehaviour {
    public int MapID; // 맵 번호
    public bool IsCleared; // 클리어 여부
    public List<ItemGet> Items; // 해당 맵의 아이템 리스트
    public int haveCount; // 보유한 아이템 개수

    public void SaveMapData() {
        PlayerPrefs.SetInt($"Map_{MapID}_Clear", IsCleared ? 1 : 0);

        foreach (var item in Items) {
            PlayerPrefs.SetInt($"Map_{MapID}_Item_{item.ItemID}_Have", item.isHave ? 1 : 0);
        }

        PlayerPrefs.Save();
        DebugMapData();
    }

    public void LoadMapData() {
        // 맵 클리어 여부 로드
        IsCleared = PlayerPrefs.GetInt($"Map_{MapID}_Clear", 0) == 1;

        // 아이템 보유 상태 로드 및 비활성화
        foreach (var item in Items) {
            item.isHave = PlayerPrefs.GetInt($"Map_{MapID}_Item_{item.ItemID}_Have", 0) == 1;
            if (item.isHave) {
                item.gameObject.SetActive(false); // 보유한 아이템 비활성화
            }
        }
    }
    public void DebugMapData() {
        // 맵 ID와 클리어 여부 출력
        Debug.Log($"[Map Debug] Map ID: {MapID}, Cleared: {IsCleared}");

        // 아이템 보유 상태 출력
        if (Items != null && Items.Count > 0) {
            foreach (var item in Items) {
                string itemStatus = item.isHave ? "Owned" : "Not Owned";
                Debug.Log($"[Map Debug] Item ID: {item.ItemID}, Status: {itemStatus}");
            }
        }
        else {
            Debug.Log("[Map Debug] No items found in this map.");
        }
    }

}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItems : MonoBehaviour{
    public int stageID;
    public GameObject[] mapItems;
    public bool isClear;

    private void Awake() {
        
    }
}
*/