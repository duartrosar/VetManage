using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Messages;

namespace VetManage.Web.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IMessageBoxRepository _messageBoxRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public MessagesController(
            IMessageBoxRepository messageBoxRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper)
        {
            _messageBoxRepository = messageBoxRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Inbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var inbox = await _messageBoxRepository.GetInboxByUserId(userId);

                var messages = inbox.ToList();

                return View(messages);
            } else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Outbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var outbox = await _messageBoxRepository.GetOutboxByUserId(userId);

                return View(outbox);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId != null)
            {
                var messageBox = await _messageBoxRepository.GetMessageBoxByUserIdAsync(userId);

                var recipients = await _messageBoxRepository.GetRecipientsAsync(messageBox.Id);

                //var recipientsColleciton = recipients.ToList();

                ViewData["Recipients"] = recipients;

                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var messageBox = await _messageBoxRepository.GetMessageBoxByUserIdAsync(userId);

                    int[] recipientsIds = Array.ConvertAll(model.Recipients, int.Parse);

                    var recipients = _messageBoxRepository.GetSelectedRecipients(recipientsIds).ToList();

                    model.RecipientsList = recipients;

                    var message = _converterHelper.ToMessage(model);

                    message.SenderId = messageBox.Id;
                    message.SenderUsername = messageBox.Username;

                    await _messageBoxRepository.SendMessage(message, recipients);

                    return RedirectToAction(nameof(Inbox));
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return View(model);
        }
    }
}
