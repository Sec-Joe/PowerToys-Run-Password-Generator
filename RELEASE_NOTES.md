# Release Notes

## v1.1.0 (2026-06-24)

### 🐛 Bug Fixes
- **`pw` empty input** now generates default 16-char password (was: no results)
- **`pw pin`** now correctly generates 6-digit PIN (was: 8 digits due to length validation)
- **Password edge symbols** — passwords no longer start or end with special characters (`@abc!` → `XabcY`)
- **Build warnings** — fixed integer overflow in `PasswordStrength.cs` (CS0220), fixed nullable warnings

### 🚀 Improvements
- **`pw help` / `pw ?`** — new help command listing all supported options
- **Interactive help** — `pw`, `pw pin`, `pw wifi` items in help list are directly enterable
- **Compact UI** — shortened text to avoid PowerToys Run truncation

### Features (same as v1.0.0)
- **FIPS 140-2** certified CSPRNG (Windows CNG `BCryptGenRandom`)
- **No network**, **no account** — fully offline, zero telemetry
- **2 seconds flat** — generate and copy to clipboard instantly
- **char[] memory safety** — explicit clear on session end
- **Toast notification** on every password generation

### Usage
| Command | Description |
|---------|-------------|
| `pw` | Generate default 16-char password |
| `pw 20` | Custom length (8-64) |
| `pw pin` | 6-digit PIN |
| `pw wifi` | 20-char WiFi-safe password |
| `pw last` | Recall last password |

### Installation
1. Close PowerToys
2. Copy DLL to `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\PasswordGenerator\`
3. Start PowerToys
4. `Alt+Space` → type `pw`

---

## v1.0.0 (Initial Release)
- Initial release with core password generation
- FIPS 140-2 CSPRNG, clipboard copy, toast notifications
