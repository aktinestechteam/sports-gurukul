namespace AI.IntegrationTests.Fixtures;

public static class AITestIds
{
    public static readonly Guid SuperAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid CoachRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid AthleteRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static readonly Guid AdminUserId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static readonly Guid CoachUserId = Guid.Parse("88888888-8888-8888-8888-888888888888");
    public static readonly Guid AthleteUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    public static readonly Guid OpenAiProviderId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid GoogleProviderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public static readonly Guid Gpt4oModelId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid EmbeddingModelId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid GeminiModelId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid DeprecatedModelId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public static readonly Guid SearchToolId = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
    public static readonly Guid NotificationToolId = Guid.Parse("2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e");
}

public static class AIRoleNames
{
    public const string PlatformAdministrator = "Platform Administrator";
    public const string AIAdministrator = "AI Administrator";
}
