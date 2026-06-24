using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.PasswordGenerator
{
    /// <summary>
    /// PowerToys Run Password Generator Plugin
    /// Trigger keyword: "pw" (configurable)
    /// 
    /// Supported commands:
    /// - pw              : Generate default 16-char password
    /// - pw 20           : Generate 20-char password
    /// - pw 16 -nosym    : Generate without symbols
    /// - pw 24 -nosim    : Generate excluding similar chars
    /// - pw pin          : Generate 6-digit PIN
    /// - pw wifi         : Generate 20-char WiFi password
    /// - pw last         : Show last generated password (session)
    /// </summary>
    public class Main : IPlugin
    {
        /// <summary>
        /// Plugin ID - required by PowerToys 0.100.0.0 for plugin validation
        /// </summary>
        public static string PluginID => "PasswordGenerator";

        private PluginInitContext? _context;
        private readonly string _actionKeyword = "pw";
        
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name => "Password Generator";

        /// <summary>
        /// Action keyword
        /// </summary>
        public string ActionKeyword => _actionKeyword;

        /// <summary>
        /// Plugin description
        /// </summary>
        public string Description => "Generate secure random passwords instantly using FIPS 140-2 certified CSPRNG.";

        /// <summary>
        /// Initialize plugin
        /// </summary>
        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Process query from PowerToys Run
        /// </summary>
        public List<Result> Query(Query query)
        {
            var results = new List<Result>();
            
            if (query == null || string.IsNullOrWhiteSpace(query.Search))
            {
                return results;
            }

            string input = query.Search.Trim();
            
            // Parse command
            var command = ParseCommand(input);
            
            if (command.IsLastCommand)
            {
                // Handle "pw last" - show last generated password
                results.Add(CreateLastPasswordResult());
            }
            else
            {
                // Generate new password
                try
                {
                    string password = SecurePasswordGenerator.Generate(command.Options);
                    
                    // Store in session state
                    SessionState.Instance.SetLastPassword(password, command.Options);
                    
                    // Create result
                    results.Add(CreatePasswordResult(password, command.Options));
                    
                    // Add alternative: regenerate with different options
                    results.Add(CreateRegenerateResult());
                }
                catch (Exception ex)
                {
                    results.Add(new Result
                    {
                        Title = "Error generating password",
                        SubTitle = ex.Message,
                        IcoPath = "Images\\icon.png",
                        Score = 100
                    });
                }
            }
            
            return results;
        }

        /// <summary>
        /// Parse user input command
        /// </summary>
        private PasswordCommand ParseCommand(string input)
        {
            var command = new PasswordCommand();
            var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                command.Options = PasswordOptions.Default;
                return command;
            }

            // Check for special commands
            if (parts[0].ToLower() == "last")
            {
                command.IsLastCommand = true;
                return command;
            }
            
            if (parts[0].ToLower() == "pin")
            {
                command.Options = PasswordOptions.Pin;
                return command;
            }
            
            if (parts[0].ToLower() == "wifi")
            {
                command.Options = PasswordOptions.Wifi;
                return command;
            }

            // Parse length (first numeric argument)
            int length = 16;
            if (parts.Length > 0 && int.TryParse(parts[0], out int parsedLength))
            {
                length = parsedLength;
                parts = parts.Skip(1).ToArray();
            }
            
            command.Options = new PasswordOptions { Length = length };
            
            // Parse flags
            for (int i = 0; i < parts.Length; i++)
            {
                string flag = parts[i].ToLower();
                
                switch (flag)
                {
                    case "-sym":
                    case "--symbols":
                        command.Options.IncludeSymbols = true;
                        break;
                    case "-nosym":
                    case "--no-symbols":
                        command.Options.IncludeSymbols = false;
                        break;
                    case "-sim":
                    case "--similar":
                        command.Options.ExcludeSimilar = false;
                        break;
                    case "-nosim":
                    case "--no-similar":
                        command.Options.ExcludeSimilar = true;
                        break;
                    case "-upper":
                        command.Options.IncludeUppercase = true;
                        break;
                    case "-noupper":
                        command.Options.IncludeUppercase = false;
                        break;
                    case "-lower":
                        command.Options.IncludeLowercase = true;
                        break;
                    case "-nolower":
                        command.Options.IncludeLowercase = false;
                        break;
                    case "-digit":
                    case "-digits":
                        command.Options.IncludeDigits = true;
                        break;
                    case "-nodigit":
                    case "-nodigits":
                        command.Options.IncludeDigits = false;
                        break;
                }
            }
            
            command.Options.Validate();
            return command;
        }

        /// <summary>
        /// Create result for generated password
        /// </summary>
        private Result CreatePasswordResult(string password, PasswordOptions options)
        {
            double entropy = PasswordStrength.CalculateEntropy(password, options);
            var strengthLevel = PasswordStrength.GetStrengthLevel(entropy);
            string strengthText = PasswordStrength.GetStrengthDescription(strengthLevel);
            string strengthBar = PasswordStrength.GetStrengthBar(strengthLevel);
            
            return new Result
            {
                Title = password,
                SubTitle = $"Strength: {strengthBar} {strengthText} ({entropy:F0} bits) | Enter to copy",
                IcoPath = "Images\\icon.png",
                Score = 100,
                Action = (context) =>
                {
                    CopyToClipboard(password);
                    ShowToast("Password copied to clipboard");
                    return true;
                }
            };
        }

        /// <summary>
        /// Create result for last password (pw last)
        /// </summary>
        private Result CreateLastPasswordResult()
        {
            if (!SessionState.Instance.HasLastPassword())
            {
                return new Result
                {
                    Title = "No password generated in this session",
                    SubTitle = "Generate a password first using 'pw' command",
                    IcoPath = "Images\\icon.png",
                    Score = 100
                };
            }

            string? password = SessionState.Instance.GetLastPassword();
            if (string.IsNullOrEmpty(password))
            {
                return new Result
                {
                    Title = "No password generated in this session",
                    SubTitle = "Generate a password first using 'pw' command",
                    IcoPath = "Images\\icon.png",
                    Score = 100
                };
            }

            PasswordOptions? options = SessionState.Instance.GetLastOptions();
            DateTime time = SessionState.Instance.GetLastGeneratedTime();
            
            return new Result
            {
                Title = password,
                SubTitle = $"Last generated: {time:HH:mm:ss} | Length: {options?.Length ?? 0} | Enter to copy",
                IcoPath = "Images\\icon.png",
                Score = 100,
                Action = (context) =>
                {
                    CopyToClipboard(password);
                    ShowToast("Password copied to clipboard");
                    return true;
                }
            };
        }

        /// <summary>
        /// Create result for regenerating password
        /// </summary>
        private Result CreateRegenerateResult()
        {
            return new Result
            {
                Title = "Regenerate with different options",
                SubTitle = "Try: pw 20, pw pin, pw wifi, pw 16 -nosym",
                IcoPath = "Images\\icon.png",
                Score = 50
            };
        }

        /// <summary>
        /// Copy password to clipboard
        /// </summary>
        private void CopyToClipboard(string password)
        {
            try
            {
                Clipboard.SetText(password);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Clipboard error: {ex.Message}");
            }
        }

        /// <summary>
        /// Show toast notification
        /// </summary>
        private void ShowToast(string message)
        {
            try
            {
                var notifyIcon = new NotifyIcon()
                {
                    Visible = true,
                    Icon = System.Drawing.SystemIcons.Information,
                    BalloonTipTitle = "Password Generator",
                    BalloonTipText = message
                };
                
                notifyIcon.ShowBalloonTip(3000);
                
                Task.Delay(4000).ContinueWith(_ =>
                {
                    notifyIcon.Dispose();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Toast error: {ex.Message}");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            SessionState.Instance.Dispose();
        }
    }

    /// <summary>
    /// Helper class for parsing password commands
    /// </summary>
    internal class PasswordCommand
    {
        public PasswordOptions Options { get; set; } = PasswordOptions.Default;
        public bool IsLastCommand { get; set; } = false;
    }
}
