using UnityEngine;
using System;
using System.Reflection;

public enum FollowMode { Disabled, Fixed, FollowUV }

[System.Serializable]
public struct StagePreset {
    public int mapNumber;
    public FollowMode mode;             // Disabled, Fixed, FollowUV
    [Header("Per-Stage Assignments")]
    public MeshRenderer target;         // 이 스테이지에서 사용할 타겟 Quad
    public Transform follower;          // 이 스테이지에서 따라갈 오브젝트(라이트)

    [Header("FollowUV Settings")] 
    [Range(0f,1f)] public float featureU; // 텍스처 내 태양의 U 위치
    [Range(0f,1f)] public float featureV; // 필요 시 세로 위치
    public bool followX;                 // X 축 추적 여부
    public bool followY;                 // Y 축 추적 여부
    public float zOffset;                // Z 보정

    [Header("Fixed Mode")]
    public Vector3 fixedWorldPosition;   // Fixed 모드일 때의 위치
}

// Quad 머티리얼의 오프셋/타일링 기준으로 UV 위치를 월드로 매핑하여 follower를 이동
// 모든 설정은 StagePreset에서 관리하고, 런타임에는 내부 상태(cur*)만 사용
public class FollowUVLight : MonoBehaviour {
    [Header("Stage Presets")]
    public StagePreset[] stagePresets;            // 스테이지별 설정
    [Tooltip("시작 시 GameManager에서 맵 번호를 가져와 프리셋 자동 적용")]
    public bool autoApplyOnStart = true;

    [Header("Runtime State")]
    public int currentMapNumber = -1; // 현재 적용된 맵 번호(외부 조회용)
    public bool liveSync = true;      // 런타임에 맵 번호 변화를 감지해 실시간 적용
    public float liveSyncInterval = 0.25f; // 체크 주기(초)

    // 내부 적용 상태
    MeshRenderer curTarget;
    Transform curFollower;
    float curFeatureU = 0.5f;
    float curFeatureV = 0.5f;
    bool curFollowX = true;
    bool curFollowY = false;
    float curZOffset = 0f;
    FollowMode curMode = FollowMode.Disabled;
    Vector3 fixedWorldPositionCache;      // Fixed 모드용 캐시
    float _nextSyncTime;

    // 매니저 캐시(리플렉션 비용 절약)
    Type _cachedManagerType;
    UnityEngine.Object _cachedManagerObj;
    FieldInfo _cachedFieldInfo;
    PropertyInfo _cachedPropInfo;

    void Reset() {
        curFollower = transform; // 기본은 자기 자신 이동
        curTarget = GetComponent<MeshRenderer>();
    }

    void Start() {
        if (autoApplyOnStart) {
            ApplyPresetFromManager();
        }
    }

    void Update() {
        if (!liveSync) return;
        if (Time.unscaledTime < _nextSyncTime) return;
        _nextSyncTime = Time.unscaledTime + Mathf.Max(0.02f, liveSyncInterval);

        int map;
        if (TryGetMapNumberCached(out map) || TryGetMapNumber(out map)) {
            if (map != currentMapNumber) {
                ApplyStagePreset(map);
            }
        }
    }

    void LateUpdate() {
        if (curTarget == null || curFollower == null) return;

        if (curMode == FollowMode.Disabled) {

            return;
        }

        if (curMode == FollowMode.Fixed) {
            Vector3 p = fixedWorldPositionCache;
            p.z += curZOffset;
            curFollower.position = p;
            return;
        }

        // 머티리얼 인스턴스의 오프셋/타일링을 읽음(스크롤 코드와 동일 참조 필요 시 material 사용)
        var mat = curTarget.material;
        Vector2 tiling = mat.mainTextureScale; if (Mathf.Approximately(tiling.x, 0f)) tiling.x = 1f; if (Mathf.Approximately(tiling.y, 0f)) tiling.y = 1f;
        Vector2 offset = mat.mainTextureOffset;

        // UV -> Quad 로컬 정규 좌표(0~1) 역변환 (Repeat 고려)
        float xNorm = Mathf.Repeat((curFeatureU - offset.x) / tiling.x, 1f);
        float yNorm = Mathf.Repeat((curFeatureV - offset.y) / tiling.y, 1f);

        // 월드 기준 폭/높이와 로컬 축
        Vector3 center = curTarget.bounds.center;
        float width = curTarget.bounds.size.x;
        float height = curTarget.bounds.size.y;
        Vector3 right = curTarget.transform.right; // Quad의 가로 방향(월드)
        Vector3 up = curTarget.transform.up;       // Quad의 세로 방향(월드)

        // 중심(0.5,0.5) 기준 오프셋 계산
        Vector3 worldPos = center;
        if (curFollowX) worldPos += right * ((xNorm - 0.5f) * width);
        if (curFollowY) worldPos += up    * ((yNorm - 0.5f) * height);
        worldPos += curTarget.transform.forward * 0f; // 필요 시 평면 방향 보정
        worldPos.z += curZOffset;

        // X만 추적하고 Y는 유지하고 싶다면 여기서 보정
        Vector3 finalPos = worldPos;
        if (!curFollowY) finalPos.y = curFollower.position.y;
        if (!curFollowX) finalPos.x = curFollower.position.x;

        curFollower.position = finalPos;
    }

    // 맵 번호에 따라 프리셋을 적용(스위치)
    public void ApplyStagePreset(int mapNumber) {
        // 외부에서 조회할 수 있도록 먼저 기록
        currentMapNumber = mapNumber;
        // 기본: 비활성
        bool found = false;
        for (int i = 0; i < (stagePresets?.Length ?? 0); i++) {
            if (stagePresets[i].mapNumber != mapNumber) continue;
            var p = stagePresets[i];
            found = true;

            // 타겟/팔로워를 먼저 설정(Disabled여도 끌 수 있도록)
            curTarget = p.target != null ? p.target : GetComponent<MeshRenderer>();
            curFollower = p.follower != null ? p.follower : transform;

            // 모드 적용 후 활성/비활성 토글
            curMode = p.mode;
            UpdateFollowerActive();
            if (curMode == FollowMode.Disabled) {
                return;
            }

            // 모드별 데이터
            if (curMode == FollowMode.Fixed) {
                fixedWorldPositionCache = p.fixedWorldPosition;
            }
            // FollowUV 파라미터
            curFeatureU = p.featureU;
            curFeatureV = p.featureV;
            curFollowX = p.followX;
            curFollowY = p.followY;
            curZOffset = p.zOffset;

            return;
        }

        if (!found) {
            // 프리셋이 없으면 팔로워만 비활성화
            curMode = FollowMode.Disabled;
            UpdateFollowerActive();
        }
    }

    // GameManager/GamaManager/MapNumbers에서 맵 번호를 읽어 프리셋을 적용
    public bool ApplyPresetFromManager() {
        int map;
        if (TryGetMapNumber(out map)) {
            currentMapNumber = map;
            ApplyStagePreset(map);
            return true;
        }
    Debug.LogWarning("[FollowUVLight] 맵 번호를 찾지 못해 비활성화합니다.");
    curMode = FollowMode.Disabled;
    UpdateFollowerActive();
        return false;
    }

    // 모드 상태에 따라 follower의 활성/비활성 전환
    void UpdateFollowerActive() {
    if (curFollower == null) curFollower = transform; // 폴백 보장
        bool on = (curMode != FollowMode.Disabled);
        if (curFollower.gameObject.activeSelf != on) curFollower.gameObject.SetActive(on);
    }

    // 다양한 매니저 후보에서 맵 번호를 가져오기(리플렉션)
    bool TryGetMapNumber(out int map) {
        // 1) 타입 이름 후보들
        string[] typeNames = new string[] { "GameManager", "GamaManager", "MapNumbers" };
        // 2) 필드/프로퍼티 이름 후보들
        string[] memberNames = new string[] {
            "currentStageID", "CurrentStageID",
            "mapNumber", "MapNumber",
            "currentMap", "CurrentMap",
            "stage", "Stage",
            "currentStage", "CurrentStage",
            "level", "Level"
        };

        foreach (var tn in typeNames) {
            var t = FindTypeInAssemblies(tn);
            if (t == null) continue;

            var obj = UnityEngine.Object.FindFirstObjectByType(t);
            if (obj == null) continue;

            foreach (var name in memberNames) {
                // 프로퍼티 우선
                var prop = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && (prop.PropertyType == typeof(int) || prop.PropertyType.IsEnum)) {
                    try {
                        object val = prop.GetValue(obj);
                        if (val is int iv) { map = iv; return true; }
                        if (prop.PropertyType.IsEnum) { map = (int)val; return true; }
                    } catch { }
                }
                // 필드
                var field = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(int) || field.FieldType.IsEnum)) {
                    try {
                        object val = field.GetValue(obj);
                        if (val is int iv) { map = iv; return true; }
                        if (field.FieldType.IsEnum) { map = (int)val; return true; }
                    } catch { }
                }
            }
        }

        // 3) PlayerPrefs 폴백
        if (PlayerPrefs.HasKey("MapNumber")) {
            map = PlayerPrefs.GetInt("MapNumber");
            return true;
        }

        map = 0;
        return false;
    }

    // 캐시된 매니저에서 빠르게 읽기 시도(우선 GamaManager.currentStageID)
    bool TryGetMapNumberCached(out int map) {
        map = 0;
        if (_cachedManagerObj == null || (_cachedFieldInfo == null && _cachedPropInfo == null)) {
            TryCacheManagerFastPath();
        }
        if (_cachedManagerObj != null) {
            try {
                if (_cachedFieldInfo != null) {
                    object v = _cachedFieldInfo.GetValue(_cachedManagerObj);
                    if (v is int iv1) { map = iv1; return true; }
                }
                if (_cachedPropInfo != null) {
                    object v = _cachedPropInfo.GetValue(_cachedManagerObj);
                    if (v is int iv2) { map = iv2; return true; }
                }
            } catch { }
        }
        return false;
    }

    void TryCacheManagerFastPath() {
        // 우선순위: GamaManager.currentStageID
        _cachedManagerType = FindTypeInAssemblies("GamaManager");
        if (_cachedManagerType != null) {
            _cachedManagerObj = UnityEngine.Object.FindFirstObjectByType(_cachedManagerType);
            if (_cachedManagerObj != null) {
                _cachedFieldInfo = _cachedManagerType.GetField("currentStageID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (_cachedFieldInfo == null) {
                    _cachedPropInfo = _cachedManagerType.GetProperty("currentStageID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
            }
        }
    }

    // 로드된 어셈블리에서 타입 이름으로 찾기
    static Type FindTypeInAssemblies(string typeName) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in assemblies) {
            try {
                var t = asm.GetType(typeName, throwOnError: false);
                if (t != null) return t;
            } catch { }
        }
        return null;
    }
}
