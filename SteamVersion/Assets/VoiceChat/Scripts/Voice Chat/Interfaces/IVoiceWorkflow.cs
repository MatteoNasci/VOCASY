public interface IVoiceWorkflow
{
    IVoiceChatSettings Settings { get; }
    void Init(IAudioDataManipulator manipulator, IAudioTransportLayer transport);
    void AddVoiceHandler(IVoiceHandler handler);
    void RemoveVoiceHandler(IVoiceHandler handler);
}