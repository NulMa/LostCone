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
            int haveValue = PlayerPrefs.GetInt($"Map_{MapID}_Item_{item.ItemID}_Have", 0);
            Debug.Log($"[디버그] MapID:{MapID}, ItemID:{item.ItemID}, PlayerPrefs:{haveValue}");
            item.isHave = haveValue == 1;
            if (item.isHave) {
                Debug.Log($"[디버그] 비활성화됨: MapID:{MapID}, ItemID:{item.ItemID}");
                item.gameObject.SetActive(false);
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