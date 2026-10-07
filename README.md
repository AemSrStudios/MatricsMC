# MatricsMC Launcher

A modern, lightweight Minecraft launcher built with C# and WPF.

MatricsMC is designed to provide a clean, fast, and customizable way to manage Minecraft installations, accounts, launcher data, settings, mods, and modpacks from one application.

> 🚧 **MatricsMC is currently in active development.**

> ⚠️ **Disclaimer:** MatricsMC is an unofficial, independent third-party Minecraft launcher. It is not affiliated with or endorsed by Microsoft or Mojang. MatricsMC does not distribute Minecraft, pirated game files, cracked accounts, stolen credentials, or authentication bypasses.

---

# 📌 Project Status

MatricsMC is currently an early-development project.

Some features are implemented, while others are planned or still being developed.

Development builds may contain bugs, incomplete functionality, experimental features, and breaking changes.

The project is currently focused on Windows.

---

# ✨ Features

## 🎮 Minecraft

Planned Minecraft management features include:

- Minecraft version selection
- Minecraft version detection
- Minecraft version installation
- Minecraft launching
- Custom Minecraft directories
- Multiple installations
- Installation management
- Download progress
- Game launch status
- Java detection
- Java configuration
- RAM allocation
- JVM configuration

---

# 👤 Account Management

MatricsMC is designed to support multiple Minecraft accounts.

Planned account features:

- Microsoft account login
- Multiple accounts
- Account switching
- Account removal
- Minecraft profile detection
- Account status
- Minecraft username
- Minecraft UUID
- Secure authentication
- Authentication state detection

MatricsMC should never ask users to provide their Microsoft password directly to the launcher.

Microsoft authentication should use Microsoft's supported authentication system.

---

# 🔐 Microsoft Authentication

MatricsMC intends to support legitimate Microsoft/Minecraft authentication.

The authentication system should:

- Use Microsoft's supported authentication flow
- Authenticate the user through the appropriate Microsoft login process
- Detect the user's Minecraft profile
- Obtain the necessary authorization to launch Minecraft
- Keep authentication information protected
- Avoid storing passwords
- Avoid exposing authentication tokens

MatricsMC will never intentionally collect or store a user's Microsoft password.

---

# 📴 Offline Profiles

MatricsMC may provide offline profiles for:

- Launcher development
- UI testing
- Local testing
- Development environments
- Testing launcher functionality without authentication

Offline profiles are **not equivalent to authenticated Minecraft accounts**.

An offline profile:

- Does not authenticate with Microsoft.
- Does not prove Minecraft ownership.
- Does not generate fake Minecraft authentication tokens.
- Does not bypass Minecraft authentication.
- Does not bypass Minecraft licensing.
- Must be clearly identified as an offline profile.

Having a legitimate Minecraft account does not make an unauthenticated/offline session an authenticated Minecraft session.

When launching Minecraft normally, MatricsMC should use the user's legitimate authenticated account.

---

# 📦 Launcher Import

One of the main goals of MatricsMC is making it easy to move from another Minecraft launcher.

Potential supported launchers include:

- Official Minecraft Launcher
- Prism Launcher
- MultiMC
- ATLauncher
- CurseForge
- Modrinth App
- Lunar Client
- Badlion Client

MatricsMC may detect these launchers and allow users to import compatible local data.

---

# 📥 Importable Data

Depending on the launcher and format, MatricsMC may support importing:

- Worlds
- Saves
- Mods
- Resource packs
- Shader packs
- Screenshots
- Configurations
- Logs
- Launcher profiles
- Instances
- Modpack data
- Version data
- Selected launcher settings

Not every launcher will support every type of data.

Import compatibility will depend on the structure and format used by the source launcher.

---

# 🔄 Import Safety

The launcher import system should:

1. Detect installed launchers.
2. Display detected launchers.
3. Allow the user to select what to import.
4. Show what data will be copied.
5. Ask for confirmation when necessary.
6. Copy the selected data into MatricsMC.
7. Display import progress.
8. Report files that could not be copied.
9. Avoid deleting original launcher data.

The original launcher data should remain untouched unless the user explicitly chooses otherwise.

---

# 🔒 Authentication Data

Authentication information must be treated differently from normal Minecraft files.

MatricsMC must never intentionally import, expose, or publish:

- Microsoft passwords
- Authentication cookies
- Session cookies
- Access tokens
- Refresh tokens
- Client secrets
- Private authentication credentials

Authentication credentials must never be committed to GitHub.

---

# ⚙️ Settings

MatricsMC will provide a dedicated settings system.

Planned settings include:

## Minecraft

- Minecraft directory
- Java executable
- Java version
- RAM allocation
- JVM arguments
- Default Minecraft version

## Launcher

- Theme
- Startup behavior
- Update settings
- Download settings
- Logging
- Import behavior

## Account

- Default account
- Account switching
- Account management

---

# ☕ Java Management

MatricsMC is planned to automatically detect compatible Java installations.

Planned functionality:

- Detect installed Java versions
- Display Java version
- Select Java executable
- Automatically select Java when possible
- Warn when an incompatible Java version is detected
- Allow advanced users to specify a custom Java path

---

# 🧩 Mods & Modpacks

Future versions of MatricsMC may support:

- Mods
- Mod loaders
- Modpacks
- Fabric
- Forge
- NeoForge
- Quilt
- Modrinth modpacks
- CurseForge modpacks

Possible future functionality:

- Install mods
- Remove mods
- Enable/disable mods
- Update mods
- Install modpacks
- Update modpacks
- Manage modpack instances

---

# 🖥️ User Interface

MatricsMC uses a modern dark interface built with WPF.

Design goals:

- Clean
- Minimal
- Modern
- Fast
- Easy to understand
- Dark theme
- Clear navigation
- Subtle rounded elements
- Useful icons
- Responsive layouts
- Avoid unnecessary clutter

The launcher should feel like a modern desktop application rather than a collection of unrelated buttons.

---

# 🧭 Navigation

The launcher uses a sidebar-based navigation system.

Main sections include:

- 🏠 Home
- 🎮 Minecraft
- 📦 Installations
- 👤 Accounts
- ⚙️ Settings

Additional sections may be added as development continues.

---

# 🔄 Automatic Updates

MatricsMC is planned to eventually include an automatic update system.

Potential functionality:

- Check for new MatricsMC versions
- Display available updates
- Download updates
- Verify downloaded files
- Install updates
- Restart the launcher after updating

Updates should only be downloaded from trusted MatricsMC release sources.

---

# 🔒 Security

Security is an important part of MatricsMC.

MatricsMC should never intentionally:

- Steal credentials
- Collect Microsoft passwords
- Steal authentication tokens
- Capture session cookies
- Install malware
- Download malicious software
- Hide malicious processes
- Bypass security protections
- Modify unrelated user files without permission

Sensitive credentials must never be hard-coded into the application or repository.

---

# 🔑 GitHub Secrets

Developers must never commit secrets to the repository.

Do not commit:

```text
Passwords
API Keys
OAuth Secrets
Client Secrets
Access Tokens
Refresh Tokens
Session Tokens
Private Keys
Authentication Cookies
