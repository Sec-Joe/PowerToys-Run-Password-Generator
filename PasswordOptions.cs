using System;

namespace Community.PowerToys.Run.Plugin.PasswordGenerator
{
    /// <summary>
    /// Password generation options model.
    /// Maps to command line arguments: pw [length] [-sym|-nosym] [-sim|-nosim] ...
    /// </summary>
    public class PasswordOptions
    {
        /// <summary>
        /// Password length (8-64, default: 16)
        /// </summary>
        public int Length { get; set; } = 16;

        /// <summary>
        /// Include uppercase letters (A-Z)
        /// </summary>
        public bool IncludeUppercase { get; set; } = true;

        /// <summary>
        /// Include lowercase letters (a-z)
        /// </summary>
        public bool IncludeLowercase { get; set; } = true;

        /// <summary>
        /// Include digits (0-9)
        /// </summary>
        public bool IncludeDigits { get; set; } = true;

        /// <summary>
        /// Include symbol characters (!@#$%^&*...)
        /// </summary>
        public bool IncludeSymbols { get; set; } = true;

        /// <summary>
        /// Exclude similar characters (1/l/I, 0/O)
        /// </summary>
        public bool ExcludeSimilar { get; set; } = false;

        /// <summary>
        /// Minimum entropy threshold in bits (default: 80)
        /// </summary>
        public int MinEntropyBits { get; set; } = 80;

        /// <summary>
        /// Create default options
        /// </summary>
        public static PasswordOptions Default => new PasswordOptions();

        /// <summary>
        /// Create options for PIN (6 digits only)
        /// </summary>
        public static PasswordOptions Pin => new PasswordOptions
        {
            Length = 6,
            IncludeUppercase = false,
            IncludeLowercase = false,
            IncludeDigits = true,
            IncludeSymbols = false,
            ExcludeSimilar = false
        };

        /// <summary>
        /// Create options for WiFi password (20 chars, alphanumeric, no symbols)
        /// </summary>
        public static PasswordOptions Wifi => new PasswordOptions
        {
            Length = 20,
            IncludeUppercase = true,
            IncludeLowercase = true,
            IncludeDigits = true,
            IncludeSymbols = false,
            ExcludeSimilar = true
        };

        /// <summary>
        /// Validate options and adjust if necessary
        /// </summary>
        public void Validate()
        {
            // Clamp length to valid range (allow 4-64 for PIN codes)
            if (Length < 4)
            {
                Length = 4;
            }
            else if (Length > 64)
            {
                Length = 64;
            }

            // Ensure at least one character set is enabled
            if (!IncludeUppercase && !IncludeLowercase && !IncludeDigits && !IncludeSymbols)
            {
                IncludeLowercase = true;
                IncludeDigits = true;
            }
        }

        /// <summary>
        /// Calculate pool size for entropy estimation
        /// </summary>
        public int GetPoolSize()
        {
            int size = 0;
            
            if (IncludeUppercase)
            {
                size += ExcludeSimilar ? 25 : 26;
            }
            
            if (IncludeLowercase)
            {
                size += ExcludeSimilar ? 24 : 26;
            }
            
            if (IncludeDigits)
            {
                size += ExcludeSimilar ? 8 : 10;
            }
            
            if (IncludeSymbols)
            {
                size += 32;
            }
            
            return size;
        }

        /// <summary>
        /// Calculate estimated entropy in bits
        /// </summary>
        public double CalculateEntropy()
        {
            int poolSize = GetPoolSize();
            if (poolSize <= 0)
            {
                return 0;
            }
            
            return Length * Math.Log(poolSize, 2);
        }
    }
}
