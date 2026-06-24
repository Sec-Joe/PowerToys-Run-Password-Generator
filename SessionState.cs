using System;

namespace Community.PowerToys.Run.Plugin.PasswordGenerator
{
    /// <summary>
    /// Session state management for password generator.
    /// Stores the last generated password in memory for `pw last` command.
    /// Password is explicitly cleared when no longer needed.
    /// </summary>
    public class SessionState : IDisposable
    {
        private char[]? _lastPassword;
        private PasswordOptions? _lastOptions;
        private DateTime _lastGeneratedTime;
        private readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly Lazy<SessionState> _instance = new Lazy<SessionState>(() => new SessionState());
        
        public static SessionState Instance => _instance.Value;

        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private SessionState()
        {
            _lastPassword = null;
            _lastOptions = null;
            _lastGeneratedTime = DateTime.MinValue;
        }

        /// <summary>
        /// Store the last generated password in memory.
        /// Uses char[] for security (can be explicitly cleared).
        /// </summary>
        public void SetLastPassword(string password, PasswordOptions options)
        {
            if (string.IsNullOrEmpty(password))
            {
                return;
            }

            lock (_lock)
            {
                // Clear previous password if exists
                ClearLastPassword();

                // Store new password as char[]
                _lastPassword = password.ToCharArray();
                _lastOptions = options;
                _lastGeneratedTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Get the last generated password.
        /// Returns null if no password has been generated in this session.
        /// </summary>
        public string? GetLastPassword()
        {
            lock (_lock)
            {
                if (_lastPassword == null || _lastPassword.Length == 0)
                {
                    return null;
                }

                return new string(_lastPassword);
            }
        }

        /// <summary>
        /// Get the options used for the last password generation
        /// </summary>
        public PasswordOptions? GetLastOptions()
        {
            lock (_lock)
            {
                return _lastOptions;
            }
        }

        /// <summary>
        /// Get the timestamp of last password generation
        /// </summary>
        public DateTime GetLastGeneratedTime()
        {
            lock (_lock)
            {
                return _lastGeneratedTime;
            }
        }

        /// <summary>
        /// Check if there is a stored password
        /// </summary>
        public bool HasLastPassword()
        {
            lock (_lock)
            {
                return _lastPassword != null && _lastPassword.Length > 0;
            }
        }

        /// <summary>
        /// Clear the last password from memory.
        /// Explicitly zeros out the char[] array.
        /// </summary>
        public void ClearLastPassword()
        {
            lock (_lock)
            {
                if (_lastPassword != null)
                {
                    // Zero out the array for security
                    Array.Clear(_lastPassword, 0, _lastPassword.Length);
                    _lastPassword = null;
                }
                _lastOptions = null;
                _lastGeneratedTime = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Dispose - clear password from memory
        /// </summary>
        public void Dispose()
        {
            ClearLastPassword();
        }

        /// <summary>
        /// Destructor to ensure password is cleared from memory
        /// </summary>
        ~SessionState()
        {
            Dispose();
        }
    }
}
