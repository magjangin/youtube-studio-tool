# 빌드 및 배포 가이드

YouTube Studio Custom 프로젝트의 빌드 요구 사항 및 단일 실행 파일(Single-File) 배포 방법을 안내합니다.

---

## 1. 사전 요구 사항

- **운영체제**: Windows 10 / 11 (x64)
- **.NET SDK**: .NET 10.0 SDK 이상
- **WebView2 런타임**: Evergreen WebView2 Runtime (최신 Windows/Edge 환경에 기본 내장)
- **주요 종속성**:
  - `Avalonia` (12.0.5)
  - `Avalonia.Controls.WebView` (12.0.1)
  - `Microsoft.Web.WebView2` (1.0.3124.44) — Avalonia `NativeWebView`의 `ICoreWebView2` COM 핸들을 직접 제어하기 위해 필수

---

## 2. 배포 명령어 (Single-File Publish)

터미널에서 아래 명령어를 실행하여 외부 DLL 종속성이 하나로 묶인 단일 `.exe` 파일을 생성합니다.

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

### 주요 옵션 설명

| 옵션 | 설명 |
| :--- | :--- |
| `-c Release` | 릴리즈 최적화 빌드 적용 |
| `-r win-x64` | 64비트 Windows 대상 런타임 식별자 |
| `--self-contained true` | .NET 런타임이 설치되지 않은 PC에서도 바로 실행 가능하도록 런타임 포함 |
| `-p:PublishSingleFile=true` | 모든 어셈블리와 리소스를 하나의 `.exe` 파일로 압축 패키징 |
| `-p:IncludeNativeLibrariesForSelfExtract=true` | 네이티브 바이너리(WebView2Loader 등)를 단일 파일에 포함 |
| `-p:DebugType=none` | 디버그 심볼 생성을 비활성화하여 최종 바이너리 용량 절감 |
| `-o "publish/win-x64"` | 출력 대상 디렉토리 지정 |

---

## 3. 퍼블리시 산출물 정리 팁

배포 완료 후 `publish/win-x64` 폴더에 `Microsoft.Web.WebView2` NuGet 패키지 등에서 파생된 `.xml` 문서 파일이나 `.pdb` 파일이 남아있을 수 있습니다.

```powershell
# 배포 폴더 내 불필요한 부수 파일 정리 (PowerShell)
Remove-Item -Path "publish/win-x64/*.xml", "publish/win-x64/*.pdb" -ErrorAction SilentlyContinue
```

위 정리 과정을 거치면 최종적으로 [**`publish/win-x64/youtube studio custom.exe`**](file:///h:/source/repos/youtube%20studio%20custom/publish/win-x64/youtube%20studio%20custom.exe) 단일 파일 하나만 배포할 수 있습니다.
