# 2026-08-06: 구독자 수 깜빡임(FOUC) 버그 수정 일지

앱을 실행하거나 대시보드를 새로고침할 때, 실제 구독자 숫자가 찰나의 순간 노출된 후 사라지는 현상을 디버깅하고 해결한 기술 기록입니다.

---

## 1. 증상

- 앱 최초 실행 시 또는 대시보드 새로고침 시 채널 분석 카드의 "현재 구독자 수" 숫자가 아주 짧게(약 1프레임) 실제 숫자로 화면에 보였다가 사라짐.
- 최종적으로는 정상 가려지지만, 순간적인 깜빡임(FOUC, Flash of Unstyled Content)이 사용자에게 인지됨.

---

## 2. 원인 분석 및 해결책

원인은 세 가지 요소가 복합적으로 작용하고 있었습니다.

### 원인 1: 스크립트 실행 타이밍의 한계
- **문제**: 기존 코드는 `NavigationCompleted`(페이지 로딩이 끝난 시점) 이벤트에서만 `InvokeScript`로 숨김 스크립트를 주입했습니다. 이 시점은 브라우저 엔진이 이미 최소 한 프레임 이상 화면을 렌더링한 직후이므로, 그 사이에 구독자 수가 노출되었습니다.
- **해결책**: WebView2의 `AddScriptToExecuteOnDocumentCreated` API를 도입했습니다. 새 문서가 생성되자마자(HTML이 파싱되기도 전) 스크립트가 실행되므로, 페이지 본문보다 숨김 스타일이 먼저 DOM에 자리잡게 됩니다.
- **구현 세부사항**: Avalonia의 `NativeWebView` 컨트롤은 이 API를 직접 노출하지 않으므로, `TryGetPlatformHandle()`을 통해 원시 `ICoreWebView2` COM 포인터(`IntPtr`)를 꺼낸 뒤 `Microsoft.Web.WebView2.Core.CoreWebView2.CreateFromComICoreWebView2(IntPtr)`로 감싸서 호출했습니다.

### 원인 2: 비동기 등록과 페이지 탐색의 레이스 컨디션
- **문제**: `AddScriptToExecuteOnDocumentCreatedAsync`는 WebView2 브라우저 프로세스로 요청을 보내는 비동기 호출입니다. 기존 XAML에는 `Source="https://studio.youtube.com"`이 정적으로 선언되어 있어, `AdapterCreated` 이벤트가 발생하는 즉시 첫 페이지 로딩이 시작되었습니다. 등록 요청이 완료되기 전에 첫 문서가 생성되면, 정작 최초 실행 시점에 스크립트가 누락되었습니다.
- **해결책**: XAML에서 `Source` 속성을 제거하고, `MainWindow.axaml.cs`의 `WebView_OnAdapterCreated` 핸들러에서 스크립트 등록을 `await`로 완전히 끝마친 뒤 `WebView.Source`를 설정하도록 순서를 강제했습니다.

### 원인 3: 극초기 시점 스크립트 실행 시의 예외
- **문제**: `AddScriptToExecuteOnDocumentCreated`로 호출되는 시점은 `document.head`는 물론 `document.documentElement`(`<html>`)조차 아직 메모리에 생성되지 않은 극초기 단계입니다. 기존 스크립트는 `(document.head || document.documentElement).appendChild(style)`를 즉시 호출하여 `null` 참조 예외가 발생했고, 뒤따르는 `MutationObserver` 초기화까지 모두 중단되었습니다.
- **해결책**:
  1. `target` 대상이 없으면 조용히 리턴하도록 가드 코드를 추가했습니다.
  2. `MutationObserver`가 감시하는 대상을 `document.documentElement`가 아닌 항시 존재하는 `document` 루트 객체로 변경했습니다. 파서가 `<html>` 노드를 추가하는 순간 옵저버 콜백이 격발되어 스타일 태그를 부착합니다.

---

## 3. 디버깅 과정 및 DOM 검증 (참고)

YouTube Studio 대시보드의 구독자 수 요소가 Shadow DOM 내부에 캡슐화되어 있는지 확인하기 위해 개발자 도구(F12) 콘솔에서 실제 구조를 덤프했습니다:

```javascript
copy([...document.querySelectorAll('ytcd-channel-facts-item')]
  .find(el => el.querySelector('h2')?.textContent.includes('채널 분석'))
  ?.outerHTML);
```

### 조사 결과
- 확인 결과 구독자 수는 `<div class="metric-value-big ...">433</div>` 형태의 순수 Light DOM(일반 DOM) 요소였습니다.
- 즉, 기존 CSS 선택자(`.metric-value-big` 등)는 정확했으며, 오직 **주입 타이밍과 라이프사이클 순서**의 문제였음이 확인되었습니다.

---

## 4. 최종 결과

- 앱 최초 실행 및 대시보드 새로고침 시 깜빡임 없이 완벽하게 은폐 상태 유지.
- `publish/win-x64/` 산출물에 수정 사항 반영 및 배포 검증 완료.
