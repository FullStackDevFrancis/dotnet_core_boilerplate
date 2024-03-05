namespace dotnet_core_boilerplate.Dtos;

public class AuthResponseDto
{
    public string? Token { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}