[Read this in English](./README.md)

# Unityリリースノート MCPサーバー

このプロジェクトは、公式のUnity Release APIに基づき、Unityエディターのリリースに関する情報を取得・提供するために設計された、C#製のMCP (Model Context Protocol) サーバーです。

## 目次

- [はじめに](#はじめに)
  - [前提条件](#前提条件)
  - [インストールと実行](#インストールと実行)
- [機能](#機能)
- [使用方法](#使用方法)
  - [`getUnityReleases`](#getunityreleases)
  - [`getUnityReleaseNotesContent`](#getunityreleasenotescontent)
- [開発](#開発)
  - [アーキテクチャ](#アーキテクチャ)
  - [テスト](#テスト)

## はじめに

このセクションでは、プロジェクトをローカルマシンでセットアップして実行するための手順を説明します。

### 前提条件

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### インストールと実行

1. **リポジトリをクローンします:**
   ```sh
   git clone https://github.com/hanachiru/UnityReleaseNoteMCP.git
   cd UnityReleaseNoteMCP
   ```

2. **サーバーを実行します:**
   サーバーが起動し、標準入力からのMCPリクエストを待ち受け、標準出力に応答を送信します。
   ```sh
   dotnet run --project src/UnityReleaseNoteMCP
   ```

## 機能

-   **包括的なリリースデータ**: APIのページネーションを処理し、利用可能なすべてのUnityリリースの完全なリストを取得します。
-   **強力なフィルタリング**: バージョン（完全または部分的、例: `"2022.3"`）およびストリーム（`LTS`, `BETA`, `ALPHA`, `TECH`）によるリリースのフィルタリングが可能です。
-   **最新LTSリリースノート**: 最新の公式LTSリリースを特定し、そのMarkdown形式のリリースノートへの直接URLを返します。
-   **堅牢性と回復力**: インメモリキャッシュを実装し、冗長なAPI呼び出しを削減し、パフォーマンスを向上させます。
-   **クリーンアーキテクチャ**: ドメインモデル、アプリケーションロジック、インフラストラクチャの関心事を分離したクリーンアーキテクチャで構築されています。
-   **完全なテスト**: 包括的なNUnitテストスイートが含まれています。

## 使用方法

このサーバーは、リリースデータを取得するための主要な2つのメソッドを持つ `UnityReleaseTool` を公開しています。失敗した場合、すべてのメソッドは `ToolExecutionException` をスローします。

### `getUnityReleases`

ページネーション、フィルタリング、ソートが適用されたUnityリリースのリストを取得します。

**パラメータ:**

| 名前         | 型                  | 説明                                                                     | デフォルト          |
|--------------|---------------------|--------------------------------------------------------------------------|---------------------|
| `limit`      | `int`               | ページごとに返される結果の数を制限します（最小1、最大25）。              | `10`                |
| `offset`     | `int`               | 結果の最初の`n`要素をオフセットします。                                  | `0`                 |
| `order`      | `string`            | リリース日で結果をソートします。`RELEASE_DATE_ASC`または`RELEASE_DATE_DESC`が指定可能です。 | `RELEASE_DATE_DESC` |
| `stream`     | `IReadOnlyList<string>` | リリースストリームでフィルタリングします（例: `["LTS", "BETA"]`）。         | `null`（すべてのストリーム） |
| `platform`   | `IReadOnlyList<string>` | ダウンロードプラットフォームでフィルタリングします（例: `["Windows", "Mac"]`）。 | `null`（すべてのプラットフォーム）|
| `architecture`| `IReadOnlyList<string>` | ダウンロードアーキテクチャでフィルタリングします（例: `["X64"]`）。          | `null`（すべてのアーキテクチャ）|
| `version`    | `string`            | バージョン文字列の全文検索でフィルタリングします。                       | `null`              |

**戻り値:** `Task<UnityReleaseOffsetConnection>`

ページネーションされた結果（`Results`）、総数（`Total`）、`limit`、および`offset`を含むオブジェクト。

**MCPリクエストの例:**

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "getUnityReleases",
  "params": {
    "limit": 2,
    "stream": ["LTS"],
    "version": "2022.3"
  }
}
```

### `getUnityReleaseNotesContent`

特定のUnityバージョンのリリースノートのコンテンツを取得します。

**パラメータ:**

| 名前      | 型       | 説明                                                              |
|-----------|----------|-------------------------------------------------------------------|
| `version` | `string` | Unityリリースの正確なバージョン文字列（例: `'2022.3.10f1'`）。 |

**戻り値:** `Task<string>`

リリースノートの生のMarkdownコンテンツ。

**MCPリクエストの例:**

```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "getUnityReleaseNotesContent",
  "params": {
    "version": "2022.3.10f1"
  }
}
```

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
