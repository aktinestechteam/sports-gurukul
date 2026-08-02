namespace SportsGurukul.Domain.Enums.AI;

[Flags]
public enum AIModelCapability
{
    TextGeneration = 1,
    CodeGeneration = 2,
    ImageGeneration = 4,
    ImageAnalysis = 8,
    AudioTranscription = 16,
    Embedding = 32,
    Reasoning = 64,
    FunctionCalling = 128,
    Vision = 256
}
