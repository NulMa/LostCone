using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDataManager : MonoBehaviour {
    public static ItemDataManager Instance { get; private set; }
    public bool[] bools; // 해당 스테이지 아이템 보유 여부 배열

    public int itemCount; // 아이템 개수 
    public int CurrentMapID; // 현재 맵 번호
    public Dictionary<int, bool> MapClearStatus = new Dictionary<int, bool>(); // 맵 클리어 여부

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        // 현재 맵 ID 불러오기
        CurrentMapID = PlayerPrefs.GetInt("CurrentMapID", 0);

        // 맵 클리어 상태 불러오기
        foreach (var mapID in MapClearStatus.Keys.ToList()) {
            MapClearStatus[mapID] = PlayerPrefs.GetInt($"Map_{mapID}_Clear", 0) == 1;
        }

        // 현재 맵의 아이템 보유 여부 불러오기
        UpdateItemOwnershipArray();

        Debug.Log($"[ItemDataManager] 데이터 로드 완료: CurrentMapID={CurrentMapID}, 아이템 개수={itemCount}");
    }

    public void changeStageNum(int number) {
        // 새로운 맵 번호 설정
        CurrentMapID = number;

        // PlayerPrefs에 새로운 맵 번호 저장
        PlayerPrefs.SetInt("CurrentMapID", CurrentMapID);
        PlayerPrefs.Save();

        // 모든 자식 MapItems 비활성화
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null) {
                child.gameObject.SetActive(false);
            }
        }

        // 해당 CurrentMapID에 맞는 MapItems 활성화
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null && mapItems.MapID == CurrentMapID) {
                child.gameObject.SetActive(true);
                Debug.Log($"[ItemDataManager] Map {CurrentMapID} 활성화됨.");
                break;
            }
        }
    }

    public void SaveCurrentMapData() {
        PlayerPrefs.SetInt("CurrentMapID", CurrentMapID);

        foreach (var map in MapClearStatus) {
            PlayerPrefs.SetInt($"Map_{map.Key}_Clear", map.Value ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    public void LoadCurrentMapData() {
        CurrentMapID = PlayerPrefs.GetInt("CurrentMapID", 0);

        foreach (var map in MapClearStatus.Keys) {
            MapClearStatus[map] = PlayerPrefs.GetInt($"Map_{map}_Clear", 0) == 1;
        }
    }

    private void Update() {
        // 현재 스테이지 아이템 보유 여부 배열을 업데이트
        UpdateItemOwnershipArray();
    }

    private void UpdateItemOwnershipArray() {
        if (CurrentMapID == -1)
            return;

        // 현재 맵의 아이템 개수를 가져오기 위해 MapItems를 참조
        MapItems currentMap = null;

        // ItemDataManager의 자식 오브젝트 중 MapItems를 찾음
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null && mapItems.MapID == CurrentMapID) {
                currentMap = mapItems;
                break;
            }
        }

        // MapItems가 없거나 MapID가 일치하지 않으면 종료
        if (currentMap == null) {
            Debug.LogWarning($"[ItemDataManager] MapID {CurrentMapID}에 해당하는 MapItems를 찾을 수 없습니다.");
            return;
        }

        // 아이템 개수 업데이트
        int itemCountInMap = currentMap.Items.Count; // List에서는 Count를 사용
        itemCount = 0;

        // 아이템 개수에 맞게 bools 배열 초기화
        if (bools == null || bools.Length != itemCountInMap) {
            bools = new bool[itemCountInMap];
        }

        // isHave가 true인 경우 bools 배열과 itemCount 업데이트
        for (int i = 0; i < itemCountInMap; i++) {
            if (currentMap.Items[i].isHave) {
                bools[i] = true;
                itemCount++;
            }
            else {
                bools[i] = false;
            }
        }
    }
}



/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapAndItem {
    public GameObject map;
    public bool isClear;
}

public class ItemDataManager : MonoBehaviour{
    public int currentStageID;
    public MapAndItem[] mapAndItems;


    private void Awake() {
       
    }




    public void LoadMapItemInfo() {
        if (!mapAndItems[currentStageID].isClear) {
            for(int i = 0; i < mapAndItems[currentStageID].map.GetComponent<MapItems>().mapItems.Length; i++) {
                if (mapAndItems[currentStageID].map.GetComponent<MapItems>().mapItems[i].GetComponent<ItemGet>().isHave) {
                    mapAndItems[currentStageID].map.GetComponent<MapItems>().mapItems[i].gameObject.SetActive(false);

                } 
            }
        }
    }
}
*/
