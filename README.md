# SapsMC — GitHub Notes

## 📌 Project Notes

**SapsMC** is the Minecraft launcher/project name associated with **AemSrStudios**.

SapsLauncher is being developed as a custom C# WPF Minecraft launcher focused on a clean, simple, modern experience.

### 🛠️ Development

* Language: **C#**
* Framework: **.NET 8**
* UI: **WPF**
* Platform: **Windows**
* Project: **SapsLauncher**
* Minecraft launcher library: **CmlLib.Core**
* Microsoft authentication: **CmlLib.Core.Auth.Microsoft**

### 🚧 Current Status

SapsLauncher is currently in development.

Current development focuses on:

* Minecraft version selection
* Minecraft installation
* Minecraft launching
* Java detection
* RAM configuration
* Microsoft account authentication
* Offline profiles for testing
* Minecraft directory configuration
* Launcher UI
* Settings
* Installation management

Some planned features may not be fully implemented yet.

### 🔮 Planned Features

* [ ] Persistent account management
* [ ] Multiple Microsoft accounts
* [ ] Launcher data importing
* [ ] Import progress and migration
* [ ] Automatic launcher detection
* [ ] Mod management
* [ ] Modpack management
* [ ] Fabric support
* [ ] Forge support
* [ ] NeoForge support
* [ ] Automatic Java management
* [ ] Automatic updates
* [ ] Better download progress
* [ ] Installation profiles
* [ ] Custom Minecraft instances
* [ ] Custom themes
* [ ] More launcher settings
* [ ] Improved error handling
* [ ] Crash logs
* [ ] Launcher logging

### 🔐 Authentication Notes

SapsLauncher should **never request or store a user's Microsoft password**.

Microsoft authentication should be handled through the supported authentication flow provided by the authentication library.

Authentication tokens and account information should be handled carefully and never committed to GitHub.

### 📂 Repository Structure

The project is intended to eventually look similar to:

```text
SapsLauncher/
├── SapsLauncher.csproj
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Services/
├── Models/
├── Views/
├── Resources/
├── Assets/
├── README.md
├── LICENSE
└── .gitignore
```

### ⚠️ GitHub Security

Never commit:

* Microsoft passwords
* Access tokens
* Refresh tokens
* Client secrets
* Personal Minecraft account data
* Local launcher databases
* Private keys
* API keys
* Build output
* User-specific configuration files

Use `.gitignore` to prevent local/private files from being uploaded.

### 🧪 Testing

Before submitting changes, test:

1. Launcher startup
2. Minecraft version detection
3. Java detection
4. Minecraft installation
5. Microsoft login
6. Offline profile creation
7. Minecraft launching
8. Settings
9. Invalid/missing Java
10. Invalid Minecraft directory
11. Network/download failures

### 🐛 Bug Reports

When reporting a bug, include:

* Windows version
* SapsLauncher version/commit
* Minecraft version
* Java version
* What happened
* What was expected
* Error message
* Relevant launcher logs

**Do not include passwords, tokens, or other private account information.**

### 💡 Development Philosophy

SapsLauncher should remain:

* Simple
* Fast
* Clean
* Modern
* Lightweight
* Easy to understand
* Safe with user data

The goal is to make a launcher that feels polished without making the interface unnecessarily complicated.

### 📜 Disclaimer

SapsLauncher is an independent project and is not affiliated with Mojang Studios or Microsoft.

Minecraft is a trademark of Mojang Studios.

SapsLauncher does not provide unauthorized Minecraft accounts or bypass Minecraft authentication.

### ❤️ SapsMC

**SapsMC** is the project/brand associated with AemSrStudios.

Built for Minecraft players who want a clean and customizable launcher experience.

> **SapsMC — Simple. Clean. Minecraft.**
