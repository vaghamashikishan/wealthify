using System;

namespace wealthify.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}