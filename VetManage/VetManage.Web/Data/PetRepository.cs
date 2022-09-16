﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {
        private readonly DataContext _context;

        public PetRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithOwners()
        {
            return _context.Pets.Include(p => p.Owner);
        }
    }
}
