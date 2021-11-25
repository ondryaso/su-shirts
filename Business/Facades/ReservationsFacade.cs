using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SUShirts.Business.Dto;
using SUShirts.Data;
using SUShirts.Data.Entities;
using SUShirts.Data.Enums;
using SUShirts.Data.Extensions;

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

        public async Task<List<ReservationDto>> GetAll()
        {
            var query = _dbContext.Reservations
                .Include(r => r.Items);

            var dto = _mapper.ProjectTo<ReservationDto>(query);
            return await dto.ToListAsync();
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

            if (reservation.State.IsClosed())
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

            reservation.State = ReservationState.Finished;
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

        public async Task<bool> SetInternalInfo(int reservationId, string assignedTo, string internalNote)
        {
            var reservation = await _dbContext.Reservations
                .Where(r => r.Id == reservationId)
                .Include(r => r.Items)
                .ThenInclude(ri => ri.ShirtVariant)
                .FirstOrDefaultAsync();

            if (reservation.State.IsClosed())
            {
                return false;
            }

            reservation.State = string.IsNullOrEmpty(assignedTo) ? ReservationState.New : ReservationState.Assigned;
            reservation.AssignedTo = assignedTo;
            reservation.InternalNote = internalNote;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot change state of reservation {Id}.", reservationId);
                return false;
            }

            return true;
        }

        public async Task<bool> Cancel(int reservationId, string handledBy)
        {
            var reservation = await _dbContext.Reservations
                .Where(r => r.Id == reservationId)
                .Include(r => r.Items)
                .ThenInclude(ri => ri.ShirtVariant)
                .FirstOrDefaultAsync();

            if (reservation.State.IsClosed())
            {
                return false;
            }

            foreach (var reservationItem in reservation.Items)
            {
                reservationItem.ShirtVariant.ItemsLeft += reservationItem.Count;
                reservationItem.Count = 0;

                if (reservationItem.ShirtVariant.ItemsInStock < reservationItem.ShirtVariant.ItemsLeft)
                {
                    _logger.LogCritical(
                        "Data inconsistency: shirt variant {Id} has less items in stock than items left for reservations.",
                        reservationItem.ShirtVariantId);

                    reservationItem.ShirtVariant.ItemsInStock = reservationItem.ShirtVariant.ItemsLeft;
                }
            }

            reservation.State = ReservationState.Cancelled;
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
                .Where(r => r.Id == reservationId && !r.State.IsClosed())
                .Include(r => r.Items)
                .ThenInclude(ri => ri.ShirtVariant)
                .FirstOrDefaultAsync(r => r.Items.Any(ri => ri.ShirtVariantId == variantId));

            var item = reservation?.Items?.FirstOrDefault(ri => ri.ShirtVariantId == variantId);
            return item;
        }
    }
}
