# Unityリリースノート MCPサーバー

このプロジェクトは、Unityエディターのリリースに関する情報を取得・提供するために設計された、C#製のMCP (Model Context Protocol) サーバーです。

## 機能

-   **リリース一覧の取得**: Unityエディターの公式版またはベータ版のリリース一覧を取得します。
-   **最新リリースノートの取得**: 指定されたカテゴリ（公式またはベータ）の最新リリースを特定し、対応するリリースノートのページを取得して概要を提供します。
-   **バージョン指定によるリリースノート取得**: 特定のバージョン文字列に対応するリリースノートを取得します。
-   **クリーンアーキテクチャ**: ドメイン、アプリケーション、インフラストラクチャ層に関心を分離する、クリーンアーキテクチャのアプローチで構築されています。
-   **テスト済み**: 信頼性を確保するため、NUnitによる単体テストスイートが含まれています。

## 使用方法

このサーバーは、MCPクライアントから呼び出すことができる以下のメソッドを持つ `UnityReleaseTool` を公開しています。失敗した場合、すべてのメソッドは `ToolExecutionException` をスローします。

### `GetUnityReleases(releaseType)`

利用可能なUnityエディターのリリース一覧を取得します。

-   **`releaseType` (string)**: 一覧表示するリリースの種類。`"official"` (デフォルト) または `"beta"` を指定できます。
-   **戻り値**: `Task<ReleaseListResult>`

`ReleaseListResult` オブジェクトの内容:
-   `string ReleaseType`: 要求されたリリースの種類。
-   `List<string> Versions`: バージョン文字列のリスト。

### `GetLatestReleaseNotes(releaseType)`

指定された種類の最新リリースを見つけ、そのリリースノートを返します。

-   **`releaseType` (string)**: 検索するリリースの種類。`"official"` (デフォルト) または `"beta"` を指定できます。
-   **戻り値**: `Task<ReleaseNotesResult>`

### `GetReleaseNotesByVersion(version)`

完全なバージョン文字列を指定して、特定のリリースノートを取得します。

-   **`version` (string)**: 検索する完全なバージョン文字列（例: `"2022.3.8f1"`）。
-   **戻り値**: `Task<ReleaseNotesResult>`

`ReleaseNotesResult` オブジェクトの内容:
-   `string Version`: バージョン番号。
-   `string Url`: 完全なリリースノートへのURL。
-   `string Summary`: リリースノート内容の簡単な要約。
-   `string ReleaseType`: リリースの種類。

## 開発

### アーキテクチャ

このプロジェクトはクリーンアーキテクチャの原則に従っています。

-   **`src/Domain`**: APIのデータ構造を表すコアなドメインモデルを含みます。
-   **`src/Application`**: MCPツールの定義やクライアントインターフェースを含む、アプリケーションロジックを保持します。
-   **`src/Infrastructure`**: Unity API用の `HttpClient` ベースのクライアントなど、外部サービスの実装を含みます。

### テスト

このソリューションには `tests/UnityReleaseNoteMCP.Tests` にテストプロジェクトが含まれています。テストはNUnitフレームワークで記述されており、.NET CLI経由で実行できます。

```sh
dotnet test
```
