﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.ComponentModel.DataAnnotations;
using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Rules.Action.Slack
{
    [RuleActionHandler(typeof(SlackActionHandler))]
    [RuleAction(Link = "https://slack.com",
        Display = "Send to Slack",
        Description = "Create a status update at slack to a channel you define.")]
    public sealed class SlackAction : RuleAction
    {
        [AbsoluteUrl]
        [Required]
        [Display(Name = "Webhook Url", Description = "The slack webhook url.")]
        public Uri WebhookUrl { get; set; }

        [Required]
        [Display(Name = "Text", Description = "The text that is sent as message to slack.")]
        public string Text { get; set; }
    }
}
