namespace KrishiClinic.API.Services
{
    public class OtpService : IOtpService
    {
        public string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<bool> SendOtpAsync(string mobile, string otp)
        {
            // In a real application, integrate with SMS service like Twilio, AWS SNS, etc.
            // For now, just log the OTP (in development)
            Console.WriteLine($"OTP for {mobile}: {otp}");
            
            // Simulate async operation
            await Task.Delay(100);
            return true;
        }

        public bool ValidateOtp(string otp)
        {
            return !string.IsNullOrEmpty(otp) && otp.Length == 6 && otp.All(char.IsDigit);
        }
    }
}
