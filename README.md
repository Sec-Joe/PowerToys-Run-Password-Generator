# PowerToys Run Password Generator Plugin

**Author**: JoeSec  
**Version**: 1.0.0

---

## Overview

A secure, zero-friction password generator plugin for PowerToys Run. Generate strong passwords instantly with `Alt+Space` → `pw`.

### Key Features

- 🔐 **FIPS 140-2 Certified CSPRNG** - Uses Windows CNG (BCryptGenRandom)
- ⚡ **Zero friction** - 2-step workflow (Alt+Space → pw → Enter)
- 🎯 **No account required** - Fully local, zero network requests
- 🧠 **Session recall** - `pw last` to retrieve last generated password
- 🎨 **Presets** - `pw pin`, `pw wifi` for common use cases
- 🛡️ **Memory safe** - Passwords stored as `char[]`, explicitly cleared

---

## Installation

### Prerequisites

- PowerToys 0.100.0.0 or later installed
- .NET 10 SDK

### Build & Install

1. **Clone this repository**
   ```bash
   git clone https://github.com/JoeSec/PowerToysPasswordGenerator.git
   cd PowerToysPasswordGenerator
   ```

2. **Build the plugin**
   ```bash
   dotnet build -c Release
   ```

3. **Copy to PowerToys plugins directory**
   
   Create the plugin folder:
   ```
   %LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\PasswordGenerator\
   ```
   
   Copy these files from `bin\Release\net10.0-windows10.0.19041.0\`:
   - `Community.PowerToys.Run.Plugin.PasswordGenerator.dll`
   - `plugin.json`
   - `Images/icon.png`

4. **Restart PowerToys**
   - Open PowerToys Settings
   - Restart PowerToys Run (or restart the app)

---

## Usage

| Command | Description |
|---------|-------------|
| `pw` | Generate default 16-char password |
| `pw 20` | Generate 20-char password |
| `pw 16 -nosym` | Generate without symbols |
| `pw 24 -nosim` | Generate excluding similar chars (1/l/0/O) |
| `pw 8 -upper -nosym` | Combine multiple options |
| `pw pin` | 6-digit PIN (digits only) |
| `pw wifi` | 20-char WiFi password (alphanumeric, no similar) |
| `pw last` | Show last generated password (session) |

### Examples

**Generate a 20-character password:**
```
Alt+Space → pw 20 → Enter
```

**Generate a WiFi password:**
```
Alt+Space → pw wifi → Enter
```

**Retrieve last password:**
```
Alt+Space → pw last → Enter
```

---

## Security

### CSPRNG (Cryptographically Secure Pseudo-Random Number Generator)

This plugin uses **Windows CNG (BCryptGenRandom)** via .NET's `System.Security.Cryptography.RandomNumberGenerator`:

- ✅ **FIPS 140-2 certified** 
- ✅ **OS kernel-level entropy**
- ✅ **Hardware entropy sources + TPM seed**

### Memory Safety

Passwords are stored in memory as `char[]` (not `string`):

```csharp
// In SessionState.cs
private char[]? _lastPassword;

public void ClearLastPassword()
{
    if (_lastPassword != null)
    {
        Array.Clear(_lastPassword, 0, _lastPassword.Length);
        _lastPassword = null;
    }
}
```

### Telemetry

- ✅ No password content logged
- ✅ No length or character set choices logged
- ✅ Only anonymous usage statistics (plugin invoked, success/failure)

---

## Comparison

| Feature | This Plugin | LastPass | Bitwarden |
|---------|-------------|----------|-----------|
| RNG Certification | ✅ FIPS 140-2 | ❌ Web Crypto | ❌ Web Crypto |
| Friction | 2 steps | 3-4 steps | 3-4 steps |
| Account Required | ❌ No | ✅ Yes | ✅ Yes |
| Open Source | ✅ Yes | ❌ No | ✅ Yes |
| Session Recall | ✅ pw last | ❌ No | ❌ No |
| Presets | ✅ pin, wifi | ❌ No | ❌ No |

---

## File Structure

```
PowerToysPasswordGenerator/
├── plugin.json                          # Plugin metadata
├── Community.PowerToys.Run.Plugin.PasswordGenerator.csproj
├── Main.cs                              # Plugin entry (IPlugin)
├── SecurePasswordGenerator.cs           # Core generation (CSPRNG)
├── PasswordOptions.cs                   # Parameter model
├── PasswordStrength.cs                  # Entropy calculation
├── SessionState.cs                      # Session password cache
├── Images/
│   └── icon.png                         # Plugin icon
├── Properties/
│   └── Resources.resx                   # Localization
└── README.md
```

---

## Troubleshooting

### Plugin not appearing in PowerToys Run

1. Check plugin is in correct directory: `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\PasswordGenerator\`
2. Verify `plugin.json` exists and is valid JSON
3. Restart PowerToys
4. Check PowerToys logs: `%LOCALAPPDATA%\Microsoft\PowerToys\`

### Build Errors

If you see "Wox.Plugin.dll not found":
1. Install PowerToys 0.100.0.0
2. Set environment variable: `POWERSHELL_POWERTOYS_DIR=C:\Users\<YourUser>\AppData\Local\PowerToys`
3. Rebuild

---

## License

MIT License

---

## V2 Roadmap

| Feature | Status |
|---------|--------|
| Passphrase mode (diceware) | Planned |
| Custom exclude characters | Planned |
| Multi-result display | Planned |

---

## Author

**JoeSec** - Security-focused developer
