using Core.Data;
using Core.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; set; }
        public IShoppingCartRepository ShoppingCart { get; set; }
        public IApplicationUserRepository ApplicationUser { get; set; }
        public IOrderDetailRepository OrderDetail { get; set; }
        public IOrderHeaderRepository OrderHeader { get; set; }

        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);

        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
