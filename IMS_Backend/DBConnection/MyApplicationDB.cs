using IMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMS_Backend.DBCommection
{
    public class MyApplicationDB : DbContext
    {
        public MyApplicationDB(DbContextOptions<MyApplicationDB> options) : base(options)
        {
        }
        //DbSet<used in c# code> and this is used in sql for DB name

        public DbSet<Products> Products { get; set; }
        public DbSet<ClsUsers> ClsUsers { get; set; }
        public DbSet<StockLvl> stockLvls { get; set; }
        public DbSet <ProductCategory> ProdCategory { get; set; }
        public DbSet<Transactions> transactions { get; set; }


    }
}
