using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.MeasurementService.Data;
using QuantityMeasurement.MeasurementService.Models;

namespace QuantityMeasurement.MeasurementService.Services;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly MeasurementDbContext _context;

    public MeasurementRepository(MeasurementDbContext context)
    {
        _context = context;
    }

    public async Task<Measurement?> GetByIdAsync(long id) =>
        await _context.Measurements.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Measurement?> GetByMeasurementIdAsync(string measurementId) =>
        await _context
            .Measurements.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MeasurementId == measurementId);

    public async Task<IEnumerable<Measurement>> GetAllAsync() =>
        await _context
            .Measurements.AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<Measurement> AddAsync(Measurement entity)
    {
        await _context.Measurements.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Measurement entity)
    {
        _context.Measurements.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _context.Measurements.FindAsync(id);
        if (entity != null)
        {
            _context.Measurements.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Measurement>> GetByOperationAsync(int operationType) =>
        await _context
            .Measurements.AsNoTracking()
            .Where(m => m.OperationType == operationType)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Measurement>> GetByCategoryAsync(string category) =>
        await _context
            .Measurements.AsNoTracking()
            .Where(m => m.FirstOperandCategory == category || m.SourceOperandCategory == category)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Measurement>> GetByDateRangeAsync(DateTime start, DateTime end) =>
        await _context
            .Measurements.AsNoTracking()
            .Where(m => m.CreatedAt >= start && m.CreatedAt <= end)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Measurement>> GetSuccessfulOperationsAsync() =>
        await _context
            .Measurements.AsNoTracking()
            .Where(m => m.IsSuccessful)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Measurement>> GetFailedOperationsAsync() =>
        await _context
            .Measurements.AsNoTracking()
            .Where(m => !m.IsSuccessful)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync() => await _context.Measurements.CountAsync();

    public async Task<int> GetCountByOperationAsync(int operationType) =>
        await _context.Measurements.CountAsync(m => m.OperationType == operationType);

    public async Task<Dictionary<int, int>> GetOperationCountsAsync()
    {
        var counts = await _context
            .Measurements.GroupBy(m => m.OperationType)
            .Select(g => new { Operation = g.Key, Count = g.Count() })
            .ToListAsync();

        return counts.ToDictionary(x => x.Operation, x => x.Count);
    }
}
