using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sec.Market.API.Data;
using Sec.Market.API.Entites;
using Sec.Market.API.Interfaces;
using Sec.Market.API.Services;

namespace Sec.Market.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;

        public UsersController(IUserRepository userRepository, PasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
         
            return await _userRepository.GetUsers();
        }

        // GET: api/Users/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<User>> GetUser(int id)
        //{
        //    var user = await _userRepository.GetUserById(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        [HttpPost("GetUser")]
        public async Task<ActionResult<User>> GetUser([FromBody] UserLoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Les informations d'identification sont invalides.");
            }

            var user = await _userRepository.GetUserByEmail(model.Email);

            if (user == null)
            {
                return NotFound();
            }
            // Vérification du mot de passe
            if (!_passwordService.VerifyPassword(user, model.Password))
            {
                return BadRequest("Mot de passe incorrect.");
            }

            return user;
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser(string email, string pwd)
        {
            var user = await _userRepository.GetUserByEmailAndPwd(email, pwd);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Si un nouveau mot de passe est fourni, hachez-le avant de l'enregistrer
            if (!string.IsNullOrEmpty(user.Password) && user.Password != existingUser.Password)
            {
                user.Password = _passwordService.HashPassword(user); // Hachage du nouveau mot de passe
            }
            else
            {
                // Si le mot de passe n'est pas fourni, vous pouvez conserver l'ancien mot de passe.
                user.Password = existingUser.Password;
            }
            await _userRepository.UpdateUser(user);

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Vérifier si l'email existe déjà
            var existingUser = await _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return BadRequest("Un utilisateur avec cet email existe déjà.");
            }

            // Hachage du mot de passe avant d'enregistrer l'utilisateur
            user.Password = _passwordService.HashPassword(user);
            await _userRepository.InsertUser(user);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
           
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUser(user);
          

            return NoContent();
        }

       
    }
}
