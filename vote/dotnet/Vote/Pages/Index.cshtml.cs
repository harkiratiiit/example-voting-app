using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vote.Messaging;
using Vote.Messaging.Messages;

namespace Vote.Pages
{
    public class IndexModel : PageModel
    {
        private string _optionA;
        private string _optionB;

        private string _optionC;

        protected readonly IMessageQueue _messageQueue;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;

        public IndexModel(IMessageQueue messageQueue, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _messageQueue = messageQueue;
            _configuration = configuration;
            _logger = logger;

            _optionA = _configuration.GetValue<string>("Voting:OptionA");
            _optionB = _configuration.GetValue<string>("Voting:OptionB");
            _optionC = _configuration.GetValue<string>("Voting:OptionC");
        }

        public string OptionA { get; private set; }

        public string OptionB { get; private set; }

        public string OptionC { get; private set; }

        [BindProperty]
        public string Vote { get; private set; }

        private string _voterId 
        {
            get { return TempData.Peek("VoterId") as string; }
            set { TempData["VoterId"] = value; }
        }

        public void OnGet()
        {
            OptionA = _optionA;
            OptionB = _optionB;
            OptionC= _optionC;
        }

        public IActionResult OnPost(string vote)
        {
            Vote = vote;
            OptionA = _optionA;
            OptionB = _optionB;
            OptionC= _optionC;
            if (_configuration.GetValue<bool>("MessageQueue:Enabled"))
            {
                PublishVote(vote);
            }
            return Page();
        }

        private void PublishVote(string vote)
        {
            if (string.IsNullOrEmpty(_voterId))
            {
                _voterId = Guid.NewGuid().ToString();
            }
            var message = new VoteCastEvent
            {
                VoterId = _voterId,
                Vote = vote
            };
           _messageQueue.Publish(message);
        }
    }
}
