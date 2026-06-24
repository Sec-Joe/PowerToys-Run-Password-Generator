using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Community.PowerToys.Run.Plugin.PasswordGenerator
{
    /// <summary>
    /// Password strength estimator using entropy-based calculation.
    /// </summary>
    public static class PasswordStrength
    {
        /// <summary>
        /// Strength level enum
        /// </summary>
        public enum StrengthLevel
        {
            VeryWeak,
            Weak,
            Fair,
            Strong,
            VeryStrong
        }

        /// <summary>
        /// Calculate password entropy in bits
        /// </summary>
        public static double CalculateEntropy(string password, PasswordOptions options)
        {
            if (string.IsNullOrEmpty(password))
            {
                return 0;
            }

            int poolSize = options.GetPoolSize();
            if (poolSize <= 0)
            {
                return 0;
            }

            // Entropy = length * log2(pool_size)
            double entropy = password.Length * Math.Log(poolSize, 2);

            // Bonus for character diversity
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));

            int diversityBonus = 0;
            if (hasUpper) diversityBonus += 4;
            if (hasLower) diversityBonus += 4;
            if (hasDigit) diversityBonus += 4;
            if (hasSymbol) diversityBonus += 8;

            entropy += diversityBonus;

            return Math.Max(0, entropy);
        }

        /// <summary>
        /// Get strength level from entropy
        /// </summary>
        public static StrengthLevel GetStrengthLevel(double entropy)
        {
            if (entropy < 28)
                return StrengthLevel.VeryWeak;
            if (entropy < 36)
                return StrengthLevel.Weak;
            if (entropy < 60)
                return StrengthLevel.Fair;
            if (entropy < 128)
                return StrengthLevel.Strong;
            return StrengthLevel.VeryStrong;
        }

        /// <summary>
        /// Get human-readable strength description
        /// </summary>
        public static string GetStrengthDescription(StrengthLevel level)
        {
            return level switch
            {
                StrengthLevel.VeryWeak => "Very Weak",
                StrengthLevel.Weak => "Weak",
                StrengthLevel.Fair => "Fair",
                StrengthLevel.Strong => "Strong",
                StrengthLevel.VeryStrong => "Very Strong",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Get visual strength bar
        /// </summary>
        public static string GetStrengthBar(StrengthLevel level)
        {
            return level switch
            {
                StrengthLevel.VeryWeak => "❌",
                StrengthLevel.Weak => "⚠️",
                StrengthLevel.Fair => "🔸",
                StrengthLevel.Strong => "🔹",
                StrengthLevel.VeryStrong => "✅",
                _ => "❓"
            };
        }

        /// <summary>
        /// Estimate crack time in human-readable format
        /// </summary>
        public static string EstimateCrackTime(double entropy)
        {
            // Assuming 10 billion guesses per second (modern GPU cluster)
            double guessesPerSecond = 10_000_000_000;
            double totalGuesses = Math.Pow(2, entropy);
            double seconds = totalGuesses / guessesPerSecond / 2; // Average case

            if (seconds < 1)
                return "Instant";
            if (seconds < 60)
                return $"{(int)seconds} seconds";
            if (seconds < 3600)
                return $"{(int)(seconds / 60)} minutes";
            if (seconds < 86400)
                return $"{(int)(seconds / 3600)} hours";
            if (seconds < 31536000)
                return $"{(int)(seconds / 86400)} days";
            if (seconds < 31536000L * 100)
                return $"{(int)(seconds / 31536000)} years";
            if (seconds < 31536000L * 1000000)
                return $"{(int)(seconds / 31536000 / 1000)} thousand years";
            if (seconds < 31536000L * 1000000000)
                return $"{(int)(seconds / 31536000 / 1000000)} million years";
            return $"{(int)(seconds / 31536000 / 1000000000)} billion years";
        }
    }
}
