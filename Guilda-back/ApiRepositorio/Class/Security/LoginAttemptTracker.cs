using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class.Security
{
    public class LoginAttemptTracker
    {
        private static Dictionary<string, (int Attempts, DateTime LockoutEndTime)> loginAttempts =
        new Dictionary<string, (int Attempts, DateTime LockoutEndTime)>();

        private const int MaxAttempts = 5;
        private const int LockoutMinutes = 5;

        public static bool IsAccountLocked(string username)
        {
            if (loginAttempts.ContainsKey(username))
            {
                var attempts = loginAttempts[username];
                if (attempts.Attempts >= MaxAttempts)
                {
                    if (DateTime.UtcNow < attempts.LockoutEndTime)
                    {
                        return true;
                    }
                    else
                    {
                        // Reset attempts after lockout period is over
                        loginAttempts[username] = (0, DateTime.UtcNow);
                        return false;
                    }
                }
            }
            return false;
        }

        public static void RecordFailedLogin(string username)
        {
            if (loginAttempts.ContainsKey(username))
            {
                var attempts = loginAttempts[username];
                attempts.Attempts++;
                if (attempts.Attempts >= MaxAttempts)
                {
                    // Set lockout end time
                    attempts.LockoutEndTime = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                }
                loginAttempts[username] = attempts;
            }
            else
            {
                loginAttempts[username] = (1, DateTime.UtcNow);
            }
        }

        public static void RecordSuccessfulLogin(string username)
        {
            if (loginAttempts.ContainsKey(username))
            {
                // Reset the counter on successful login
                loginAttempts.Remove(username);
            }
        }
    }
}