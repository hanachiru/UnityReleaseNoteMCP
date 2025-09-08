# Unity Release Note MCP Server

This project is a C# MCP (Model Context Protocol) server designed to fetch and provide information about Unity Editor releases, based on the official Unity Release API.

## Features

-   **Comprehensive Release Data**: Fetches a complete list of all available Unity releases by handling API pagination.
-   **Powerful Filtering**: Allows filtering releases by version (full or partial, e.g., "2022.3") and by stream (`LTS`, `BETA`, `ALPHA`, `TECH`).
-   **Latest LTS Release Notes**: Includes a dedicated method to find the latest official LTS release and return the direct URL to its markdown-based release notes.
-   **Robust and Resilient**: Implements in-memory caching to reduce redundant API calls and improve performance.
-   **Clean Architecture**: Built using a clean architecture, separating domain models, application logic, and infrastructure concerns.
-   **Fully Tested**: Includes a comprehensive NUnit test suite.

## Usage

The server exposes a `UnityReleaseTool` with the following methods. On failure, all methods throw a `ToolExecutionException`.

### `GetReleases(version, stream)`

Retrieves a list of `UnityRelease` objects, which can be filtered by version and/or stream.

-   **`version` (string, optional)**: Filter by a full or partial version string (e.g., `"2022.3"`, `"2023.1.0a22"`).
-   **`stream` (string, optional)**: Filter by release stream. Can be `"LTS"`, `"BETA"`, `"ALPHA"`, or `"TECH"`.
-   **Returns**: `Task<List<UnityRelease>>`

The `UnityRelease` object is a rich model containing the full data provided by the API, including version, release date, stream, and a list of downloads.

### `GetLatestLtsRelease()`

Finds the latest official LTS (Long-Term Support) release and returns the full `UnityRelease` object.

-   **Returns**: `Task<UnityRelease>`

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