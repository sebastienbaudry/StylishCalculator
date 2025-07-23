using System;
using System.IO;
using System.Text;
using StylishCalculator.Models;

namespace StylishCalculator.Services
{
    /// <summary>
    /// Centralized logging service that respects configuration settings
    /// </summary>
    public class LoggingService
    {
        #region Singleton Pattern
        
        private static LoggingService? _instance;
        private static readonly object _lock = new object();
        
        public static LoggingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggingService();
                        }
                    }
                }
                return _instance;
            }
        }
        
        private LoggingService()
        {
            // Initialize logging directory if needed
            InitializeLogging();
        }
        
        #endregion

        #region Private Fields
        
        private readonly object _logLock = new object();
        private string? _logFilePath;
        
        #endregion

        #region Initialization
        
        /// <summary>
        /// Initialize logging system
        /// </summary>
        private void InitializeLogging()
        {
            try
            {
                var config = AppConfiguration.Instance;
                if (config.EnableFileLogging)
                {
                    _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.LogFileName);
                    
                    // Create log directory if it doesn't exist
                    var logDirectory = Path.GetDirectoryName(_logFilePath);
                    if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    
                    // Check for log rotation
                    CheckLogRotation();
                }
            }
            catch (Exception ex)
            {
                // Fallback to debug output if file logging fails
                System.Diagnostics.Debug.WriteLine($"Failed to initialize logging: {ex.Message}");
            }
        }
        
        #endregion

        #region Public Logging Methods
        
        /// <summary>
        /// Log a trace message
        /// </summary>
        public void LogTrace(string message, string? category = null)
        {
            Log(LogLevel.Trace, message, category);
        }
        
        /// <summary>
        /// Log a debug message
        /// </summary>
        public void LogDebug(string message, string? category = null)
        {
            Log(LogLevel.Debug, message, category);
        }
        
        /// <summary>
        /// Log an info message
        /// </summary>
        public void LogInfo(string message, string? category = null)
        {
            Log(LogLevel.Info, message, category);
        }
        
        /// <summary>
        /// Log a warning message
        /// </summary>
        public void LogWarning(string message, string? category = null)
        {
            Log(LogLevel.Warning, message, category);
        }
        
        /// <summary>
        /// Log an error message
        /// </summary>
        public void LogError(string message, string? category = null)
        {
            Log(LogLevel.Error, message, category);
        }
        
        /// <summary>
        /// Log an error with exception details
        /// </summary>
        public void LogError(string message, Exception exception, string? category = null)
        {
            var fullMessage = $"{message}\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            Log(LogLevel.Error, fullMessage, category);
        }
        
        #endregion

        #region Core Logging Logic
        
        /// <summary>
        /// Core logging method
        /// </summary>
        private void Log(LogLevel level, string message, string? category = null)
        {
            var config = AppConfiguration.Instance;
            
            // Check if this log level should be processed
            if (level < config.LogLevel)
            {
                return;
            }
            
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var levelStr = level.ToString().ToUpper().PadRight(7);
            var categoryStr = string.IsNullOrEmpty(category) ? "" : $"[{category}] ";
            var logEntry = $"{timestamp} {levelStr} {categoryStr}{message}";
            
            lock (_logLock)
            {
                // Console logging
                if (config.EnableConsoleLogging)
                {
                    Console.WriteLine(logEntry);
                }
                
                // Debug output logging
                if (config.EnableDebugLogging)
                {
                    System.Diagnostics.Debug.WriteLine(logEntry);
                }
                
                // File logging
                if (config.EnableFileLogging && !string.IsNullOrEmpty(_logFilePath))
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
                        
                        // Check if log rotation is needed after writing
                        CheckLogRotation();
                    }
                    catch (Exception ex)
                    {
                        // Fallback to debug output if file writing fails
                        System.Diagnostics.Debug.WriteLine($"Failed to write to log file: {ex.Message}");
                    }
                }
            }
        }
        
        #endregion

        #region Log Rotation
        
        /// <summary>
        /// Check if log rotation is needed and perform it
        /// </summary>
        private void CheckLogRotation()
        {
            try
            {
                if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
                {
                    return;
                }
                
                var config = AppConfiguration.Instance;
                var fileInfo = new FileInfo(_logFilePath);
                var maxSizeBytes = config.MaxLogFileSizeMB * 1024 * 1024;
                
                if (fileInfo.Length > maxSizeBytes)
                {
                    RotateLogFiles();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to check log rotation: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Rotate log files
        /// </summary>
        private void RotateLogFiles()
        {
            try
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    return;
                }
                
                var config = AppConfiguration.Instance;
                var directory = Path.GetDirectoryName(_logFilePath);
                var fileName = Path.GetFileNameWithoutExtension(_logFilePath);
                var extension = Path.GetExtension(_logFilePath);
                
                if (string.IsNullOrEmpty(directory))
                {
                    return;
                }
                
                // Remove oldest log file if we've reached the retention limit
                var oldestLogFile = Path.Combine(directory, $"{fileName}.{config.LogFileRetentionCount}{extension}");
                if (File.Exists(oldestLogFile))
                {
                    File.Delete(oldestLogFile);
                }
                
                // Shift existing log files
                for (int i = config.LogFileRetentionCount - 1; i >= 1; i--)
                {
                    var currentFile = Path.Combine(directory, $"{fileName}.{i}{extension}");
                    var nextFile = Path.Combine(directory, $"{fileName}.{i + 1}{extension}");
                    
                    if (File.Exists(currentFile))
                    {
                        if (File.Exists(nextFile))
                        {
                            File.Delete(nextFile);
                        }
                        File.Move(currentFile, nextFile);
                    }
                }
                
                // Move current log file to .1
                var firstRotatedFile = Path.Combine(directory, $"{fileName}.1{extension}");
                if (File.Exists(firstRotatedFile))
                {
                    File.Delete(firstRotatedFile);
                }
                File.Move(_logFilePath, firstRotatedFile);
                
                LogInfo("Log file rotated successfully", "LoggingService");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to rotate log files: {ex.Message}");
            }
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Clear all log files
        /// </summary>
        public void ClearLogs()
        {
            try
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    return;
                }
                
                var config = AppConfiguration.Instance;
                var directory = Path.GetDirectoryName(_logFilePath);
                var fileName = Path.GetFileNameWithoutExtension(_logFilePath);
                var extension = Path.GetExtension(_logFilePath);
                
                if (string.IsNullOrEmpty(directory))
                {
                    return;
                }
                
                lock (_logLock)
                {
                    // Delete main log file
                    if (File.Exists(_logFilePath))
                    {
                        File.Delete(_logFilePath);
                    }
                    
                    // Delete rotated log files
                    for (int i = 1; i <= config.LogFileRetentionCount; i++)
                    {
                        var rotatedFile = Path.Combine(directory, $"{fileName}.{i}{extension}");
                        if (File.Exists(rotatedFile))
                        {
                            File.Delete(rotatedFile);
                        }
                    }
                }
                
                LogInfo("All log files cleared", "LoggingService");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to clear logs: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get current log file path
        /// </summary>
        public string? GetLogFilePath()
        {
            return _logFilePath;
        }
        
        /// <summary>
        /// Check if logging is enabled for a specific level
        /// </summary>
        public bool IsLogLevelEnabled(LogLevel level)
        {
            return level >= AppConfiguration.Instance.LogLevel;
        }
        
        #endregion
    }
}