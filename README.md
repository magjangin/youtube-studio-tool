# YouTube Studio Custom (YouTube Studio Tool)

<p align="center">
  <img src="youtube%20studio%20custom/Assets/icon.png" width="128" height="128" alt="YouTube Studio Custom Icon" />
</p>

<p align="center">
  <strong>유튜브 크리에이터의 멘탈 케어와 집중을 위한 맞춤형 데스크톱 뷰어</strong><br>
  YouTube Studio의 대시보드 및 분석 화면에서 <em>실시간 구독자 수</em>를 완벽하게 가려줍니다.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Avalonia-12.0-8A2BE2?logo=avalonia" alt="Avalonia UI" />
  <img src="https://img.shields.io/badge/Platform-Windows%20x64-blue?logo=windows" alt="Platform" />
</p>

---

## 💡 개발 배경

유튜브 채널을 운영하다 보면 실시간 구독자 수의 등락에 수시로 시선이 빼앗겨 스트레스를 받거나 영상 기획과 제작 집중력이 흐트러지기 쉽습니다.

**YouTube Studio Custom**은 공식 웹 YouTube Studio(`studio.youtube.com`)의 모든 관리·업로드·통계 기능은 그대로 이용하면서, 사용자의 시선에 불필요한 자극을 주는 **"현재 구독자 수" 수치만 정확히 화면에서 은폐**하여 크리에이터 본연의 작업에 몰입할 수 있도록 돕습니다.

---

## ✨ 핵심 기능

- ⚡ **깜빡임 없는(Zero-Flicker) 완전 은폐**
  - 일반적인 브라우저 확장이나 렌더링 후 주입 방식과 달리, WebView2의 `AddScriptToExecuteOnDocumentCreatedAsync`를 활용하여 **HTML이 파싱되기도 전에** 숨김 스타일을 사전 주입합니다. 첫 로딩이나 새로고침 시 숫자가 1프레임이라도 노출되지 않습니다.
- 🔄 **SPA(Single Page Application) 실시간 추적**
  - YouTube Studio의 내부 페이지 이동(동적 렌더링)에도 은폐 상태가 깨지지 않도록 `MutationObserver`가 DOM 트리를 실시간으로 감시하고 스타일을 유지합니다.
- 🔑 **로그인 세션 영구 보존**
  - 앱 전용 로컬 프로필 디렉터리(`%LocalAppData%\YoutubeStudioCustom\WebView2`)를 사용하여 구글/유튜브 계정 로그인을 영구적으로 보존합니다.
- 🛠️ **DevTools(F12) 내장 지원**
  - YouTube의 DOM 구조 변경이나 새로운 선택자 대응을 위해 개발자 도구 및 마우스 우클릭 검사 기능이 기본 활성화되어 있습니다.

---

## 🛠️ 기술 스택

- **프레임워크**: [.NET 10](https://dotnet.microsoft.com/)
- **UI 툴킷**: [Avalonia UI 12.0](https://avaloniaui.net/)
- **웹 런타임**: [Microsoft.Web.WebView2](https://developer.microsoft.com/ko-kr/microsoft-edge/webview2/) (Chromium 기반)
- **대상 OS**: Windows 10 / 11 (64-bit)

---

## 🚀 시작하기

### 사전 요구 사항
- Windows 10 / 11 (x64)
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) 이상
- Microsoft Edge WebView2 Runtime (최신 윈도우 환경에 기본 탑재)

### 빌드 및 실행

1. **저장소 복제**
   ```bash
   git clone https://github.com/magjangin/youtube-studio-tool.git
   cd youtube-studio-tool
   ```

2. **디버그 실행**
   ```bash
   dotnet run --project "youtube studio custom/youtube studio custom.csproj"
   ```

3. **단일 실행 파일(Single-File) 배포 빌드**
   ```bash
   dotnet publish "youtube studio custom/youtube studio custom.csproj" \
     -c Release \
     -r win-x64 \
     --self-contained true \
     -p:PublishSingleFile=true \
     -p:IncludeNativeLibrariesForSelfExtract=true \
     -p:DebugType=none \
     -o "publish/win-x64"
   ```

---

## 📚 상세 문서

프로젝트의 내부 아키텍처와 트러블슈팅 기록은 `docs` 디렉토리에서 확인하실 수 있습니다:

- [📖 문서 목차 및 개요 (docs/README.md)](docs/README.md)
- [🏗️ 아키텍처 및 라이프사이클 흐름도 (docs/architecture.md)](docs/architecture.md)
- [📦 빌드 및 배포 가이드 (docs/build-and-deployment.md)](docs/build-and-deployment.md)
- [🐛 트러블슈팅: 깜빡임 버그 수정 일지 (docs/bugfixes/2026-08-06-flicker-bug.md)](docs/bugfixes/2026-08-06-flicker-bug.md)

---

## 📜 라이선스

이 프로젝트는 개인 및 비상업적 용도로 자유롭게 사용, 수정 및 배포할 수 있습니다.
