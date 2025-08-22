namespace Authorization.Public.Dtos;


public record AuthorizationTokens(
string AccessToken,
DateTime AccessTokenExpiresAt,
string RefreshToken,
DateTime RefreshTokenExpiresAt,
Guid Jti);
