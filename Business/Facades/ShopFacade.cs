using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SUShirts.Business.Dto;
using SUShirts.Business.Models;
using SUShirts.Business.Services;
using SUShirts.Configuration;
using SUShirts.Data;
using SUShirts.Data.Entities;
using SUShirts.Data.Enums;

namespace SUShirts.Business.Facades
{
    public class ShopFacade
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptionsMonitor<MessageOptions> _messageOptions;
        private readonly ILogger<ShopFacade> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IEmailService _emailService;

        public ShopFacade(AppDbContext dbContext, IMapper mapper, IHttpClientFactory httpClientFactory,
            IOptionsMonitor<MessageOptions> messageOptions, ILogger<ShopFacade> logger,
            IHttpContextAccessor contextAccessor, LinkGenerator linkGenerator, IEmailService emailService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _messageOptions = messageOptions;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _linkGenerator = linkGenerator;
            _emailService = emailService;
        }

        public async Task<(List<ShirtDto> Men, List<ShirtDto> Women)> GetOffers()
        {
            var shirts = await _dbContext.Shirts
                .Include(s => s.Variants)
                .Where(s => s.Variants.Any(v => v.ItemsLeft > 0))
                .OrderBy(s => s.Name)
                .ToListAsync();

            var men = this.GetOffersFor(SexVariant.Man, shirts);
            var women = this.GetOffersFor(SexVariant.Woman, shirts);

            return (men, women);
        }

        private List<ShirtDto> GetOffersFor(SexVariant sex, List<Shirt> shirtEntities)
        {
            var mapped = _mapper.Map<List<ShirtDto>>(shirtEntities);
            var ret = new List<ShirtDto>();

            foreach (var shirtDto in mapped)
            {
                shirtDto.Variants.RemoveAll(variant => (variant.Sex != sex && variant.Sex != SexVariant.Unisex)
                                                       || variant.ItemsLeft == 0);

                if (shirtDto.Variants.Count == 0)
                {
                    continue;
                }

                shirtDto.Variants.Sort((a, b) => (int) a.Size - (int) b.Size);
                ret.Add(shirtDto);
            }

            return ret;
        }

        public async Task<List<ShirtVariantRequestDto>> GetCartItems(string cartJson)
        {
            if (string.IsNullOrEmpty(cartJson))
                return null;

            Dictionary<string, string> cartObject;

            try
            {
                cartObject = JsonSerializer.Deserialize<Dictionary<string, string>>(cartJson);

                if (cartObject is null)
                    return null;
            }
            catch (JsonException)
            {
                return null;
            }

            var keys = cartObject.Keys.Select(k => int.Parse(k)).ToList();

            var variantEntitiesQuery = _dbContext.ShirtVariants
                .Where(sv => keys.Contains(sv.Id))
                .Include(sv => sv.Shirt);

            var variantEntitiesDto = await _mapper.ProjectTo<ShirtVariantRequestDto>(variantEntitiesQuery)
                .ToListAsync();

            foreach (var cartItem in cartObject)
            {
                var id = int.Parse(cartItem.Key);
                var count = int.Parse(cartItem.Value);

                var dto = variantEntitiesDto.Find(v => v.Id == id);

                if (dto is null)
                {
                    continue;
                }

                dto.RequestedCount = count;
                dto.WillBeRequested = count <= dto.ItemsLeft;
            }

            return variantEntitiesDto;
        }

        public async Task<bool> MakeReservation(ReservationModel model, string cartJson)
        {
            var cart = await this.GetCartItems(cartJson);
            if (cart is null)
            {
                return false;
            }

            var items = cart.Where(c => c.WillBeRequested)
                .Select(c => new ReservationItem()
                {
                    Count = c.RequestedCount,
                    Price = c.Price,
                    ShirtVariantId = c.Id
                }).ToList();

            var newEntity = new Reservation()
            {
                Name = model.Name,
                Email = model.Email,
                Note = HttpUtility.HtmlEncode(model.Note +
                                              (model.IsClubMember ? "\nRezervující je sympatizujícím členem SU." : "")
                                              .Trim()),
                PhoneOrDiscordTag = model.PhoneOrDiscordTag,
                Items = items,
                MadeOn = DateTime.Now
            };

            foreach (var reservationItem in items)
            {
                var variantEntity = await _dbContext.ShirtVariants.FindAsync(reservationItem.ShirtVariantId);
                variantEntity.ItemsLeft -= reservationItem.Count;
            }

            _dbContext.Reservations.Add(newEntity);

            try
            {
                await _dbContext.SaveChangesAsync();

                newEntity = await _dbContext.Reservations.Include(r => r.Items)
                    .ThenInclude(ri => ri.ShirtVariant)
                    .ThenInclude(sv => sv.Shirt)
                    .ThenInclude(s => s.PrimaryColor)
                    .FirstOrDefaultAsync(ri => ri.Id == newEntity.Id);

                var url = _linkGenerator.GetUriByPage(_contextAccessor.HttpContext,
                    "/Admin/ReservationDetail", values: new {id = newEntity.Id});

                _ = Task.Run(async () =>
                {
                    if (!await this.SendDiscordNotification(newEntity, url).ConfigureAwait(false))
                    {
                        await _emailService.SendReservationEmailToManager(url).ConfigureAwait(false);
                    }

                    await _emailService.SendReservationEmailToUser(newEntity).ConfigureAwait(false);
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> SendDiscordNotification(Reservation reservation, string reservationUrl)
        {
            if (string.IsNullOrEmpty(_messageOptions.CurrentValue.DiscordWebhookUrl))
            {
                _logger.LogWarning("No Discord webhook url set, cannot send a notification.");
                return false;
            }

            if (!Uri.TryCreate(_messageOptions.CurrentValue.DiscordWebhookUrl, UriKind.RelativeOrAbsolute,
                out var address))
            {
                _logger.LogWarning("Invalid Discord webhook url set, cannot send a notification.");
                return false;
            }

            var msg =
                $"Nová rezervace triček – {reservation.Name}, {reservation.Items.Sum(i => i.Count)} ks. Odkaz na rezervaci: {reservationUrl}"
                    .Replace("\"", "\\\"");
            var jsonMsg = $"{{ \"content\": \"{msg}\" }}";

            using var client = _httpClientFactory.CreateClient();

            try
            {
                var resp = await client.PostAsync(address,
                    new StringContent(jsonMsg, Encoding.UTF8, "application/json")).ConfigureAwait(false);

                resp.EnsureSuccessStatusCode();
                _logger.LogInformation("Discord notification sent.");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot send Discord notification.");
                return false;
            }
        }
    }
}
