
namespace ManagedDnsQuery.SPF.Interfaces
{
    public enum SpfResult
    {
        NoResult = 0,
        Pass = 1,
        Fail = 2,
        SoftFail = 3,
    }

    public interface ISpfChecker
    {
        SpfResult VerifySpfRecord(string domain, string ip);
    }
}
