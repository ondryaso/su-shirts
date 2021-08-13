using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SUShirts.Business.Dto;
using SUShirts.Data;
using SUShirts.Data.Entities;

namespace SUShirts.Business.Facades
{
    public class ReservationsFacade
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationsFacade> _logger;

        public ReservationsFacade(AppDbContext dbContext, IMapper mapper,
            ILogger<ReservationsFacade> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReservationDto> GetDetails(int id)
        {
            var query = _dbContext.Reservations
                .Where(r => r.Id == id)
                .Include(r => r.Items)
                .ThenInclude(ri => ri.ShirtVariant)
                .ThenInclude(sv => sv.Shirt);

            var dto = _mapper.ProjectTo<ReservationDto>(query);
            return await dto.FirstOrDefaultAsync();
        }

        public async Task<bool> MarkHandled(int reservationId, string handledBy)
        {
            var reservation = await _dbContext.Reservations
                .Where(r => r.Id == reservationId)
                .Include(r => r.Items)
                .ThenInclude(ri => ri.ShirtVariant)
                .FirstOrDefaultAsync();

            if (reservation.Handled)
            {
                return false;
            }

            foreach (var reservationItem in reservation.Items)
            {
                reservationItem.ShirtVariant.ItemsInStock -= reservationItem.Count;

                if (reservationItem.ShirtVariant.ItemsInStock < 0)
                {
                    _logger.LogCritical("Data inconsistency: shirt variant {Id} has less than zero items in stock.",
                        reservationItem.ShirtVariantId);
                }
            }

            reservation.Handled = true;
            reservation.HandledBy = handledBy;
            reservation.HandledOn = DateTime.Now;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot finish reservation {Id}.", reservationId);
                return false;
            }

            return true;
        }

        public async Task<bool> AddItem(int reservationId, int variantId)
        {
            var item = await this.GetReservationItem(reservationId, variantId);

            if (item is null)
            {
                return false;
            }

            if (item.ShirtVariant.ItemsLeft - 1 < 0)
            {
                return false;
            }

            item.Count++;
            item.ShirtVariant.ItemsLeft--;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update reservation item count.");
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveItem(int reservationId, int variantId)
        {
            var item = await this.GetReservationItem(reservationId, variantId);
            if (item is null)
            {
                return false;
            }

            if (item.Count <= 0)
            {
                return false;
            }

            item.Count--;
            item.ShirtVariant.ItemsLeft++;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update reservation item count.");
                return false;
            }

            return true;
        }

        private async Task<ReservationItem> GetReservationItem(int reservationId, int variantId)
        {
            var reservation = await _dbContext.Reservations
                .Where(r => r.Id == reservationId && !r.Handled)
                .Include(r => r.Items)
                .ThenInclude(ri => ri.ShirtVariant)
                .FirstOrDefaultAsync(r => r.Items.Any(ri => ri.ShirtVariantId == variantId));

            var item = reservation?.Items?.FirstOrDefault(ri => ri.ShirtVariantId == variantId);
            return item;
        }
    }
}
