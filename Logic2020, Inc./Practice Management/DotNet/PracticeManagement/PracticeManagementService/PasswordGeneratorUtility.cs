namespace PracticeManagementService
{
    using System;
    using System.Text;

    internal static class PasswordGeneratorUtility
    {
        /// <summary>
        /// Generates new password that contains only numbers and letters.
        /// Replaces the disallowed symbols in the old password with new random symbols.
        /// </summary>
        /// <param name="oldPassword">old generated password</param>
        /// <returns>the newly generated password which length will be the same as the length of the provided password.</returns>
        internal static string GeneratePassword(string oldPassword)
        {
            if (oldPassword == null)
            {
                throw new ArgumentNullException("oldPassword");
            }

            if (oldPassword.Length.Equals(0))
            {
                throw new ArgumentException(@"Password can't be empty!", "oldPassword");
            }

            Random rnd = new Random(DateTime.Now.Millisecond);

            StringBuilder sb = new StringBuilder(oldPassword);
            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                if (!IsValidChar(c))
                {
                    int randomQuantity = rnd.Next(0, 3000);
                    if (randomQuantity >= 0 && randomQuantity < 1000)
                    {
                        sb[i] = (char)rnd.Next('0', '9');
                    }
                    else
                    {
                        if (randomQuantity >= 1000 && randomQuantity < 2000)
                        {
                            sb[i] = (char)rnd.Next('a', 'z');
                        }
                        else
                        {
                            sb[i] = (char)rnd.Next('A', 'Z');
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Check whether character is allowed password character.
        /// </summary>
        /// <param name="c">character to check</param>
        /// <returns>true if character is allowed</returns>
        internal static bool IsValidChar(char c)
        {
            if (((c >= '0') && (c <= '9')) ||
                ((c >= 'a') && (c <= 'z')) ||
                ((c >= 'A') && (c <= 'Z')))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

