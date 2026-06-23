using FCE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FCE.Infrastructure
{
    public class UnitOfWork
    {
        private readonly Context _context;
        IDbContextTransaction _transaction;
        short _depth = 0;
        string _savePointName = string.Empty;

        public UnitOfWork(Context context)
        {
            _context = context;
        }

        public async Task CreateSavePoint(string SavePointName, CancellationToken cs)
        {
            _savePointName = SavePointName;
            if (_transaction is null)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cs);
            }
            await _transaction.CreateSavepointAsync(SavePointName, cs);
        }

        public async Task ExecuteAsync(Func<Task> action, CancellationToken cs)
        {
            if (_transaction is null)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cs);
            }
            _depth++;
            try
            {
                await action();
                await _context.SaveChangesAsync(cs);
                if (_depth == 1)
                    await _transaction.CommitAsync(cs);
            }
            catch
            {
                if (!string.IsNullOrEmpty(_savePointName))
                    await _transaction.RollbackToSavepointAsync(_savePointName, cs);
                else
                    await _transaction.RollbackAsync(cs);
                throw;
            }
            finally
            {
                _depth--;
                if (_depth == 0 && _transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }  
    }
}
