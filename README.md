# Unity Release Note MCP Server

This project is a C# MCP (Model Context Protocol) server designed to fetch and provide information about Unity Editor releases.

## Features

-   **Fetch Release Lists**: Retrieves a list of official or beta Unity Editor releases.
-   **Get Latest Release Notes**: Identifies the latest release in a given category (official or beta), fetches the corresponding release notes page, and provides a summary.
-   **Get Release Notes by Version**: Fetches the release notes for a specific version string.
-   **Clean Architecture**: Built using a clean architecture approach, separating concerns into Domain, Application, and Infrastructure layers.
-   **Tested**: Includes a suite of NUnit unit tests to ensure reliability.

## Usage

The server exposes a `UnityReleaseTool` with the following methods that can be called by an MCP client. On failure, all methods throw a `ToolExecutionException`.

### `GetUnityReleases(releaseType)`

Retrieves a list of available Unity Editor releases.

-   **`releaseType` (string)**: The type of releases to list. Can be `"official"` (default) or `"beta"`.
-   **Returns**: `Task<ReleaseListResult>`

A `ReleaseListResult` object contains:
-   `string ReleaseType`: The type of release requested.
-   `List<string> Versions`: A list of version strings.

### `GetLatestReleaseNotes(releaseType)`

Finds the latest release of a specific type and returns its release notes.

-   **`releaseType` (string)**: The type of release to find. Can be `"official"` (default) or `"beta"`.
-   **Returns**: `Task<ReleaseNotesResult>`

### `GetReleaseNotesByVersion(version)`

Fetches the release notes for a specific, full version string.

-   **`version` (string)**: The full version string to look up (e.g., `"2022.3.8f1"`).
-   **Returns**: `Task<ReleaseNotesResult>`

A `ReleaseNotesResult` object contains:
-   `string Version`: The version number.
-   `string Url`: The URL to the full release notes.
-   `string Summary`: A brief summary of the release notes content.
-   `string ReleaseType`: The type of release.

### `GetReleasesByStream(stream)`

Retrieves a list of releases that belong to a specific major or minor version stream.

-   **`stream` (string)**: The partial version string to filter by (e.g., `"2022.3"`).
-   **Returns**: `Task<ReleaseListResult>`

## Development

### Architecture

The project follows Clean Architecture principles:

-   **`src/Domain`**: Contains the core domain models representing the API data structures.
-   **`src/Application`**: Holds the application logic, including the MCP tool definitions and client interfaces.
-   **`src/Infrastructure`**: Contains the implementation for external services, such as the `HttpClient`-based client for the Unity API.

### Testing

The solution includes a test project at `tests/UnityReleaseNoteMCP.Tests`. Tests are written using the NUnit framework and can be run via the .NET CLI:

```sh
dotnet test
```