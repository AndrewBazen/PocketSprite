# PocketSprite

**A cross-platform pixel art editor built for game developers, hobbyists, and creative coders.**

PocketSprite is an intuitive and powerful tool for creating pixel art sprites and animations. Designed with a focus on usability, performance, and extensibility, it streamlines the artistic workflow for indie devs, designers, and retro art enthusiasts.

---

## Table of Contents

1. [Introduction](#introduction)
2. [Features](#features)
3. [Roadmap](#roadmap)
4. [Installation](#installation)
5. [Usage](#usage)
6. [Testing](#testing)
7. [Dependencies](#dependencies)
8. [Contributing](#contributing)
9. [License](#license)

---

## Introduction

PocketSprite is a cross-platform pixel art editor targeting simplicity, flexibility, and developer friendliness. Whether you’re creating game assets or exploring the world of pixel art, PocketSprite provides a solid foundation with essential tools and planned extensibility.

### Why PocketSprite?

* **All-in-One Workflow**: Draw, edit, manage layers, and export — all within a single environment.
* **Cross-Platform Support**: Runs on Windows, macOS, Linux, Android, and iOS via .NET MAUI.
* **Built for Developers and Artists**: Practical features, a responsive UI, and code structured for extension.

---

## Features

* Pixel-accurate drawing tools (brush, eraser)
* Grid snapping, zooming, and panning
* Multiple layer support
* Palette editor and color picker
* Preview animations
* SkiaSharp-powered rendering for performance

---

## Roadmap

### Near-Term Goals

* Enhanced palette editor
* Core editing tools (selection, shapes)
* Canvas resizing
* Project save/load system
* Export as PNG/GIF
* Mobile app support
* UI settings and preferences

### Long-Term Vision

* Sprite animation timelines
* Export for popular game engines
* Palette sharing via [PaletteTheory](https://github.com/AndrewBazen/PaletteTheory)
* Custom UI theming
* Scriptable tools and plugin support

---

## Installation

> ⚠️ PocketSprite is currently in early development and must be built from source.

```bash
git clone https://github.com/AndrewBazen/PocketSprite.git
cd PocketSprite
dotnet workload install maui
dotnet clean
dotnet restore
dotnet build
dotnet run
```

Requires [.NET 9.0 SDK](https://dotnet.microsoft.com).

---

## Usage

Launch the app and begin drawing in the main canvas. Features include:

* **Left-click** to draw
* **Right-click** to erase
* **Scroll wheel** to zoom
* **Touch gestures** supported (WIP)

Sidebar and toolbar features are being added continuously.

---

## Testing

Run tests with:

```bash
dotnet test
```

Markdown summaries and color previews are saved to `/TestOutput/` when relevant.

---

## Dependencies

Make sure the following are installed:

* [.NET 9.0 SDK](https://dotnet.microsoft.com/)
* [MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui)
* [SkiaSharp](https://github.com/mono/SkiaSharp)
* [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
* [Docker](https://www.docker.com/) & [Docker Compose](https://docs.docker.com/compose/)
* [xUnit](https://xunit.net/)
* [Testcontainers for .NET](https://github.com/testcontainers/testcontainers-dotnet)

---

## Contributing

Contributions are welcome!

```bash
git clone https://github.com/AndrewBazen/PocketSprite.git
git checkout -b feature/my-feature
```

Open a pull request with a clear description and screenshots or test results.

---

## License

PocketSprite is licensed under the **GNU General Public License v3.0** (GPL-3.0).

> This license ensures that the project remains open source. Any derivative work must also remain open. While the source code and binaries are free, **prebuilt versions may be offered for purchase in the future** to help support ongoing development.

See the [`LICENSE`](LICENSE) file for full details.
