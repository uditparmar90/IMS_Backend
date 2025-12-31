using IMS_Backend.DBCommection;

namespace IMS_Backend.Services
{
    public class AuthServices
    {
        readonly MyApplicationDB _context;
        public AuthServices(MyApplicationDB context)
        {
            _context = context;
        }


    }
}
