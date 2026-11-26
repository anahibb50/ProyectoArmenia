namespace LigaArmenia.Application.Interfaces.Security;

public interface ILoginAttemptTracker
{
    bool IsLockedOut(string key, out TimeSpan remaining);
    void RecordFailure(string key);
    void Reset(string key);
}
