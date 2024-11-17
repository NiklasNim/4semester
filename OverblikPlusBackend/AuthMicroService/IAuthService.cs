namespace AuthMicroService;

public interface IAuthService
{
    Task<string> LoginAsync(LoginDto loginDto);
    Task<RegistrationResult> RegisterAsync(RegisterDto registerDto);
    Task<string> RefreshTokenAsync(string token);
    
}