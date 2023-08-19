﻿using Core.Data;
using Core.DataAccess.Repository.IRepository;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository 
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Category category)
        {
            _db.Update(category);
        }
    }
}
