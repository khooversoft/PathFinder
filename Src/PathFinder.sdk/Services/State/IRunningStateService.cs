namespace PathFinder.sdk.Services.State
{
    public interface IRunningStateService
    {
        RunningState ServiceState { get; }

        void SetState(RunningState stateType);
    }
}