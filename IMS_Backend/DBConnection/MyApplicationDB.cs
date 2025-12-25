using IMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMS_Backend.DBCommection
{
    public class MyApplicationDB : DbContext
    {
        public MyApplicationDB(DbContextOptions<MyApplicationDB> options) : base(options)
        {
        }
        public DbSet<Products> Products { get; set; }
        public DbSet<ClsUsers> ClsUsers { get; set; }
        public DbSet<Inventory_Transactions> Inventory_Transactions { get; set; }
        public DbSet<StockLvl> stockLvls { get; set; }


    }
}
