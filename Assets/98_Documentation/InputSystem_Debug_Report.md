# Unity Input System 디버깅 리포트

**날짜**: 2025년 12월 30일  
**프로젝트**: LostCone  
**문제**: 플레이어 키보드 입력이 간헐적으로 작동하지 않음

---

## 📋 증상

- 이동, 점프, 대시 등 **모든 키보드 입력이 간헐적으로 작동 중단**
- 코드나 씬(DefaultScene2)을 수정한 후 발생
- 수정 내용을 되돌려도 문제가 해결되지 않음
- 동일한 Git 커밋에서도 작동할 때가 있고 안 될 때가 있음

---

## ❌ 시도한 방법들 (실패)

### 1. Action Map 명시적 활성화
```csharp
// Start()에서 Player, UI 맵 강제 활성화
var pMap = playerInput.actions.FindActionMap("Player");
var uMap = playerInput.actions.FindActionMap("UI");
if(pMap != null) pMap.Enable();
if(uMap != null) uMap.Enable();
```
**결과**: 실패 - 문제 지속

### 2. Update()에서 Action Map 상태 체크
```csharp
// Update()에서 Player 맵이 비활성화되면 재활성화
private void Update()
{
    var pMap = playerInput.actions.FindActionMap("Player");
    if(pMap != null && !pMap.enabled) pMap.Enable();
}
```
**결과**: 실패 - 문제 지속

### 3. Polling 방식으로 전환
```csharp
// Send Messages 대신 직접 ReadValue 사용
MoveInput = playerInput.actions["Move"].ReadValue<Vector2>();
```
**결과**: 실패 - 문제 지속

### 4. OnSceneLoaded 핸들러 추가
```csharp
// 씬 로드 시 PlayerInput 재초기화
SceneManager.sceneLoaded += OnSceneLoaded;
```
**결과**: 실패 - 문제 지속

### 5. Domain Reload 대응
```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
private static void ResetStatics()
{
    instance = null;
}
```
**결과**: 실패 - 문제 지속

### 6. Library 캐시 삭제
```
- Unity 종료
- Library 폴더 삭제
- Unity 재실행
```
**결과**: 실패 - 문제 지속

### 7. Git에서 작동하던 커밋으로 복원
```bash
git checkout eac51d0
```
**결과**: 실패 - 동일 코드인데도 간헐적으로 작동/미작동

---

## ✅ 해결 방법 (성공)

### 원인 발견
**플레이 모드에서 Inspector 확인** 결과:
- `Control Scheme: Touch`
- `Devices: Touchscreen/Touchscreen`

→ **Unity Input System이 키보드가 아닌 터치스크린을 입력 장치로 인식**

### 원인 분석
1. PlayerInput 컴포넌트의 **Auto-Switch**가 활성화되어 있음
2. 코드/씬 수정 시 **Domain Reload** 발생
3. PlayerInput 초기화 후 Input System이 장치 재스캔
4. **터치스크린이 먼저 감지**되어 Control Scheme이 "Touch"로 설정됨
5. Touch는 키보드 입력을 받지 않으므로 모든 키 입력 무시됨

### 해결 코드
```csharp
private void Start()
{
    if(playerInput != null)
    {
        // Control Scheme을 Keyboard&Mouse로 강제 설정
        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        
        // Action Map 활성화
        if(playerInput.actions != null)
        {
            var pMap = playerInput.actions.FindActionMap("Player");
            var uMap = playerInput.actions.FindActionMap("UI");
            if(pMap != null) pMap.Enable();
            if(uMap != null) uMap.Enable();
        }
    }
}
```

### 추가 권장 설정 (Prefab)
`_InputManager` 프리팹에서:
- **Default Scheme**: `<Any>` → `Keyboard&Mouse`
- **Auto-Switch**: ✓ → ☐ (체크 해제)

---

## 📝 교훈

1. **Inspector 런타임 상태 확인의 중요성**
   - 코드만 보지 말고, 플레이 모드에서 컴포넌트 상태 확인 필수
   
2. **Unity Input System Auto-Switch 주의**
   - 터치스크린이 있는 PC에서는 예상치 못한 장치 전환 발생 가능
   
3. **Domain Reload 후 상태 초기화**
   - 스크립트 재컴파일 시 Input System 상태가 리셋됨

4. **간헐적 버그 = 코드 외적 요인 의심**
   - 동일 코드가 때때로 작동/미작동이면 환경 문제일 가능성 높음

---

## 🔧 관련 파일

- `Assets/01_Scripts/Player/InputManager.cs`
- `Assets/02_Prefabs/DontDestroy/_InputManager.prefab`
- `Assets/InputSystem/Player.inputactions`

---

*이 문서는 향후 유사 문제 발생 시 참고용으로 작성됨*
