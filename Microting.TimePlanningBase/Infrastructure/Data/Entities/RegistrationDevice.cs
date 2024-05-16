using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

public class RegistrationDevice : PnBase
{
    public string Token { get; set; }
    public string SoftwareVersion { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public string OsVersion { get; set; }
    public string LastIp { get; set; }
    public string LastKnownLocation { get; set; }
    public string LookedUpIp { get; set; }
    public string OtpCode { get; set; }
    public bool OtpEnabled { get; set; }

    public async Task GenerateOtp(TimePlanningPnDbContext dbContext)
    {
        Random random = new Random();
        while (dbContext.RegistrationDevices.Any(x => x.OtpCode == OtpCode))
        {
            OtpCode = (100000 + random.Next(899999)).ToString();
        }

        OtpEnabled = true;

        await Update(dbContext);
    }

    public async Task GenerateToken(TimePlanningPnDbContext dbContext)
    {
        Random random = new Random();
        Console.WriteLine($"Unit.GenerateToken: Token request, current token {Token}");
        while (dbContext.RegistrationDevices.Any(x => x.Token == Token))
        {
            const string allowedChars = "abcdef0123456789";
            char[] chars = new char[32];

            for (int i = 0; i < 32; i++)
            {
                chars[i] = allowedChars[random.Next(0, allowedChars.Length)];
            }

            Console.WriteLine($"Unit.GenerateToken: Token with token {Token}");
            Token = new string(chars);
        }

        Console.WriteLine($"Unit.GenerateToken: Token set to {Token}");

        await Update(dbContext);
    }
}