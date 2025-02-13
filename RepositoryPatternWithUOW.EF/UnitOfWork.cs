﻿using RepositoryPatternWithUOW.Core;
using RepositoryPatternWithUOW.Core.Interfaces;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternWithUOW.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        public IBaseRepository<Author> Authors { get; private set; }
        public IBooksRepository Books { get; private set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;

            Authors = new BaseRepository<Author>(_context);
            Books = new BooksRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
         
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
