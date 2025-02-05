using Domain.Abstractions;
using Domain.Models.Telegram;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Api.Controllers
{
    [Route("bot")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotService _botService;

        private readonly ILogger<BotController> logger;

        private readonly ITelegramBotClient _telegramClient;

        public BotController(IBotService botService, ILogger<BotController> logger, ITelegramBotClient telegramBotClient)
        {
            _botService = botService;
            this.logger = logger;
            _telegramClient = telegramBotClient;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("сircles-saver-bot");
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            try
            {
                logger.LogInformation("{@update}", Newtonsoft.Json.JsonConvert.SerializeObject(update));
                await _botService.Run(TelegramMessageModel.Init(update));
            }
            catch (Exception e)
            {
                logger.LogError(e, "UpdateError");
            }

            return Ok();
        }

        [HttpPut("webhook")]
        public async Task<IActionResult> WebHook(string url)
        {
            await _telegramClient.SetWebhookAsync(url);
            return Ok();
        }

        [HttpGet("webhook")]
        public async Task<IActionResult> WebHookInfo()
        {
            var result = await _telegramClient.GetWebhookInfoAsync();
            return Ok(result);
        }
    }
}