using IMS_Backend.DBCommection;

namespace IMS_Backend.Services
{
    public class ProductService
    {
        public readonly MyApplicationDB _context;
        //public IConfiguration _config;
        public ProductService(MyApplicationDB context)
        {
            _context = context;
            //_config = config;
        }



    }
}
