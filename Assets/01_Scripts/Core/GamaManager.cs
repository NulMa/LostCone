using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Blade.SoundSystem;
using Core;
using PaperFlower.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro; // For TextMeshProUGUI


[DefaultExecutionOrder(-5)]
public class GamaManager : MonoBehaviour
{
    public static GamaManager Instance { get; private set; }

    public GameObject[] BGs;
    public GameObject[] Maps;

    public SceneManager SceneManager;
    public ItemDataManager ItemDataManager;
    public UICtrl UIManager; // Reference to UICtrl
    public AchiveManager AchiveManager;

    // [설정 필요] Player 오브젝트를 Inspector에서 연결해주세요.
    public Player player;
    public GameObject playerParent;
    public GameObject followerNeko;
    public GameObject playerCone;
    public MapItems currentMap;
    public Image panel;
    public Camera mainCam;

    public GameObject rain;

    public bool ignoreSavePos;

    public int langue; // 언어 인덱스
    public int currentStageID;

    public bool isLastSceneCleared = false;
    
    public SoundSO[] bgm;
    public SoundSO[] ambients;

    
    private readonly PlaySoundEvent _playSoundEvent = new PlaySoundEvent();
    private readonly StopSoundEvent _stopSoundEvent = new StopSoundEvent();
    private int _bgmChannel = 100;
    private int _asChannel = 100;

    private void Awake()
    {
        isLastSceneCleared = PlayerPrefs.GetInt("achive_LostCone") == 1 ? true : false;
        if (!isLastSceneCleared)
        {

            followerNeko.SetActive(false);
        }
        else
        {
            followerNeko.SetActive(true);
            playerCone.SetActive(true);
        }

        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 제거
        }
        
        if (SaveManager.Instance.HasPath("playerPos"))
            LoadGame();

        if (GamaManager.Instance.SceneManager.scenes[1].isDone) rain.SetActive(false);

        // 현재 스테이지에 따른 BGM 재생
        PlayBGMForCurrentStage();
    }

    private void Start()
    {

    }

    private void Update()
    {
        ItemDataManager.CurrentMapID = currentStageID;

        if(currentStageID != 0)
            rain.SetActive(false);

        if (ItemDataManager.Instance != null && ItemDataManager.Instance.itemCount != 1)
        {
            if (UIManager != null)
                UIManager.UpdateItemCount(ItemDataManager.Instance.itemCount, ItemDataManager.Instance.bools.Length);
        }
    }




    public void achiveCall(string key)
    {
        //if key is not exist, return

        if (PlayerPrefs.GetInt("achive_" + key, 0) == 1)
            return;

        if (AchiveManager.instance == null)
        {
            Debug.LogWarning($"[AchiveManager] Instance is null! Cannot update achievement: {key}");
            return;
        }

        AchiveManager.instance.SetAchiveClear(key);
    }

    public void rainSwitch()
    {
        if (rain.activeSelf)
        {
            rain.SetActive(false);
        }
        else
        {
            rain.SetActive(true);
        }
    }

    public void passFirstScene()
    {
        Debug.Log("첫 번째 넘기도록 : " + SceneManager.scenes[0].isDone);
        DOTween.Kill(SceneManager.scenes[0].scene);

        player.isScenePlaying = false;
        panel.gameObject.SetActive(false);
        SceneManager.scenes[0].scene.SetActive(false);
        mainCam.orthographicSize = 7.5f;
        BGOn(currentStageID);
        Maps[0].GetComponent<MapNumbers>().OnOffAll();

        //UIManager.Message.gameObject.SetActive(true);
        //UIManager.Message.DOColor(Color.black, 1f);

        UIManager.Stage.gameObject.SetActive(true);
        UIManager.Items.gameObject.SetActive(true);
        Color color = new Color(0x63 / 255f, 0x63 / 255f, 0x63 / 255f);
        UIManager.Stage.DOColor(color, 1f);
        UIManager.Items.DOColor(color, 1f);

        foreach (Transform child in player.transform)
        {
            // 자식 오브젝트의 태그가 "Remove"인지 확인
            if (child.CompareTag("Remove"))
            {
                // 해당 오브젝트 삭제
                Destroy(child.gameObject);
            }
        }

        if (currentStageID == 0 && !SceneManager.scenes[1].isDone)
        {
            rainSwitch();
        }
        currentMap.gameObject.SetActive(true);
    }

    public void SaveGame()
    {
        if (!ignoreSavePos)
            player.SavePlayerPosition();
        
        ItemDataManager.Instance.SaveCurrentMapData();
        currentMap.SaveMapData();
        SceneManager.SaveSceneData();
        Debug.Log("게임 데이터가 저장되었습니다.");
    }

    public void LoadGame()
    {
        ItemDataManager.Instance.LoadCurrentMapData();
        SceneManager.LoadSceneData();
        currentMap.LoadMapData();
        passFirstScene();

        if (!ignoreSavePos)
            player.LoadPlayerPosition();

        Debug.Log("게임 데이터가 로드되었습니다.");
    }

    // [수정됨] 아이템 획득 시의 로직 전체
    public void OnItemCollected(Sprite collectedItemSprite) // [수정] Sprite를 인자로 받도록 변경
    {
        Debug.Log("[GamaManager] OnItemCollected() called.");
        // 데이터 저장 및 업데이트
        ItemDataManager.Instance.SaveCurrentMapData();
        ItemDataManager.Instance.UpdateItemOwnershipArray();
        SaveGame();

        // 첫 번째 아이템인지 확인
        if (ItemDataManager.Instance.itemCount == 1)
        {
            Debug.Log("[GamaManager] First item collected. Triggering NEW UI animation sequence.");
            // [핵심 변경] UIManager의 새로운 연출 함수를 플레이어 위치와 함께 호출
            if (player != null && UIManager != null)
            {
                // [수정] 획득한 아이템의 스프라이트를 함께 전달
                UIManager.PlayItemAcquireAnimation(player.transform.position, collectedItemSprite);
            }
            else
            {
                Debug.LogWarning("GamaManager: Player 또는 UIManager 참조가 없습니다!");
            }
        }
        // 첫 아이템이 아닐 경우, UI 텍스트만 조용히 업데이트
        else
        {
            if (UIManager != null)
            {
                UIManager.UpdateItemCount(ItemDataManager.Instance.itemCount, ItemDataManager.Instance.bools.Length);
            }
        }
    }

    public void BGOn(int number)
    {
        if (BGs == null || BGs.Length == 0)
        {
            Debug.LogError("BGs 배열이 초기화되지 않았습니다. Unity Inspector에서 BGs 배열을 설정하세요.");
            return;
        }

        if (number < 0 || number >= BGs.Length)
        {
            Debug.LogError($"잘못된 인덱스: {number}. BGs 배열의 범위를 확인하세요.");
            return;
        }

        foreach (var chooseAll in BGs)
        {
            if (chooseAll != null && chooseAll.gameObject.activeSelf)
            {
                chooseAll.gameObject.SetActive(false);
            }
        }

        BGs[number].gameObject.SetActive(true);

        // 배경 변경 시 해당 스테이지 BGM 재생
        currentStageID = number;
        PlayBGMForCurrentStage();

        if (ItemDataManager.Instance != null && ItemDataManager.Instance.itemCount != 1)
        {
            if (UIManager != null)
                UIManager.UpdateItemCount(ItemDataManager.Instance.itemCount, ItemDataManager.Instance.bools.Length);
        }
    }

    // 현재 스테이지에 따른 BGM 재생
    public void PlayBGMForCurrentStage()
    {
        GameEventBus.RaiseEvent(_stopSoundEvent.Initialize(_bgmChannel));
        GameEventBus.RaiseEvent(_stopSoundEvent.Initialize(_asChannel));
        _bgmChannel = bgm[currentStageID].GetInstanceID();
        _asChannel = ambients[currentStageID].GetInstanceID();
        GameEventBus.RaiseEvent(_playSoundEvent.Initialize(bgm[currentStageID], _bgmChannel));
        GameEventBus.RaiseEvent(_playSoundEvent.Initialize(ambients[currentStageID], _asChannel));
    }

    // AudioManager SFX 실행 함수들
    public void PlayUISFX(int index){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayUISFX(index);
        }
    }

    public void PlayUISFX(int index, float pitch){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayUISFX(index, pitch);
        }
    }

    public void PlayUISFX(string clipName){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayUISFX(clipName);
        }
    }

    public void PlayUISFX(string clipName, float pitch){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayUISFX(clipName, pitch);
        }
    }

    public void PlaySFXAt(int index, Vector3 position){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlaySFXAt(index, position);
        }
    }

    public void PlaySFXAt(int index, Vector3 position, float pitch){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlaySFXAt(index, position, pitch);
        }
    }

    public void PlaySFXAt(string clipName, Vector3 position){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlaySFXAt(clipName, position);
        }
    }

    public void PlaySFXAt(string clipName, Vector3 position, float pitch){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlaySFXAt(clipName, position, pitch);
        }
    }

    public void PlayLoopSFX(int index, GameObject owner){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayLoopSFX(index, owner);
        }
    }

    public void PlayLoopSFX(int index, GameObject owner, float pitch){
        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayLoopSFX(index, owner, pitch);
        }
    }

    public void StopLoopSFX(GameObject owner){
        if (AudioManager.Instance != null){
            AudioManager.Instance.StopLoopSFX(owner);
        }
    }


}
