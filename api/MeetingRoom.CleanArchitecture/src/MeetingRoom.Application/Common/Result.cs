namespace MeetingRoom.Application.Common;

/// <summary>
/// 操作结果类
/// 用于封装应用层操作的结果，包括成功/失败状态、错误信息等
/// </summary>
/// <typeparam name="T">结果数据类型</typeparam>
public class Result<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// 结果数据
    /// </summary>
    public T? Data { get; }
    
    /// <summary>
    /// 错误代码
    /// </summary>
    public int? ErrorCode { get; }
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private Result(bool isSuccess, T? data, string? error, int? errorCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <param name="data">结果数据</param>
    /// <returns>成功结果</returns>
    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null, null);
    }
    
    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败结果</returns>
    public static Result<T> Failure(string error, int? errorCode = null)
    {
        return new Result<T>(false, default, error, errorCode);
    }
    
    /// <summary>
    /// 从另一个结果创建结果
    /// </summary>
    /// <typeparam name="TSource">源结果数据类型</typeparam>
    /// <param name="result">源结果</param>
    /// <param name="data">新结果数据</param>
    /// <returns>新结果</returns>
    public static Result<T> FromResult<TSource>(Result<TSource> result, T? data = default)
    {
        return result.IsSuccess
            ? Success(data!)
            : Failure(result.Error!, result.ErrorCode);
    }
}

/// <summary>
/// 无数据的操作结果类
/// </summary>
public class Result
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// 错误代码
    /// </summary>
    public int? ErrorCode { get; }
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private Result(bool isSuccess, string? error, int? errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <returns>成功结果</returns>
    public static Result Success()
    {
        return new Result(true, null, null);
    }
    
    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败结果</returns>
    public static Result Failure(string error, int? errorCode = null)
    {
        return new Result(false, error, errorCode);
    }
    
    /// <summary>
    /// 从带数据的结果创建无数据结果
    /// </summary>
    /// <typeparam name="TSource">源结果数据类型</typeparam>
    /// <param name="result">源结果</param>
    /// <returns>无数据结果</returns>
    public static Result FromResult<TSource>(Result<TSource> result)
    {
        return result.IsSuccess
            ? Success()
            : Failure(result.Error!, result.ErrorCode);
    }
}
