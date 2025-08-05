using System.Collections.Generic;
using UnityEngine;

public class MapItems : MonoBehaviour {
    public int MapID; // 맵 번호
    public bool IsCleared; // 클리어 상태
    public List<ItemGet> Items; // 해당 맵의 아이템 리스트
    public int haveCount; // 플레이어 아이템 개수

    public void SaveMapData() {
        PlayerPrefs.SetInt($"Map_{MapID}_Clear", IsCleared ? 1 : 0);

        foreach (var item in Items) {
            PlayerPrefs.SetInt($"Map_{MapID}_Item_{item.ItemID}_Have", item.isHave ? 1 : 0);
        }

        PlayerPrefs.Save();
        DebugMapData();
    }

    public void LoadMapData() {
        // 맵 클리어 상태 로드
        IsCleared = PlayerPrefs.GetInt($"Map_{MapID}_Clear", 0) == 1;

        // 아이템이 있을 경우 로드 후 비활성화
        foreach (var item in Items) {
            int haveValue = PlayerPrefs.GetInt($"Map_{MapID}_Item_{item.ItemID}_Have", 0);
            Debug.Log($"[로딩] MapID:{MapID}, ItemID:{item.ItemID}, PlayerPrefs:{haveValue}");
            item.isHave = haveValue == 1;
            if (item.isHave) {
                Debug.Log($"[로딩] 비활성화됨: MapID:{MapID}, ItemID:{item.ItemID}");
                item.gameObject.SetActive(false);
            }
        }
    }
    public void DebugMapData() {
        // 맵 ID로 클리어 상태 저장
        Debug.Log($"[Map Debug] Map ID: {MapID}, Cleared: {IsCleared}");

        // 아이템의 보유 상태 저장
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