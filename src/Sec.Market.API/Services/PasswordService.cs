using Microsoft.AspNetCore.Identity;
using Sec.Market.API.Entites;

namespace Sec.Market.API.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        // Méthode pour hacher le mot de passe avant de sauvegarder l'utilisateur
        public string HashPassword(User user)
        {
            return _passwordHasher.HashPassword(user, user.Password);
        }

        // Méthode pour vérifier le mot de passe
        public bool VerifyPassword(User user, string password)
        {
            if (user == null || string.IsNullOrEmpty(password))
                return false;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return verificationResult == PasswordVerificationResult.Success;
        }
    }
}
