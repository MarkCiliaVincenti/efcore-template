﻿using EfCoreProject.Models;

namespace EfCoreProject.Services
{
    public interface IProductService
    {
        Task Add(Product product);
        Task<IEnumerable<Product>> GetList();
        ValueTask<Product> GetById(int id);
        Task Update(Product entity);
        Task Delete(Product entity);
    }
}
