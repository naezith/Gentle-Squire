using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Flurl.Http;
using GentleSquire;

namespace GentleSquire
{
    public class Commands
    {
        public static string SpeedRunPostLink = "http://ron.naezith.com/fetchSpeedrunLB";
        [Command("hi")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
        }
    }
}
