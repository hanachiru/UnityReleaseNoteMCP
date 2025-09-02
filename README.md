# Unity Release Note MCP Server

This project is a C# MCP (Model Context Protocol) server designed to fetch and provide information about Unity Editor releases.

## Features

-   **Fetch Release Lists**: Retrieves a list of official or beta Unity Editor releases.
-   **Get Latest Release Notes**: Identifies the latest release in a given category (official or beta), fetches the corresponding release notes page, and provides a summary.
-   **Get Release Notes by Version**: Fetches the release notes for a specific version string.
-   **Clean Architecture**: Built using a clean architecture approach, separating concerns into Domain, Application, and Infrastructure layers.
-   **Tested**: Includes a suite of NUnit unit tests to ensure reliability.

## Usage

The server exposes a `UnityReleaseTool` with the following methods that can be called by an MCP client:

### `GetUnityReleases(releaseType)`

Retrieves a list of available Unity Editor releases.

-   **`releaseType` (string)**: The type of releases to list.
    -   `"official"` (default): Lists official `f`-series releases.
    -   `"beta"`: Lists beta `b`-series and alpha `a`-series releases.

**Example Response:**

```
--- Unity Official Releases ---
- 2022.3.10f1
- 2022.3.5f1
```

### `GetLatestReleaseNotes(releaseType)`

Finds the latest release of a specific type and returns a summary of its release notes.

-   **`releaseType` (string)**: The type of release to find the latest of.
    -   `"official"` (default): Finds the latest official release.
    -   `"beta"`: Finds the latest beta/alpha release.

**Example Response:**

```
Latest official version: 2022.3.10f1
Release Notes URL: https://unity.com/releases/editor/whats-new/2022.3.10f1

Release Notes Summary:
<html><body><h1>Hello Unity 2022.3.10f1</h1></body></html>...
```

### `GetReleaseNotesByVersion(version)`

Fetches the release notes for a specific, full version string.

-   **`version` (string)**: The full version string to look up (e.g., `"2022.3.8f1"`).

**Example Response:**
```
Release Notes for 2022.3.8f1:
URL: https://unity.com/releases/editor/whats-new/2022.3.8f1

Summary:
<html><body>Mocked release notes content.</body></html>...
```

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