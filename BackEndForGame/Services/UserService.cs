using BackEndForGame.Configuration;
using BackEndForGame.Contracts;
using BackEndForGame.Entities.DataBaseEntities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace BackEndForGame.Services
{
    public class UserService
    {
        private OrbitiumDataBaseContext _context;
        private PlayerService _playerSevice;
        private JwtService _jwtService;
        public UserService(OrbitiumDataBaseContext context)
        {
            _context = context;
            _playerSevice = _context.GetService<PlayerService>();
            _jwtService = _context.GetService<JwtService>();
        }

        public JwtToken? Login(LoginData data)
        {
            Guid? u = _context.Set<Users>().Where(x => x.login == data.login &&
            x.password == CreateHashCode(data.password) ).
            ToList().FirstOrDefault()?.uid;

            if (u == null)
                return null;

            return new JwtToken() { Token = _jwtService.GenerateToken( (Guid)u ) };
        }

        public Guid CreateUser(RegisterData Data)
        {
            Users? log = _context.
                Set<Users>().
                Include(x => x.player).
                Where(x => x.login == Data.Login || x.player.nick_name == Data.Nick).
                ToList().FirstOrDefault();

            if (log != null)
                return Guid.Empty;

            Users _user = new Users()
            {
                uid = Guid.NewGuid(),
                login = Data.Login,
                password = CreateHashCode( Data.Password )
            };

            _context.Users.Add(_user);
            _context.SaveChanges();

            Users? _temp = _context.Users.Include(x => x.player).
                Where(y => y.login == Data.Login).
                ToList().FirstOrDefault();

            _playerSevice.CreatePlayer(Data);
            
            return _user.uid;
        }

        public bool DeleteUser(Guid uid)
        {
            Users? _user = _context.
                Set<Users>().
                Include(x => x.player).
                Include(x => x.player.inventory).
                Where(y => y.uid == uid).
                FirstOrDefault();                   

            if (_user == null)
                return false;

            _context.Users.Remove(_user);

            _context.SaveChanges();

            return true;
        }

        private static byte[] CreateHashCode(string str)
        {          
            var a = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(str));
            return a;
        }
    }
}