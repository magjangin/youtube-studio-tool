# youtube studio custom — 인수인계 문서 (AGENTS.md)

> 작성일 2026-11-15. 폴더의 코드·프로젝트 파일·기존 문서와 실제 게임 설치 폴더(Steam `.acf`, `UnityPlayer.dll`, `MelonLoader.dll`)를 읽어 정리했다.
> 이 문서 작성 시점에 인게임 재검증은 하지 않았다. **"실측"** 표시는 디스크에서 직접 읽은 값, **"미확인"**은 다음 세션에서 확인할 항목이다.

## 0. 프로젝트 개요

- **무엇인가:** YouTube Studio 커스텀 뷰어(Avalonia WebView, net10). 구독자 수 숨김 스크립트(`Scripts/hide-subscribers.js`)를 주입. 문서 `docs/`(깜빡임 버그 수정 기록 포함).
- **종류:** 게임 모드가 아닌 앱/도구
- **최근 수정일(파일 기준):** 2026-11-13
- **git 원격:** `H:\local-repos\youtube`
- **최근 커밋:**
  - `acdfff3 Hide redesigned 'Total subscribers' card in YouTube Studio Analytics`
  - `f05ec8d Add GitHub Sponsors funding configuration`
  - `812bf20 Initial commit: YouTube Studio Custom project and documentation`
- **먼저 읽을 기존 문서:**
  - [README.md](README.md)
  - [docs\README.md](docs/README.md)
  - [docs\architecture.md](docs/architecture.md)
  - [docs\bugfixes\2026-08-06-flicker-bug.md](docs/bugfixes/2026-08-06-flicker-bug.md)
  - [docs\build-and-deployment.md](docs/build-and-deployment.md)

## 1. 게임 버전 및 고정 여부

- 게임 모드가 아니므로 해당 없음.

## 2. 런타임 백엔드 및 디컴파일 경로

- 게임 모드가 아님. 런타임/언어: .NET (`net10.0`)
- 모드/앱 프로젝트 `youtube studio custom\youtube studio custom.csproj` 타깃 프레임워크: `net10.0` (SDK 스타일)

## 3. 설계 의도와 우회 사유 (Rationale)

코드 주석과 기존 문서에서 작성자가 남긴 의도·주의 문장을 그대로 옮겼다(파일:줄).

- `youtube studio custom\YoutubeStudioWebView.cs:27` — cookies survive across app restarts instead of using a throwaway profile.
- **(2026-11-18 추가, 실측)** 고급 모드 > 실시간 실적(`explore_type=LATEST_ACTIVITY`) 표의 맨 위 **합계 행 미니 막대그래프만** 숨긴다. 영상 행 그래프 바로 위에 붙어 있어 영상이 1개 더 있는 것처럼 헷갈린다는 사용자 요청. 같은 요청으로 합계 행의 **지난 60분 합계 숫자**(`#debug-minutely-total`)도 숨긴다. 지난 48시간 합계(`#debug-hourly-total`)는 그대로 둔다. selector: `yta-explore-latest-activity-row #row-container[total-row] yta-tiny-chart`, `... #row-container[total-row] #debug-minutely-total` (`hide-subscribers.js`의 주입 CSS). 합계 행은 `#row-container`에 붙는 `total-row` 속성으로만 구분된다(텍스트 '합계'에 의존하지 않음).

## 4. 핵심 훅 시그니처와 주입 파이프라인

- **주요 소스 파일(큰 순):**
  - `youtube studio custom\YoutubeStudioWebView.cs`
  - `youtube studio custom\Scripts\hide-subscribers.js`
  - `youtube studio custom\MainWindow.axaml.cs`
  - `youtube studio custom\Program.cs`
  - `youtube studio custom\App.axaml.cs`

## 5. 시도했다가 실패한 것들 (Dead Ends)

코드 주석과 기존 문서에서 실패·크래시·금지 관련 문장을 그대로 옮겼다(파일:줄). 같은 방법을 다시 시도하기 전에 해당 위치를 읽을 것.

- `README.md:91` — [🐛 트러블슈팅: 깜빡임 버그 수정 일지 (docs/bugfixes/2026-08-06-flicker-bug.md)](docs/bugfixes/2026-08-06-flicker-bug.md)
- `docs\README.md:32` — [**트러블슈팅: 구독자 수 깜빡임 버그 수정 일지 (bugfixes/2026-08-06-flicker-bug.md)**](file:///h:/source/repos/youtube%20studio%20custom/docs/bugfixes/2026-08-06-flicker-bug.md)
- `docs\architecture.md:64` — 네비게이션이 완료될 때마다 `InvokeScript`로 스크립트를 한 번 더 실행합니다. 이는 예외적인 환경에서 문서 생성 시점 등록이 누락될 경우를 대비한 2차 안전망입니다.
- `docs\architecture.md:76` — 플랫폼 핸들 획득 실패 등 특수한 예외 상황이 발생했을 때를 대비한 백업 안전망으로 유지됩니다.
- `docs\bugfixes\2026-08-06-flicker-bug.md:1` — : 구독자 수 깜빡임(FOUC) 버그 수정 일지
- `docs\bugfixes\2026-08-06-flicker-bug.md:19` — 문제**: 기존 코드는 `NavigationCompleted`(페이지 로딩이 끝난 시점) 이벤트에서만 `InvokeScript`로 숨김 스크립트를 주입했습니다. 이 시점은 브라우저 엔진이 이미 최소 한 프레임 이상 화면을 렌더링한 직후이므로, 그 사이에 구독자 수가 노출되었습니다.
- `docs\bugfixes\2026-08-06-flicker-bug.md:24` — 문제**: `AddScriptToExecuteOnDocumentCreatedAsync`는 WebView2 브라우저 프로세스로 요청을 보내는 비동기 호출입니다. 기존 XAML에는 `Source="https://studio.youtube.com"`이 정적으로 선언되어 있어, `AdapterCreated` 이벤트가 발생하는 즉시 첫 페이지 로딩이 시작되었습니다. 등록 요청이 완료되기 전에 첫 문서
- `docs\bugfixes\2026-08-06-flicker-bug.md:27` — 원인 3: 극초기 시점 스크립트 실행 시의 예외
- `youtube studio custom\Program.cs:8` — Initialization code. Don't use any Avalonia, third-party APIs or any
- `youtube studio custom\Program.cs:15` — Avalonia configuration, don't remove; also used by visual designer.

## 6. 빌드 명령 및 배포 경로

- 솔루션: `youtube studio custom.slnx`
- `youtube studio custom\youtube studio custom.csproj`:
  ```powershell
  dotnet build "youtube studio custom\youtube studio custom.csproj" -c Release
  ```

### 실제 DOM 확인 방법 (2026-11-18 실측)

Studio 클래스명은 개편마다 바뀌므로 추측하지 말고 실제 DOM을 읽는다. 앱을 원격 디버깅 포트와 함께 띄우면 로그인된 세션 그대로 CDP로 접근할 수 있다:

```powershell
$env:WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS = "--remote-debugging-port=9333"; Start-Process "youtube studio custom\bin\Release\net10.0\youtube studio custom.exe"
```

`http://127.0.0.1:9333/json`에서 페이지의 `webSocketDebuggerUrl`을 얻어 `Runtime.evaluate`로 DOM을 조회한다(Node 22+ 내장 `WebSocket`으로 충분). 확인 후에는 포트 없이 앱을 다시 띄운다. 참고: 고급 모드에서 `Escape` 키 이벤트를 보내면 고급 모드가 닫혀 개요로 돌아간다. 보고서 선택 드롭다운은 헤더의 '신고'(Reports 오역) `ytcp-dropdown-trigger`다.

## 7. 다음 작업자에게

- 작업 전 기존 문서(0절)부터 읽는다.
- 인게임 동작을 확인하면 3·5절의 **미확인** 항목을 실제 내용으로 바꾼다.
