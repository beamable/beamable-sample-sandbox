using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Api.Mail;
using Beamable.Common.Inventory;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        private async Task SendEmail(long userId)
        {
            var mail = new MailSendEntry()
            {
                senderGamerTag = 0,
                receiverGamerTag = userId,
                body = "you received a reward",
                subject = "Reward Notification",
                category = "daily_login_reward",
                rewards = new MailRewards()
                {
                    applyVipBonus = false,
                    currencies = new List<CurrencyChange>
                    {
                        // Example: Add currency rewards
                        new CurrencyChange {amount = 100, symbol = "currency.gems"},
                    },

                    items = new List<ItemCreateRequest>(),
                }
            };

            var mailRequest = new MailSendRequest();
            mailRequest.Add(mail);

            try
            {
                await Services.Mail.SendMail(mailRequest);
                Debug.Log($"Email sent to user {userId}!");
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to send email to user {userId}: {e.Message}");
            }
        }
        
        [ClientCallable]
        public async Task<List<string>> CollectAllMailAndReturnRewards(long userId)
        {
            var rewardsList = new List<string>();

            try
            {
                var mailService = Services.Mail;
                var mailResponse = await mailService.GetMail("daily_login_reward");

                if (mailResponse.result.Count == 0)
                {
                    Debug.Log($"No mails found for user {userId}.");
                    return rewardsList; // Return empty list if no mails
                }

                foreach (var mailMessage in mailResponse.result)
                {
                    // Extract rewards from mail
                    var rewards = mailMessage.rewards;
                    if (rewards != null)
                    {
                        rewardsList.AddRange(rewards.currencies.Select(currency => $"Currency: {currency.symbol}, Amount: {currency.amount}"));
                    }

                    // Mark mail as read and collected
                    var mailUpdateRequest = new MailUpdateRequest();
                    mailUpdateRequest.Add(mailMessage.id, MailState.Read, true, mailMessage.expires);
                    await mailService.Update(mailUpdateRequest);

                    Debug.Log($"Mail ID {mailMessage.id} processed for user {userId}.");
                }

                Debug.Log($"All mails collected for user {userId}. Total rewards: {rewardsList.Count}");
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to collect mails for user {userId}: {e.Message}");
            }

            return rewardsList;
        }
    }
}