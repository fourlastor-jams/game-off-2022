# Game Off 2022

Repository for our [Game Off 2022](https://itch.io/jam/game-off-2022) jam entry.

## Setup with VSCode

Note: both Mono and .NET SDK links refer to Apple Silicon versions, but you can install others.

- Install [Mono](https://www.mono-project.com/download/stable/#download-mac)
- Install [.NET Core SDK](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.402-macos-arm64-installer?journey=vs-code)
- Install the [VSCode C# Plugin](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- Install the [VSCode C# Gogot Plugin](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)

### git pre-commit hook

We use the dotnet formatter to format the code. You can setup a pre-commit hook
so it the code is automatically formatted on each commit. In the repo's dir run:

```sh
echo '#!/bin/sh
set -eo pipefail
FILES=$(git diff --name-only --cached)
dotnet format game-off-2022.sln --include $FILES
git add $FILES' > .git/hooks/pre-commit\
chmod +x .git/hooks/pre-commit
```
