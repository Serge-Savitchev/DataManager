﻿namespace DataManagerAPI.Helpers;

public class ResultWrapper<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
}