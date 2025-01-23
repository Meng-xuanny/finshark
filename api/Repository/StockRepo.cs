using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helppers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepo : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        public StockRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks=query.IsDecsending? stocks.OrderByDescending(s=>s.Symbol) : stocks.OrderBy(s=>s.Symbol);
                }
            }

            var skipNumber=(query.PageNumber-1) * query.PageSize;//if page number is 1 skips no records; if page number is 2 skips first 20 records

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync(); //returns all stocks if no query's given
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context
                .Stocks.Include(c => c.Comments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // public Task<Stock?> GetBySymbolAsync(string symbol)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockDto.Symbol;
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.Industry = stockDto.Industry;
            existingStock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();
            return existingStock;
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> PatchAsync(int id, StockPatchDto patchDto)
        {
            var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            // Update only the fields provided in the patch DTO
            if (patchDto.Purchase.HasValue)
            {
                existingStock.Purchase = patchDto.Purchase.Value;
            }

            if (!string.IsNullOrWhiteSpace(patchDto.CompanyName))
            {
                existingStock.CompanyName = patchDto.CompanyName;
            }

            await _context.SaveChangesAsync();
            return existingStock;
        }
    }
}
