# Unityリリースノート MCPサーバー

このプロジェクトは、公式のUnity Release APIに基づき、Unityエディターのリリースに関する情報を取得・提供するために設計された、C#製のMCP (Model Context Protocol) サーバーです。

## 機能

-   **包括的なリリースデータ**: APIのページネーションを処理し、利用可能なすべてのUnityリリースの完全なリストを取得します。
-   **強力なフィルタリング**: バージョン（完全または部分的、例: `"2022.3"`）およびストリーム（`LTS`, `BETA`, `ALPHA`, `TECH`）によるリリースのフィルタリングが可能です。
-   **最新LTSリリースノート**: 最新の公式LTSリリースを特定し、そのMarkdown形式のリリースノートへの直接URLを返します。
-   **堅牢性と回復力**: インメモリキャッシュを実装し、冗長なAPI呼び出しを削減し、パフォーマンスを向上させます。
-   **クリーンアーキテクチャ**: ドメインモデル、アプリケーションロジック、インフラストラクチャの関心事を分離したクリーンアーキテクチャで構築されています。
-   **完全なテスト**: 包括的なNUnitテストスイートが含まれています。

## 使用方法

このサーバーは、以下のメソッドを持つ `UnityReleaseTool` を公開しています。失敗した場合、すべてのメソッドは `ToolExecutionException` をスローします。

### `GetReleases(version, stream)`

`UnityRelease` オブジェクトのリストを取得します。バージョンやストリームでフィルタリングできます。

-   **`version` (string, optional)**: 完全または部分的なバージョン文字列でフィルタリングします（例: `"2022.3"`, `"2023.1.0a22"`）。
-   **`stream` (string, optional)**: リリースストリームでフィルタリングします。`"LTS"`, `"BETA"`, `"ALPHA"`, `"TECH"` が指定可能です。
-   **戻り値**: `Task<List<UnityRelease>>`

`UnityRelease` オブジェクトは、バージョン、リリース日、ストリーム、ダウンロードリストなど、APIから提供される完全なデータを含むリッチモデルです。

### `GetLatestLtsReleaseNotesUrl()`

最新の公式LTS（長期サポート）リリースを見つけ、そのMarkdown形式のリリースノートファイルへの直接URLを返します。

-   **戻り値**: `Task<string>`

## 開発

### アーキテクチャ

このプロジェクトはクリーンアーキテクチャの原則に従っています。

-   **`src/Domain`**: Unity Release APIの公式スキーマに直接マッピングされたC#クラスを含みます。
-   **`src/Application`**: `UnityReleaseTool`やクライアントインターフェースを含む、アプリケーションロジックを保持します。
-   **`src/Infrastructure`**: `HttpClient`ベースの`UnityReleaseClient`や、HTMLパーサーの実装を含む、外部サービスの実装を含みます。

### テスト

このソリューションには `tests/UnityReleaseNoteMCP.Tests` にテストプロジェクトが含まれています。テストは.NET CLI経由で実行できます。

```sh
dotnet test
```
