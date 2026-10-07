# MatricsMC Launcher
## Disclaimer

MatricsMC is an independent third-party Minecraft launcher project.

MatricsMC is not affiliated with, endorsed by, sponsored by, or officially connected to Microsoft, Mojang Studios, Minecraft, Prism Launcher, MultiMC, ATLauncher, CurseForge, Modrinth, Lunar Client, Badlion Client, or any other third-party launcher or service referenced by this project.

Minecraft is a trademark of Microsoft Corporation and Mojang Studios.

MatricsMC does not distribute Minecraft game files, authentication credentials, or other copyrighted game assets.
A modern, lightweight Minecraft launcher built with C# and WPF.

MatricsMC is designed to provide a clean and simple way to manage Minecraft installations, accounts, launcher data, settings, and eventually mods/modpacks — all from one launcher.

## 🚧 Project Status

**Early Development / Experimental**

MatricsMC is currently under active development. Some features are working, while others are still being built.

Do not expect everything to work yet.

## ✨ Planned Features

- 🎮 Minecraft version management
- ▶️ Minecraft launching
- 👤 Account management
- 🔐 Microsoft account login
- 📦 Minecraft installation management
- 📁 Custom Minecraft directories
- 💾 Launcher data importing
- 🔄 Import worlds, mods, resource packs, shaders and configurations
- 🧩 Mod and modpack support
- ☕ Java detection and configuration
- 🧠 RAM allocation
- ⚙️ Launcher settings
- 🔄 Launcher auto-updates
- 📊 Download and installation progress
- 🖥️ Modern dark UI
- 🚀 Fast and lightweight launcher

## 📥 Launcher Import

MatricsMC is planned to support importing data from existing Minecraft launchers.

Potential supported launchers include:

- Official Minecraft Launcher
- Prism Launcher
- MultiMC
- ATLauncher
- CurseForge
- Modrinth
- Lunar Client
- Badlion Client

The goal is to make moving to MatricsMC as easy as possible without deleting or modifying the original launcher data.

## 👤 Accounts

The launcher currently has the foundation for account management.

Planned account features include:

- Microsoft account login
- Multiple accounts
- Account switching
- Account removal
- Minecraft profile detection
- Secure authentication

> Offline profiles are intended for development/testing and should not be considered equivalent to a real Microsoft Minecraft account.

## ⚙️ Settings

MatricsMC will provide settings for things such as:

- Minecraft directory
- RAM allocation
- Java configuration
- Selected account
- Launcher preferences
- Download settings

## 🛠️ Built With

- C#
- .NET
- WPF
- XAML
- Visual Studio

## 💻 Requirements

Currently intended for:

- Windows 10 / Windows 11
- .NET 8
- x64 Windows systems

Additional requirements may be added as development continues.

## 📂 Project Structure

```text
MatricsMC/
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── LauncherImportWindow.xaml
├── LauncherImportWindow.xaml.cs
├── AddAccountWindow.xaml
├── AddAccountWindow.xaml.cs
└── MatricsMC.csproj
