# YouTube Studio Custom

YouTube Studio의 화면에서 구독자 수 표시를 완전히 가려주는 Windows 전용 데스크톱 애플리케이션입니다.

크리에이터가 구독자 수 수치 변화에 따른 심리적 부담을 줄이고 스튜디오 본연의 기능에 집중할 수 있도록 지원합니다.

---

## 주요 특징

- **순수 웹뷰 기반**: Avalonia 12 및 `Avalonia.Controls.WebView`(WebView2)를 기반으로 동작하여 공식 웹 스튜디오의 모든 기능을 동일하게 사용 가능.
- **로그인 세션 유지**: 전용 프로필 디렉터리를 지정하여 앱을 종료하고 다시 켜도 구글/유튜브 계정 로그인이 풀리지 않음.
- **깜빡임 없는(Zero-Flicker) 은폐**: `CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync`를 통해 HTML 파싱 이전에 스타일을 사전 주입하여 1프레임 노출 현상 완벽 차단.
- **SPA 실시간 감시**: YouTube Studio의 클라이언트 사이드 라우팅 및 동적 DOM 렌더링에 대응하기 위해 `MutationObserver` 상시 가동.

---

## 📚 상세 문서 목록

프로젝트의 상세한 구조와 가이드는 아래 개별 문서를 참고하세요:

- [**아키텍처 및 동작 원리 (architecture.md)**](file:///h:/source/repos/youtube%20studio%20custom/docs/architecture.md)
  - 소스 코드 구조 및 컴포넌트별 책임 분리
  - 수명 주기 시퀀스 다이어그램 및 스크립트 이중 등록 전략
  - SPA DOM 변이 감시 구조

- [**빌드 및 배포 가이드 (build-and-deployment.md)**](file:///h:/source/repos/youtube%20studio%20custom/docs/build-and-deployment.md)
  - 실행 및 빌드 사전 요구 사항
  - 단일 파일(`.exe`) 배포 명령어 (`dotnet publish`)
  - 배포 산출물 정리 및 패키징 팁

- [**트러블슈팅: 구독자 수 깜빡임 버그 수정 일지 (bugfixes/2026-08-06-flicker-bug.md)**](file:///h:/source/repos/youtube%20studio%20custom/docs/bugfixes/2026-08-06-flicker-bug.md)
  - 로딩 시 구독자 숫자가 0.1초 노출되던 원인 분석 (레이스 컨디션, DOM 파싱 타이밍)
  - Avalonia 원시 COM 포인터 래핑을 통한 해결 과정
  - 개발자 도구를 통한 YouTube Studio DOM 조사 기록
