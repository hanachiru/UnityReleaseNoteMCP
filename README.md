[日本語版はこちら](./README_JA.md)

# Unity Release Note MCP Server

This project is a C# MCP (Model Context Protocol) server designed to fetch and provide information about Unity Editor releases, based on the official Unity Release API.

## Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation & Running](#installation--running)
- [Usage](#usage)
  - [`getUnityReleases`](#getunityreleases)
  - [`getUnityReleaseNotesContent`](#getunityreleasenotescontent)
- [Development](#development)
  - [Architecture](#architecture)
  - [Testing](#testing)

## Getting Started

Follow these instructions to get the project up and running on your local machine.

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation & Running

1. **Clone the repository:**
   ```sh
   git clone https://github.com/hanachiru/UnityReleaseNoteMCP.git
   cd UnityReleaseNoteMCP
   ```

2. **Run the server:**
   The server will start, listen for MCP requests from standard input, and send responses to standard output.
   ```sh
   dotnet run --project src/UnityReleaseNoteMCP
   ```

## Features

-   **Comprehensive Release Data**: Fetches a complete list of all available Unity releases by handling API pagination.
-   **Powerful Filtering**: Allows filtering releases by version (full or partial, e.g., "2022.3") and by stream (`LTS`, `BETA`, `ALPHA`, `TECH`).
-   **Latest LTS Release Notes**: Includes a dedicated method to find the latest official LTS release and return the direct URL to its markdown-based release notes.
-   **Robust and Resilient**: Implements in-memory caching to reduce redundant API calls and improve performance.
-   **Clean Architecture**: Built using a clean architecture, separating domain models, application logic, and infrastructure concerns.
-   **Fully Tested**: Includes a comprehensive NUnit test suite.

## Usage

The server exposes a `UnityReleaseTool` with two primary methods for fetching release data. On failure, all methods throw a `ToolExecutionException`.

### `getUnityReleases`

Retrieves a paginated, filtered, and ordered list of Unity releases.

**Parameters:**

| Name         | Type                | Description                                                              | Default             |
|--------------|---------------------|--------------------------------------------------------------------------|---------------------|
| `limit`      | `int`               | Limits the number of results returned per page (min 1, max 25).          | `10`                |
| `offset`     | `int`               | Offsets the first `n` elements from the results.                         | `0`                 |
| `order`      | `string`            | Orders results by release date. Can be `RELEASE_DATE_ASC` or `RELEASE_DATE_DESC`. | `RELEASE_DATE_DESC` |
| `stream`     | `IReadOnlyList<string>` | Filters by release stream (e.g., `["LTS", "BETA"]`).                     | `null` (all streams) |
| `platform`   | `IReadOnlyList<string>` | Filters by download platform (e.g., `["Windows", "Mac"]`).               | `null` (all platforms)|
| `architecture`| `IReadOnlyList<string>` | Filters by download architecture (e.g., `["X64"]`).                      | `null` (all architectures)|
| `version`    | `string`            | Filters by a full-text search on the version string.                     | `null`              |

**Returns:** `Task<UnityReleaseOffsetConnection>`

An object containing the paginated results (`Results`), total count (`Total`), `limit`, and `offset`.

**Example MCP Request:**

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

Get the content of the release notes for a specific Unity version.

**Parameters:**

| Name      | Type     | Description                                               |
|-----------|----------|-----------------------------------------------------------|
| `version` | `string` | The exact version string of the Unity Release (e.g., `'2022.3.10f1'`). |

**Returns:** `Task<string>`

The raw Markdown content of the release notes.

**Example MCP Request:**

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

## Development

### Architecture

The project follows Clean Architecture principles:

-   **`src/Domain`**: Contains the C# classes that map directly to the official Unity Release API schema.
-   **`src/Application`**: Holds the application logic, including the `UnityReleaseTool` and client interfaces.
-   **`src/Infrastructure`**: Contains the implementation for external services, including the `HttpClient`-based `UnityReleaseClient` and an `AngleSharp`-based HTML parser.

### Testing

The solution includes a test project at `tests/UnityReleaseNoteMCP.Tests`. Tests can be run via the .NET CLI:

```sh
dotnet test
```