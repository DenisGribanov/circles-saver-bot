using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class DataStore : IDataStore
    {
        private readonly IMyDbContextPoolFactory _contextPoolFactory;

        public DataStore(IMyDbContextPoolFactory contextPoolFactory)
        {
            _contextPoolFactory = contextPoolFactory;
        }

        public async Task<TgMediaFile> AddTgMediaFile(TgMediaFile tgMediaFile)
        {
            using var _context = _contextPoolFactory.GetDbContext();

            var totalAmountByUser = await _context.TgMediaFiles
                        .Where(x => x.OwnerTgUserId == tgMediaFile.OwnerTgUserId)
                        .CountAsync();

            tgMediaFile.Number = totalAmountByUser + 1;
            _context.TgMediaFiles.Add(tgMediaFile);

            await _context.SaveChangesAsync();

            return tgMediaFile;
        }

        public async Task<TgUser> AddTgUser(TgUser tgUser)
        {
            using var _context = _contextPoolFactory.GetDbContext();
            _context.TgUsers.Add(tgUser);
            await _context.SaveChangesAsync();
            return tgUser;
        }

        public async Task<TgMediaFile?> GetTgMediaFile(long chatId, string fileUniqueId)
        {
            using var _context = _contextPoolFactory.GetDbContext();
            return await _context.TgMediaFiles.Where(x => x.OwnerTgUserId == chatId && x.FileUniqueId == fileUniqueId && !x.IsVisable).FirstOrDefaultAsync();
        }

        public async Task<List<TgMediaFile>> GetTgMediaFiles(long ownerUserId)
        {
            using var _context = _contextPoolFactory.GetDbContext();
            return await _context.TgMediaFiles.Where(x => !x.IsDeleted && x.OwnerTgUserId == ownerUserId).ToListAsync();
        }

        public async Task<TgUser?> GetTgUser(long chatId)
        {
            using var _context = _contextPoolFactory.GetDbContext();
            return await _context.TgUsers.FirstOrDefaultAsync(x => x.ChatId == chatId);
        }

        public async Task UpdateTgMediaFile(TgMediaFile tgMediaFile)
        {
            using var _context = _contextPoolFactory.GetDbContext();
            _context.Entry(tgMediaFile).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInlineResultStatistics(InlineResultStatistics stat)
        {
            using var _context = _contextPoolFactory.GetDbContext();

            if (stat.TgMediaFile != null)
            {
                _context.Entry(stat.TgMediaFile).State = EntityState.Detached;
            }

            if (stat.TgUser != null)
            {
                _context.Entry(stat.TgUser).State = EntityState.Detached;
            }

            if (stat.Id == default)
            {
                _context.Entry(stat).State = EntityState.Added;
                _context.InlineResultStatistics.Add(stat);
            }
            else
            {
                _context.Entry(stat).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<InlineResultStatistics?> GetInlineResultStatistics(long chatId, long tgMediaFileId)
        {
            using var _context = _contextPoolFactory.GetDbContext();

            return await _context.InlineResultStatistics
                    .FirstOrDefaultAsync(x => x.TgUserId == chatId && x.TgMediaFileId == tgMediaFileId);
        }

        public async Task SaveUserAction(UserAction userAction)
        {
            using var _context = _contextPoolFactory.GetDbContext();
            _context.UserActions.Add(userAction);
            await _context.SaveChangesAsync();
        }
    }
}