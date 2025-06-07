using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDataManager : MonoBehaviour {
    public static ItemDataManager Instance { get; private set; }
    public bool[] bools; // �ش� �������� ������ ���� ���� �迭

    public int itemCount; // ������ ���� 
    public int CurrentMapID; // ���� �� ��ȣ
    public Dictionary<int, bool> MapClearStatus = new Dictionary<int, bool>(); // �� Ŭ���� ����

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        // ���� �� ID �ҷ�����
        CurrentMapID = PlayerPrefs.GetInt("CurrentMapID", 0);

        // �� Ŭ���� ���� �ҷ�����
        foreach (var mapID in MapClearStatus.Keys.ToList()) {
            MapClearStatus[mapID] = PlayerPrefs.GetInt($"Map_{mapID}_Clear", 0) == 1;
        }

        // ���� ���� ������ ���� ���� �ҷ�����
        UpdateItemOwnershipArray();

        Debug.Log($"[ItemDataManager] ������ �ε� �Ϸ�: CurrentMapID={CurrentMapID}, ������ ����={itemCount}");
    }

    public void changeStageNum(int number) {
        // ���ο� �� ��ȣ ����
        CurrentMapID = number;

        // PlayerPrefs�� ���ο� �� ��ȣ ����
        PlayerPrefs.SetInt("CurrentMapID", CurrentMapID);
        PlayerPrefs.Save();

        // ��� �ڽ� MapItems ��Ȱ��ȭ
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null) {
                child.gameObject.SetActive(false);
            }
        }

        // �ش� CurrentMapID�� �´� MapItems Ȱ��ȭ
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null && mapItems.MapID == CurrentMapID) {
                child.gameObject.SetActive(true);
                Debug.Log($"[ItemDataManager] Map {CurrentMapID} Ȱ��ȭ��.");
                break;
            }
        }
    }

    public void SaveCurrentMapData() {
        PlayerPrefs.SetInt("CurrentMapID", CurrentMapID);

        // ��ü �� Ŭ���� ���� ����
        foreach (var map in MapClearStatus) {
            PlayerPrefs.SetInt($"Map_{map.Key}_Clear", map.Value ? 1 : 0);
        }

        // ��ü ���� ������ ���� ���� ����
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null) {
                foreach (var item in mapItems.Items) {
                    PlayerPrefs.SetInt($"Map_{mapItems.MapID}_Item_{item.ItemID}_Have", item.isHave ? 1 : 0);
                }
            }
        }

        PlayerPrefs.Save();
    }

    public void LoadCurrentMapData() {
        CurrentMapID = PlayerPrefs.GetInt("CurrentMapID", 0);

        // ��ü �� Ŭ���� ���� �ҷ�����
        foreach (var map in MapClearStatus.Keys.ToList()) {
            MapClearStatus[map] = PlayerPrefs.GetInt($"Map_{map}_Clear", 0) == 1;
        }

        // ��ü ���� ������ ���� ���� �ҷ����� �� ��Ȱ��ȭ ó��
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null) {
                // �� Ŭ���� ���� �ݿ�
                mapItems.IsCleared = MapClearStatus.ContainsKey(mapItems.MapID) && MapClearStatus[mapItems.MapID];

                // ������ ���� ���� �ݿ� �� ��Ȱ��ȭ
                foreach (var item in mapItems.Items) {
                    int haveValue = PlayerPrefs.GetInt($"Map_{mapItems.MapID}_Item_{item.ItemID}_Have", 0);
                    item.isHave = haveValue == 1;
                    if (item.isHave) {
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void Update() {
        // ���� �������� ������ ���� ���� �迭�� ������Ʈ
        UpdateItemOwnershipArray();
    }

    private void UpdateItemOwnershipArray() {
        if (CurrentMapID == -1)
            return;

        // ���� ���� ������ ������ �������� ���� MapItems�� ����
        MapItems currentMap = null;

        // ItemDataManager�� �ڽ� ������Ʈ �� MapItems�� ã��
        foreach (Transform child in transform) {
            MapItems mapItems = child.GetComponent<MapItems>();
            if (mapItems != null && mapItems.MapID == CurrentMapID) {
                currentMap = mapItems;
                break;
            }
        }

        // MapItems�� ���ų� MapID�� ��ġ���� ������ ����
        if (currentMap == null) {
            Debug.LogWarning($"[ItemDataManager] MapID {CurrentMapID}�� �ش��ϴ� MapItems�� ã�� �� �����ϴ�.");
            return;
        }

        // ������ ���� ������Ʈ
        int itemCountInMap = currentMap.Items.Count; // List������ Count�� ���
        itemCount = 0;

        // ������ ������ �°� bools �迭 �ʱ�ȭ
        if (bools == null || bools.Length != itemCountInMap) {
            bools = new bool[itemCountInMap];
        }

        // isHave�� true�� ��� bools �迭�� itemCount ������Ʈ
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
