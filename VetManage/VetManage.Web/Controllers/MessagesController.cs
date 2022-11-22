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

                var messageMessageBoxes = await _messageBoxRepository.GetMessageMessageBoxByUserId(userId);

                var model = _converterHelper.AllToMessageViewModel(inbox, messageMessageBoxes);

                return View(model);
            }

            return new NotFoundViewResult("MessageNotFound");
        }

        public async Task<IActionResult> Outbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var outbox = await _messageBoxRepository.GetOutboxByUserId(userId);

                var model = _converterHelper.AllToMessageViewModelOutbox(outbox);

                return View(model);
            }
            else
            {
                return new NotFoundViewResult("MessageNotFound");
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
                return new NotFoundViewResult("MessageNotFound");
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

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return View(model);
        }


        public async Task<IActionResult> SingleMessage(int messageId)
        {
            var message = await _messageBoxRepository.GetMessageById(messageId);

            if(message == null)
            {
                return new NotFoundViewResult("MessageNotFound");
            }

            // Get the logged in user's MessageBox
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messageBox = await _messageBoxRepository.GetMessageBoxByUserIdAsync(userId);

            // Get the specific messageMessageBox
            var messageMessageBox = await _messageBoxRepository.GetMessageMessageBox(message.Id, messageBox.Id);

            var model = new MessageViewModel();

            IQueryable<Message> messages;
            List<int> messagesIds;
            // If the user is trying to open a message from their outbox
            // messageMessageBox will be null
            if (messageMessageBox != null)
            {
                // Check if message that user is trying to read is their's
                messages = await _messageBoxRepository.GetInboxByUserId(userId);

                messagesIds = messages.Select(messages => messages.Id).ToList();

                if (!messagesIds.Contains(messageId))
                {
                    // TODO: User not allowed to see this message
                    return new NotFoundViewResult("MessageNotFound");
                }

                await _messageBoxRepository.ReadMessage(messageMessageBox);

                model = _converterHelper.ToMessageViewModel(message, messageMessageBox);
            }
            else
            {
                // Check if message that user is trying to read is their's
                messages = await _messageBoxRepository.GetOutboxByUserId(userId);

                messagesIds = messages.Select(messages => messages.Id).ToList();

                if (!messagesIds.Contains(messageId))
                {
                    // TODO: User not allowed to see this message
                    return new NotFoundViewResult("MessageNotFound");
                }

                model = _converterHelper.ToMessageViewModelOutbox(message);
            }

            return View(model);
        }

        public IActionResult MessageNotFound()
        {
            return View();
        }
    }
}
