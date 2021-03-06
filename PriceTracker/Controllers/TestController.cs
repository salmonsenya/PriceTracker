﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceTracker.Repositories;
using PriceTracker.Services;
using System;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private readonly IShopService _pullAndBearService;
        private readonly ITrackingRepository _trackingRepository; 

        public TestController(IShopService pullAndBearService, ITrackingRepository trackingRepository)
        {
            _pullAndBearService = pullAndBearService ?? throw new ArgumentNullException(nameof(pullAndBearService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
        }

        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            return Ok("PriceTracker is active.");
        }

        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> Items()
        {
            var items = await _trackingRepository.GetItemsAsync();
            return Ok(JsonConvert.SerializeObject(items, Formatting.Indented));
        }

        [HttpGet]
        [Route("statuses")]
        public async Task<IActionResult> Statuses()
        {
            var statuses = await _trackingRepository.GetUserStatusesAsync();
            return Ok(JsonConvert.SerializeObject(statuses, Formatting.Indented));
        }

        // setwaitingforadd?userId=158875852&value=false
        [HttpGet]
        [Route("setwaitingforadd")]
        public async Task<IActionResult> SetWaitingForAdd(int userId, bool value)
        {
            await _trackingRepository.SetWaitingForAddAsync(userId, value);
            var statuses = await _trackingRepository.GetUserStatusesAsync();
            var status = statuses.Find(x => x.UserId == userId);
            return Ok(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
    }
}
