using System.Net.NetworkInformation;

namespace LuxeLookAPI.DTO;

public class ResponseDTO
{
    public string? Message { get; set; }
    public APIStatus Status { get; set; }
    public object? Data { get; set; }
}
public enum APIStatus
{
    Successful = 0,
    Error = 1,
    SystemError = 2
}
public static class Messages
{
    public const string Successfully = "Successfully";
    public const string UpdateSucess = "Update Successfully";
    public const string DeleteSucess = "Delete Successfully";
    public const string AddSucess = "Add Successfully";
    public const string ErrorWhileFetchingData = "Error while fetching data.";
    public const string InvalidPostedData = "Posted invalid data.";
    public const string NoData = "No data was found";
    public const string Result = "Result Found!";
    public const string UpdateFail = "Updated Fail!";
}
