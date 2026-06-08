namespace OSSAcademicService.Application.Common;

/// <summary>
/// 通用操作结果
/// </summary>
public class Result
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public int ErrorCode { get; init; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error, int code = 0) => new() { IsSuccess = false, ErrorMessage = error, ErrorCode = code };

    public static Result<T> Success<T>(T data) => Result<T>.Success(data);
    public static Result<T> Failure<T>(string error, int code = 0) => Result<T>.Failure(error, code);
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    public static new Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static new Result<T> Failure(string error, int code = 0) => new() { IsSuccess = false, ErrorMessage = error, ErrorCode = code };
}

/// <summary>
/// 分页结果
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public long TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    public PagedResult(IReadOnlyList<T> items, long totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}