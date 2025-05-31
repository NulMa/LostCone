using System.Collections.Generic;
using UnityEngine;

public class MapItems : MonoBehaviour {
    public int MapID; // �� ��ȣ
    public bool IsCleared; // Ŭ���� ����
    public List<ItemGet> Items; // �ش� ���� ������ ����Ʈ
    public int haveCount; // ������ ������ ����

    public void SaveMapData() {
        PlayerPrefs.SetInt($"Map_{MapID}_Clear", IsCleared ? 1 : 0);

        foreach (var item in Items) {
            PlayerPrefs.SetInt($"Map_{MapID}_Item_{item.ItemID}_Have", item.isHave ? 1 : 0);
        }

        PlayerPrefs.Save();
        DebugMapData();
    }

    public void LoadMapData() {
        // �� Ŭ���� ���� �ε�
        IsCleared = PlayerPrefs.GetInt($"Map_{MapID}_Clear", 0) == 1;

        // ������ ���� ���� �ε� �� ��Ȱ��ȭ
        foreach (var item in Items) {
            int haveValue = PlayerPrefs.GetInt($"Map_{MapID}_Item_{item.ItemID}_Have", 0);
            Debug.Log($"[�����] MapID:{MapID}, ItemID:{item.ItemID}, PlayerPrefs:{haveValue}");
            item.isHave = haveValue == 1;
            if (item.isHave) {
                Debug.Log($"[�����] ��Ȱ��ȭ��: MapID:{MapID}, ItemID:{item.ItemID}");
                item.gameObject.SetActive(false);
            }
        }
    }
    public void DebugMapData() {
        // �� ID�� Ŭ���� ���� ���
        Debug.Log($"[Map Debug] Map ID: {MapID}, Cleared: {IsCleared}");

        // ������ ���� ���� ���
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