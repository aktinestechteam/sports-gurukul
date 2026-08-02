using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AI_Domain_Persistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIAssistants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AssistantType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Personality = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SystemPrompt = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    GreetingMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    MaxHistoryLength = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIAssistants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Action = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ActorId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ActorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PreviousState = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    NewState = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ApiBaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ApiVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MaxRetries = table.Column<int>(type: "integer", nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    CostPerToken = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIRoutingPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Strategy = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProviderIds = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ModelIds = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Rules = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    MaxRetries = table.Column<int>(type: "integer", nullable: false),
                    FallbackEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    FallbackPolicy = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRoutingPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Visibility = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TotalSources = table.Column<int>(type: "integer", nullable: false),
                    TotalDocuments = table.Column<int>(type: "integer", nullable: false),
                    TotalSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TemplateContent = table.Column<string>(type: "text", nullable: false),
                    Variables = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SemanticSearchRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Query = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    IndexId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaxResults = table.Column<int>(type: "integer", nullable: false),
                    MinScore = table.Column<double>(type: "double precision", nullable: false),
                    ModelName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Filters = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResultCount = table.Column<int>(type: "integer", nullable: true),
                    ExecutionTimeMs = table.Column<double>(type: "double precision", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemanticSearchRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToolDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Schema = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    EndpointUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Authentication = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Parameters = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ReturnType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VectorIndexes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IndexType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dimensions = table.Column<int>(type: "integer", nullable: false),
                    DistanceMetric = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TotalVectors = table.Column<int>(type: "integer", nullable: false),
                    IndexConfiguration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TableName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VectorIndexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Steps = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Triggers = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Conditions = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Variables = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AssistantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Configuration = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Tools = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Rules = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Constraints = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    MaxIterations = table.Column<int>(type: "integer", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentDefinitions_AIAssistants_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AIAssistants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AssistantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContextSummary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TokenCount = table.Column<int>(type: "integer", nullable: true),
                    MessageCount = table.Column<int>(type: "integer", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_AIAssistants_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AIAssistants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AIModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Capabilities = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaxTokens = table.Column<int>(type: "integer", nullable: true),
                    MaxContextLength = table.Column<int>(type: "integer", nullable: true),
                    CostPerInputToken = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    CostPerOutputToken = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    CostPerImageToken = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: true),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: true),
                    DefaultTemperature = table.Column<double>(type: "double precision", nullable: false),
                    SupportsStreaming = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsFunctionCalling = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsVision = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsEmbeddings = table.Column<bool>(type: "boolean", nullable: false),
                    ModelVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIModels_AIProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AIProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SourceUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Configuration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DocumentCount = table.Column<int>(type: "integer", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeSources_KnowledgeBases_KnowledgeBaseId",
                        column: x => x.KnowledgeBaseId,
                        principalTable: "KnowledgeBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromptVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromptTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ChangeNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptVersions_PromptTemplates_PromptTemplateId",
                        column: x => x.PromptTemplateId,
                        principalTable: "PromptTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Input = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Output = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CurrentStep = table.Column<int>(type: "integer", nullable: true),
                    TotalSteps = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowExecutions_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgentExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Input = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Output = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Iterations = table.Column<int>(type: "integer", nullable: true),
                    TokensUsed = table.Column<int>(type: "integer", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentExecutions_AgentDefinitions_AgentDefinitionId",
                        column: x => x.AgentDefinitionId,
                        principalTable: "AgentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationMemories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Importance = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Keywords = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Context = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsConsolidated = table.Column<bool>(type: "boolean", nullable: false),
                    RelevanceScore = table.Column<double>(type: "double precision", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMemories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationMemories_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PromptTokens = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CompletionTokens = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TotalTokens = table.Column<int>(type: "integer", nullable: true),
                    TokensUsed = table.Column<int>(type: "integer", nullable: true),
                    ToolCalls = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ToolResults = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    LatencyMs = table.Column<double>(type: "double precision", nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationMessages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToolExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ToolDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Input = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Output = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExecutionTimeMs = table.Column<double>(type: "double precision", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolExecutions_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ToolExecutions_ToolDefinitions_ToolDefinitionId",
                        column: x => x.ToolDefinitionId,
                        principalTable: "ToolDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIModelConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Temperature = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: true),
                    MaxTokens = table.Column<int>(type: "integer", nullable: true),
                    TopP = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: true),
                    FrequencyPenalty = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: true),
                    PresencePenalty = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: true),
                    StopSequences = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ModelParameters = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModelConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIModelConfigurations_AIModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PageCount = table.Column<int>(type: "integer", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EmbeddingStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IndexedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeDocuments_KnowledgeSources_KnowledgeSourceId",
                        column: x => x.KnowledgeSourceId,
                        principalTable: "KnowledgeSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AITokenUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PromptTokens = table.Column<int>(type: "integer", nullable: false),
                    CompletionTokens = table.Column<int>(type: "integer", nullable: false),
                    TotalTokens = table.Column<int>(type: "integer", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RequestType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AITokenUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AITokenUsages_ConversationMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "ConversationMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AITokenUsages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmbeddingChunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TokenCount = table.Column<int>(type: "integer", nullable: true),
                    CharacterCount = table.Column<int>(type: "integer", nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddingChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbeddingChunks_KnowledgeDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "KnowledgeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SemanticSearchResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    DocumentTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ChunkContent = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemanticSearchResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemanticSearchResults_KnowledgeDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "KnowledgeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SemanticSearchResults_SemanticSearchRequests_SearchRequestId",
                        column: x => x.SearchRequestId,
                        principalTable: "SemanticSearchRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Embeddings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChunkId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dimensions = table.Column<int>(type: "integer", nullable: false),
                    Vector = table.Column<float[]>(type: "real[]", nullable: false),
                    Text = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    TokenCount = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Embeddings_EmbeddingChunks_ChunkId",
                        column: x => x.ChunkId,
                        principalTable: "EmbeddingChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Embeddings_KnowledgeDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "KnowledgeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AIAssistants",
                columns: new[] { "Id", "AssistantType", "AvatarUrl", "CreatedAt", "Description", "GreetingMessage", "IsActive", "IsDeleted", "IsPublic", "MaxHistoryLength", "Name", "Personality", "SystemPrompt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), "Nutritionist", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AI-powered nutrition and diet planning assistant", "Hi! I'm your AI nutrition advisor. Let's plan your diet!", true, false, true, null, "Nutrition Advisor", "Friendly", "You are an expert nutrition advisor...", null },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "Coach", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AI-powered sports coaching assistant", "Hello! I'm your AI sports coach. How can I help you today?", true, false, true, null, "Sports Coach", "Motivational", "You are an expert sports coach assistant...", null }
                });

            migrationBuilder.InsertData(
                table: "AIProviders",
                columns: new[] { "Id", "ApiBaseUrl", "ApiVersion", "CostPerToken", "CreatedAt", "Description", "IsActive", "IsDeleted", "MaxRetries", "Name", "TimeoutSeconds", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "https://api.openai.com/v1", null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, "OpenAI", null, "OpenAI", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "https://api.azure.com/openai", null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, "Azure OpenAI", null, "AzureOpenAI", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "https://api.anthropic.com/v1", null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, "Anthropic", null, "Anthropic", null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "https://generativelanguage.googleapis.com", null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, "Google AI", null, "Google", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "http://localhost:11434", null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, "Ollama", null, "Ollama", null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "https://openrouter.ai/api/v1", null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, "OpenRouter", null, "OpenRouter", null }
                });

            migrationBuilder.InsertData(
                table: "AIModels",
                columns: new[] { "Id", "Capabilities", "CostPerImageToken", "CostPerInputToken", "CostPerOutputToken", "CreatedAt", "DefaultTemperature", "Description", "DisplayName", "IsDeleted", "MaxContextLength", "MaxTokens", "ModelVersion", "Name", "ProviderId", "ReleasedAt", "Status", "SupportsEmbeddings", "SupportsFunctionCalling", "SupportsStreaming", "SupportsVision", "TemperatureMax", "TemperatureMin", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "TextGeneration, CodeGeneration, Reasoning, FunctionCalling", null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.69999999999999996, null, "GPT-4", false, 32768, 8192, null, "gpt-4", new Guid("11111111-1111-1111-1111-111111111111"), null, "Active", false, true, true, true, null, null, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "TextGeneration, CodeGeneration, FunctionCalling", null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.69999999999999996, null, "GPT-3.5 Turbo", false, 16384, 4096, null, "gpt-3.5-turbo", new Guid("11111111-1111-1111-1111-111111111111"), null, "Active", false, true, true, false, null, null, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "TextGeneration, CodeGeneration, Reasoning, Vision", null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.69999999999999996, null, "Claude 3 Opus", false, 200000, 4096, null, "claude-3-opus", new Guid("33333333-3333-3333-3333-333333333333"), null, "Active", false, false, true, false, null, null, null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "TextGeneration, CodeGeneration, Reasoning, Vision", null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.69999999999999996, null, "Claude 3 Sonnet", false, 200000, 4096, null, "claude-3-sonnet", new Guid("33333333-3333-3333-3333-333333333333"), null, "Active", false, false, true, false, null, null, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "TextGeneration, CodeGeneration, Reasoning, Vision", null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.69999999999999996, null, "Gemini Pro", false, 32768, 8192, null, "gemini-pro", new Guid("44444444-4444-4444-4444-444444444444"), null, "Active", false, false, true, false, null, null, null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "TextGeneration, CodeGeneration", null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.69999999999999996, null, "Llama 3", false, 8192, 8192, null, "llama3", new Guid("55555555-5555-5555-5555-555555555555"), null, "Active", false, false, true, false, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_AssistantId",
                table: "AgentDefinitions",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_Name",
                table: "AgentDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_Status",
                table: "AgentDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AgentExecutions_AgentDefinitionId",
                table: "AgentExecutions",
                column: "AgentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentExecutions_Status",
                table: "AgentExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_AssistantType",
                table: "AIAssistants",
                column: "AssistantType");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_Name",
                table: "AIAssistants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_ActorId",
                table: "AIAuditLogs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_EntityId",
                table: "AIAuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_EntityType",
                table: "AIAuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_EventType",
                table: "AIAuditLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_Severity",
                table: "AIAuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_IsDefault",
                table: "AIModelConfigurations",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_ModelId",
                table: "AIModelConfigurations",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Name",
                table: "AIModels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_ProviderId",
                table: "AIModels",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Status",
                table: "AIModels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AIProviders_Name",
                table: "AIProviders",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AIProviders_Type",
                table: "AIProviders",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_Name",
                table: "AIRoutingPolicies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_Status",
                table: "AIRoutingPolicies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_Strategy",
                table: "AIRoutingPolicies",
                column: "Strategy");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ConversationId",
                table: "AITokenUsages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_MessageId",
                table: "AITokenUsages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ModelName",
                table: "AITokenUsages",
                column: "ModelName");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ProviderName",
                table: "AITokenUsages",
                column: "ProviderName");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_UserId",
                table: "AITokenUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMemories_ConversationId",
                table: "ConversationMemories",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMemories_Importance",
                table: "ConversationMemories",
                column: "Importance");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMemories_Type",
                table: "ConversationMemories",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_ConversationId",
                table: "ConversationMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_Role",
                table: "ConversationMessages",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_Status",
                table: "ConversationMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AssistantId",
                table: "Conversations",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastActivityAt",
                table: "Conversations",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Status",
                table: "Conversations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingChunks_DocumentId",
                table: "EmbeddingChunks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingChunks_DocumentId_ChunkIndex",
                table: "EmbeddingChunks",
                columns: new[] { "DocumentId", "ChunkIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_ChunkId",
                table: "Embeddings",
                column: "ChunkId",
                unique: true,
                filter: "[ChunkId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_DocumentId",
                table: "Embeddings",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_ModelName",
                table: "Embeddings",
                column: "ModelName");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_Name",
                table: "KnowledgeBases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_Status",
                table: "KnowledgeBases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_Visibility",
                table: "KnowledgeBases",
                column: "Visibility");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_EmbeddingStatus",
                table: "KnowledgeDocuments",
                column: "EmbeddingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_KnowledgeSourceId",
                table: "KnowledgeDocuments",
                column: "KnowledgeSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_Title",
                table: "KnowledgeDocuments",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_Type",
                table: "KnowledgeDocuments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_KnowledgeBaseId",
                table: "KnowledgeSources",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_Name",
                table: "KnowledgeSources",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_SourceType",
                table: "KnowledgeSources",
                column: "SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_Status",
                table: "KnowledgeSources",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_Name",
                table: "PromptTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_Status",
                table: "PromptTemplates",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_Type",
                table: "PromptTemplates",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_PromptVersions_PromptTemplateId",
                table: "PromptVersions",
                column: "PromptTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptVersions_PromptTemplateId_VersionNumber",
                table: "PromptVersions",
                columns: new[] { "PromptTemplateId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_KnowledgeBaseId",
                table: "SemanticSearchRequests",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_Status",
                table: "SemanticSearchRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchResults_DocumentId",
                table: "SemanticSearchResults",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchResults_SearchRequestId",
                table: "SemanticSearchResults",
                column: "SearchRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_Name",
                table: "ToolDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_Status",
                table: "ToolDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_Type",
                table: "ToolDefinitions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_ConversationId",
                table: "ToolExecutions",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_ToolDefinitionId",
                table: "ToolExecutions",
                column: "ToolDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_VectorIndexes_Name",
                table: "VectorIndexes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_VectorIndexes_Status",
                table: "VectorIndexes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_Name",
                table: "WorkflowDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_Status",
                table: "WorkflowDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_Status",
                table: "WorkflowExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_WorkflowDefinitionId",
                table: "WorkflowExecutions",
                column: "WorkflowDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentExecutions");

            migrationBuilder.DropTable(
                name: "AIAuditLogs");

            migrationBuilder.DropTable(
                name: "AIModelConfigurations");

            migrationBuilder.DropTable(
                name: "AIRoutingPolicies");

            migrationBuilder.DropTable(
                name: "AITokenUsages");

            migrationBuilder.DropTable(
                name: "ConversationMemories");

            migrationBuilder.DropTable(
                name: "Embeddings");

            migrationBuilder.DropTable(
                name: "PromptVersions");

            migrationBuilder.DropTable(
                name: "SemanticSearchResults");

            migrationBuilder.DropTable(
                name: "ToolExecutions");

            migrationBuilder.DropTable(
                name: "VectorIndexes");

            migrationBuilder.DropTable(
                name: "WorkflowExecutions");

            migrationBuilder.DropTable(
                name: "AgentDefinitions");

            migrationBuilder.DropTable(
                name: "AIModels");

            migrationBuilder.DropTable(
                name: "ConversationMessages");

            migrationBuilder.DropTable(
                name: "EmbeddingChunks");

            migrationBuilder.DropTable(
                name: "PromptTemplates");

            migrationBuilder.DropTable(
                name: "SemanticSearchRequests");

            migrationBuilder.DropTable(
                name: "ToolDefinitions");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "AIProviders");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "KnowledgeDocuments");

            migrationBuilder.DropTable(
                name: "AIAssistants");

            migrationBuilder.DropTable(
                name: "KnowledgeSources");

            migrationBuilder.DropTable(
                name: "KnowledgeBases");
        }
    }
}
