namespace BlImplementation;
using BlApi;
using BO;
using Helpers;

internal class AdminImplementation : IAdmin
{
    public bool AuthenticateManager(int managerId, string password)
    {
        PasswordValidator.ValidateOrThrow(password);

        // currently stored as plaintext (hashing will be applied later)
        return DalApi.Factory.Get.Config.Managers.TryGetValue(managerId, out var storedPwd)
            && storedPwd == password;
    }

    public void ForwardClock(int requesterId, TimeUnit timeUnit)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        _ = AdminManager.GetConfig(requesterId);

        switch (timeUnit)
        {
            case TimeUnit.Month:
                AdminManager.UpdateClock(AdminManager.Now.AddSeconds(1));
                break;
            case TimeUnit.Minutes:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case TimeUnit.Hours:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case TimeUnit.Days:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case TimeUnit.Years:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
        }
    }

    public DateTime GetClock(int requesterId)
    {
        _ = AdminManager.GetConfig(requesterId);
        return AdminManager.Now;
    }

    public Config GetConfig(int requesterId)
    {
        return AdminManager.GetConfig(requesterId);
    }

    public void InitializeDB(int requesterId)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.InitializeDB(requesterId);
    }

    public void ResetDB(int requesterId)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.ResetDB(requesterId);
    }

    public void SetConfig(int requesterId, Config config)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.SetConfig(requesterId, config);
    }

    #region Stage 7
    public void StartSimulator(int requesterId, int intervalMinutes)
    {
        _ = AdminManager.GetConfig(requesterId);
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.Start(intervalMinutes); //stage 7
    }

    public void StopSimulator(int requesterId)
    {
        _ = AdminManager.GetConfig(requesterId);
        AdminManager.Stop(); //stage 7
    }
    public void ResetClock(string requesterId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.ResetClock();
    }

    #endregion Stage 7

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;

    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;

    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;

    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
}
