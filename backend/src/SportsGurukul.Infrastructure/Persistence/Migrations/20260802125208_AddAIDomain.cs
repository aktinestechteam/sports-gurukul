using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAIDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    DetailsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    BeforeJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    AfterJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ChangedFieldsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ProviderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AuthType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DefaultApiVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SupportsChat = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsEmbeddings = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsVision = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsFunctionCalling = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ConfigurationSchemaJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DocumentationUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                name: "VectorIndexes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    DistanceMetric = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IndexName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ItemCount = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ConfigurationJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    LastIndexedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WorkflowType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DefinitionJson = table.Column<string>(type: "text", nullable: false),
                    EntryNode = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                name: "AIModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Family = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContextWindow = table.Column<int>(type: "integer", nullable: true),
                    MaxOutputTokens = table.Column<int>(type: "integer", nullable: true),
                    InputCostPerMillionTokens = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    OutputCostPerMillionTokens = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SupportsChat = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsEmbeddings = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsVision = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsFunctionCalling = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsJsonMode = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsStreaming = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RateLimitPerMinute = table.Column<int>(type: "integer", nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TriggerType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InputJson = table.Column<string>(type: "text", nullable: true),
                    OutputJson = table.Column<string>(type: "text", nullable: true),
                    ErrorJson = table.Column<string>(type: "text", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    TotalTokens = table.Column<int>(type: "integer", nullable: true),
                    TotalCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExecutedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                name: "AgentDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AgentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SystemPrompt = table.Column<string>(type: "text", nullable: true),
                    Temperature = table.Column<double>(type: "double precision", nullable: true),
                    MaxIterations = table.Column<int>(type: "integer", nullable: true),
                    MemoryEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ToolsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentDefinitions_AIModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AgentDefinitions_WorkflowDefinitions_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AIAssistants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AssistantType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SystemPrompt = table.Column<string>(type: "text", nullable: true),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Temperature = table.Column<double>(type: "double precision", nullable: true),
                    TopP = table.Column<double>(type: "double precision", nullable: true),
                    MaxTokens = table.Column<int>(type: "integer", nullable: true),
                    MemoryEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    StreamingEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GuardrailsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIAssistants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIAssistants_AIModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AIRoutingPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    DefaultModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RoutingStrategy = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ConditionsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    PreferredModelIdsJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    FallbackModelIdsJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    MinScore = table.Column<double>(type: "double precision", nullable: true),
                    MaxCostPerRequest = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxLatencyMs = table.Column<int>(type: "integer", nullable: true),
                    AllowFallback = table.Column<bool>(type: "boolean", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRoutingPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIRoutingPolicies_AIModels_DefaultModelId",
                        column: x => x.DefaultModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AIRoutingPolicies_AIProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AIProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    KnowledgeBaseType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OwnerType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    VectorIndexId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmbeddingModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChunkingStrategy = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ChunkSize = table.Column<int>(type: "integer", nullable: false),
                    ChunkOverlap = table.Column<int>(type: "integer", nullable: false),
                    EmbeddingDimension = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetadataSchemaJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    StatisticsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeBases_AIModels_EmbeddingModelId",
                        column: x => x.EmbeddingModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KnowledgeBases_VectorIndexes_VectorIndexId",
                        column: x => x.VectorIndexId,
                        principalTable: "VectorIndexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AgentExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowExecutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InputJson = table.Column<string>(type: "text", nullable: true),
                    OutputJson = table.Column<string>(type: "text", nullable: true),
                    ErrorJson = table.Column<string>(type: "text", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    TokensUsed = table.Column<int>(type: "integer", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ExecutedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgentExecutions_WorkflowExecutions_WorkflowExecutionId",
                        column: x => x.WorkflowExecutionId,
                        principalTable: "WorkflowExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToolDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ToolType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Endpoint = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    InputSchemaJson = table.Column<string>(type: "text", nullable: false),
                    OutputSchemaJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemTool = table.Column<bool>(type: "boolean", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    RetryPolicyJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolDefinitions_AgentDefinitions_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AgentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AIModelConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssistantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgentDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Temperature = table.Column<double>(type: "double precision", nullable: true),
                    TopP = table.Column<double>(type: "double precision", nullable: true),
                    TopK = table.Column<double>(type: "double precision", nullable: true),
                    MaxTokens = table.Column<int>(type: "integer", nullable: true),
                    StopSequencesJson = table.Column<string>(type: "text", nullable: true),
                    FrequencyPenalty = table.Column<double>(type: "double precision", nullable: true),
                    PresencePenalty = table.Column<double>(type: "double precision", nullable: true),
                    ApiKeyEncrypted = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ApiVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BaseUrlOverride = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    MaxRetries = table.Column<int>(type: "integer", nullable: true),
                    StreamingEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModelConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIModelConfigurations_AIAssistants_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AIAssistants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AIModelConfigurations_AIModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AIModelConfigurations_AIProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AIProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AIModelConfigurations_AgentDefinitions_AgentDefinitionId",
                        column: x => x.AgentDefinitionId,
                        principalTable: "AgentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssistantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ParticipantType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ParticipantUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MessageCount = table.Column<int>(type: "integer", nullable: false),
                    TokenCount = table.Column<int>(type: "integer", nullable: false),
                    KnowledgeBaseIdsJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ContextMetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromptTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssistantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PromptType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TemplateText = table.Column<string>(type: "text", nullable: false),
                    InputSchemaJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    OutputSchemaJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    VariablesJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptTemplates_AIAssistants_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AIAssistants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SourceType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Uri = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IngestionStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StatusMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LastIngestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefreshIntervalMinutes = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ErrorDetailsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                name: "ToolExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ToolDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentExecutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkflowExecutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestJson = table.Column<string>(type: "text", nullable: true),
                    ResponseJson = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    TokenCount = table.Column<int>(type: "integer", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolExecutions_AgentExecutions_AgentExecutionId",
                        column: x => x.AgentExecutionId,
                        principalTable: "AgentExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToolExecutions_ToolDefinitions_ToolDefinitionId",
                        column: x => x.ToolDefinitionId,
                        principalTable: "ToolDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToolExecutions_WorkflowExecutions_WorkflowExecutionId",
                        column: x => x.WorkflowExecutionId,
                        principalTable: "WorkflowExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AITokenUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssistantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UsageType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InputTokens = table.Column<int>(type: "integer", nullable: false),
                    OutputTokens = table.Column<int>(type: "integer", nullable: false),
                    TotalTokens = table.Column<int>(type: "integer", nullable: false),
                    CacheReadTokens = table.Column<int>(type: "integer", nullable: true),
                    CacheWriteTokens = table.Column<int>(type: "integer", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LatencyMs = table.Column<long>(type: "bigint", nullable: true),
                    ModelName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AITokenUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AITokenUsages_AIAssistants_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AIAssistants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AITokenUsages_AIModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AITokenUsages_AIProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AIProviders",
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
                name: "ConversationMemories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemoryType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Importance = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ModelName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PromptVersionUsed = table.Column<int>(type: "integer", nullable: true),
                    InputTokenCount = table.Column<int>(type: "integer", nullable: true),
                    OutputTokenCount = table.Column<int>(type: "integer", nullable: true),
                    LatencyMs = table.Column<long>(type: "bigint", nullable: true),
                    ToolCallsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ToolResultsJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                name: "SemanticSearchRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Query = table.Column<string>(type: "text", nullable: false),
                    QueryEmbedding = table.Column<float[]>(type: "real[]", nullable: true),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    VectorIndexId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    TopK = table.Column<int>(type: "integer", nullable: false),
                    SimilarityThreshold = table.Column<double>(type: "double precision", nullable: true),
                    FiltersJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ModelUsed = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResultCount = table.Column<int>(type: "integer", nullable: false),
                    LatencyMs = table.Column<long>(type: "bigint", nullable: true),
                    RequestedByType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemanticSearchRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemanticSearchRequests_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SemanticSearchRequests_KnowledgeBases_KnowledgeBaseId",
                        column: x => x.KnowledgeBaseId,
                        principalTable: "KnowledgeBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SemanticSearchRequests_VectorIndexes_VectorIndexId",
                        column: x => x.VectorIndexId,
                        principalTable: "VectorIndexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PromptVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromptTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ChangeSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeployedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EvaluationJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
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
                name: "KnowledgeDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StoragePath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MimeType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PageCount = table.Column<int>(type: "integer", nullable: true),
                    WordCount = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeDocuments_KnowledgeBases_KnowledgeBaseId",
                        column: x => x.KnowledgeBaseId,
                        principalTable: "KnowledgeBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnowledgeDocuments_KnowledgeSources_KnowledgeSourceId",
                        column: x => x.KnowledgeSourceId,
                        principalTable: "KnowledgeSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmbeddingChunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TokenCount = table.Column<int>(type: "integer", nullable: true),
                    CharacterCount = table.Column<int>(type: "integer", nullable: false),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddingChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbeddingChunks_KnowledgeBases_KnowledgeBaseId",
                        column: x => x.KnowledgeBaseId,
                        principalTable: "KnowledgeBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmbeddingChunks_KnowledgeDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "KnowledgeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Embeddings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChunkId = table.Column<Guid>(type: "uuid", nullable: false),
                    KnowledgeBaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Vector = table.Column<float[]>(type: "real[]", nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    Norm = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Embeddings_AIModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Embeddings_EmbeddingChunks_ChunkId",
                        column: x => x.ChunkId,
                        principalTable: "EmbeddingChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Embeddings_KnowledgeBases_KnowledgeBaseId",
                        column: x => x.KnowledgeBaseId,
                        principalTable: "KnowledgeBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SemanticSearchResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SemanticSearchRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChunkId = table.Column<Guid>(type: "uuid", nullable: true),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    MetadataJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ReRankScore = table.Column<double>(type: "double precision", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemanticSearchResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemanticSearchResults_EmbeddingChunks_ChunkId",
                        column: x => x.ChunkId,
                        principalTable: "EmbeddingChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SemanticSearchResults_KnowledgeDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "KnowledgeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SemanticSearchResults_SemanticSearchRequests_SemanticSearch~",
                        column: x => x.SemanticSearchRequestId,
                        principalTable: "SemanticSearchRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AIProviders",
                columns: new[] { "Id", "AuthType", "BaseUrl", "ConfigurationSchemaJson", "CreatedAt", "DefaultApiVersion", "Description", "DisplayName", "DocumentationUrl", "IconUrl", "IsActive", "IsDeleted", "Name", "ProviderType", "SupportsChat", "SupportsEmbeddings", "SupportsFunctionCalling", "SupportsVision", "UpdatedAt", "WebsiteUrl" },
                values: new object[,]
                {
                    { new Guid("a1000000-0000-0000-0000-000000000001"), "ApiKey", "https://api.openai.com/v1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OpenAI GPT and embedding models.", "OpenAI", "https://platform.openai.com/docs", null, true, false, "openai", "OpenAi", true, true, true, true, null, "https://openai.com" },
                    { new Guid("a1000000-0000-0000-0000-000000000002"), "ApiKey", "https://{resource}.openai.azure.com", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OpenAI models hosted on Microsoft Azure.", "Azure OpenAI", "https://learn.microsoft.com/azure/ai-services/openai", null, true, false, "azure-openai", "AzureOpenAi", true, true, true, true, null, "https://azure.microsoft.com/products/ai-services/openai-service" },
                    { new Guid("a1000000-0000-0000-0000-000000000003"), "BearerToken", "https://api.anthropic.com/v1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Anthropic Claude models.", "Anthropic Claude", "https://docs.anthropic.com", null, true, false, "anthropic", "Anthropic", true, false, true, true, null, "https://www.anthropic.com" },
                    { new Guid("a1000000-0000-0000-0000-000000000004"), "ApiKey", "https://generativelanguage.googleapis.com/v1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Google Gemini models.", "Google Gemini", "https://ai.google.dev", null, true, false, "google", "Google", true, true, true, true, null, "https://deepmind.google/technologies/gemini" },
                    { new Guid("a1000000-0000-0000-0000-000000000005"), "None", "http://localhost:11434", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Self-hosted local open-source models via Ollama.", "Ollama", "https://github.com/ollama/ollama", null, true, false, "ollama", "Ollama", true, true, true, true, null, "https://ollama.com" },
                    { new Guid("a1000000-0000-0000-0000-000000000006"), "ApiKey", "https://openrouter.ai/api/v1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Unified gateway to multiple AI model providers.", "OpenRouter", "https://openrouter.ai/docs", null, true, false, "openrouter", "OpenRouter", true, true, true, true, null, "https://openrouter.ai" }
                });

            migrationBuilder.InsertData(
                table: "AIModels",
                columns: new[] { "Id", "ContextWindow", "CreatedAt", "Currency", "Description", "DisplayName", "Family", "InputCostPerMillionTokens", "IsActive", "IsDeleted", "MaxOutputTokens", "Name", "OutputCostPerMillionTokens", "ProviderId", "RateLimitPerMinute", "SupportsChat", "SupportsEmbeddings", "SupportsFunctionCalling", "SupportsJsonMode", "SupportsStreaming", "SupportsVision", "UpdatedAt", "Version" },
                values: new object[,]
                {
                    { new Guid("a2000000-0000-0000-0000-000000000001"), 128000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "High-intelligence multimodal flagship model.", "GPT-4o", "Gpt", 2.50m, true, false, 16384, "gpt-4o", 10.00m, new Guid("a1000000-0000-0000-0000-000000000001"), null, true, false, true, true, true, true, null, "2024-08-06" },
                    { new Guid("a2000000-0000-0000-0000-000000000002"), 128000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Cost-efficient small model for high-volume tasks.", "GPT-4o Mini", "Gpt", 0.15m, true, false, 16384, "gpt-4o-mini", 0.60m, new Guid("a1000000-0000-0000-0000-000000000001"), null, true, false, true, true, true, true, null, "2024-07-18" },
                    { new Guid("a2000000-0000-0000-0000-000000000003"), 16385, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Legacy low-latency chat model.", "GPT-3.5 Turbo", "Gpt", 0.50m, true, false, 4096, "gpt-3.5-turbo", 1.50m, new Guid("a1000000-0000-0000-0000-000000000001"), null, true, false, true, true, true, false, null, "0125" },
                    { new Guid("a2000000-0000-0000-0000-000000000004"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Efficient text embedding model, 1536 dimensions.", "Text Embedding 3 Small", "Embedding", 0.02m, true, false, null, "text-embedding-3-small", 0.00m, new Guid("a1000000-0000-0000-0000-000000000001"), null, false, true, false, false, false, false, null, "1" },
                    { new Guid("a2000000-0000-0000-0000-000000000005"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "High-quality text embedding model, 3072 dimensions.", "Text Embedding 3 Large", "Embedding", 0.13m, true, false, null, "text-embedding-3-large", 0.00m, new Guid("a1000000-0000-0000-0000-000000000001"), null, false, true, false, false, false, false, null, "1" },
                    { new Guid("a2000000-0000-0000-0000-000000000006"), 128000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "GPT-4o deployed on Azure OpenAI.", "GPT-4o (Azure)", "Gpt", 2.50m, true, false, 16384, "gpt-4o", 10.00m, new Guid("a1000000-0000-0000-0000-000000000002"), null, true, false, true, true, true, true, null, "2024-11-20" },
                    { new Guid("a2000000-0000-0000-0000-000000000007"), 128000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "GPT-4o Mini deployed on Azure OpenAI.", "GPT-4o Mini (Azure)", "Gpt", 0.15m, true, false, 16384, "gpt-4o-mini", 0.60m, new Guid("a1000000-0000-0000-0000-000000000002"), null, true, false, true, true, true, true, null, "2024-09-03" },
                    { new Guid("a2000000-0000-0000-0000-000000000008"), 200000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Balanced intelligence and speed for production workloads.", "Claude Sonnet 4", "Claude", 3.00m, true, false, 64000, "claude-sonnet-4", 15.00m, new Guid("a1000000-0000-0000-0000-000000000003"), null, true, false, true, true, true, true, null, "20250514" },
                    { new Guid("a2000000-0000-0000-0000-000000000009"), 200000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Fast, low-cost model for high-throughput tasks.", "Claude Haiku 4.5", "Claude", 1.00m, true, false, 64000, "claude-haiku-4-5", 5.00m, new Guid("a1000000-0000-0000-0000-000000000003"), null, true, false, true, true, true, true, null, "20250514" },
                    { new Guid("a2000000-0000-0000-0000-00000000000a"), 1048576, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Google's advanced multimodal reasoning model.", "Gemini 2.0 Pro", "Gemini", 3.50m, true, false, 8192, "gemini-2.0-pro", 15.00m, new Guid("a1000000-0000-0000-0000-000000000004"), null, true, false, true, true, true, true, null, "002" },
                    { new Guid("a2000000-0000-0000-0000-00000000000b"), 1048576, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Fast, cost-efficient multimodal model.", "Gemini 2.0 Flash", "Gemini", 0.30m, true, false, 8192, "gemini-2.0-flash", 1.50m, new Guid("a1000000-0000-0000-0000-000000000004"), null, true, false, true, true, true, true, null, "001" },
                    { new Guid("a2000000-0000-0000-0000-00000000000c"), 131072, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Open-source 8B parameter model, self-hosted.", "Llama 3.1 8B", "Llama", null, true, false, 8192, "llama3.1:8b", null, new Guid("a1000000-0000-0000-0000-000000000005"), null, true, false, true, false, true, false, null, "8b" },
                    { new Guid("a2000000-0000-0000-0000-00000000000d"), 131072, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Open-source 70B parameter model, self-hosted.", "Llama 3.3 70B", "Llama", null, true, false, 8192, "llama3.3:70b", null, new Guid("a1000000-0000-0000-0000-000000000005"), null, true, false, true, false, true, false, null, "70b" },
                    { new Guid("a2000000-0000-0000-0000-00000000000e"), 128000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "GPT-4o accessible through OpenRouter gateway.", "GPT-4o (OpenRouter)", "Gpt", 2.50m, true, false, 16384, "openai/gpt-4o", 10.00m, new Guid("a1000000-0000-0000-0000-000000000006"), null, true, false, true, true, true, true, null, "1" },
                    { new Guid("a2000000-0000-0000-0000-00000000000f"), 200000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Claude Sonnet 4 accessible through OpenRouter gateway.", "Claude Sonnet 4 (OpenRouter)", "Claude", 3.00m, true, false, 64000, "anthropic/claude-sonnet-4", 15.00m, new Guid("a1000000-0000-0000-0000-000000000006"), null, true, false, true, true, true, true, null, "1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_AgentType",
                table: "AgentDefinitions",
                column: "AgentType");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_IsActive",
                table: "AgentDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_ModelId",
                table: "AgentDefinitions",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_WorkflowId",
                table: "AgentDefinitions",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDefinitions_WorkflowId_Name",
                table: "AgentDefinitions",
                columns: new[] { "Name", "WorkflowId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentExecutions_AgentDefinitionId",
                table: "AgentExecutions",
                column: "AgentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentExecutions_CreatedAt",
                table: "AgentExecutions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AgentExecutions_Status",
                table: "AgentExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AgentExecutions_WorkflowExecutionId",
                table: "AgentExecutions",
                column: "WorkflowExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_AssistantType",
                table: "AIAssistants",
                column: "AssistantType");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_IsActive",
                table: "AIAssistants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_ModelId",
                table: "AIAssistants",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_Name",
                table: "AIAssistants",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_OwnerType",
                table: "AIAssistants",
                column: "OwnerType");

            migrationBuilder.CreateIndex(
                name: "IX_AIAssistants_OwnerUserId",
                table: "AIAssistants",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_Action",
                table: "AIAuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_ActorUserId",
                table: "AIAuditLogs",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_CorrelationId",
                table: "AIAuditLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_CreatedAt",
                table: "AIAuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_EntityType_EntityId",
                table: "AIAuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AIAuditLogs_Severity",
                table: "AIAuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_AgentDefinitionId",
                table: "AIModelConfigurations",
                column: "AgentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_AssistantId",
                table: "AIModelConfigurations",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_AssistantId_Name",
                table: "AIModelConfigurations",
                columns: new[] { "AssistantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_IsActive",
                table: "AIModelConfigurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_ModelId",
                table: "AIModelConfigurations",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_ProviderId",
                table: "AIModelConfigurations",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Family",
                table: "AIModels",
                column: "Family");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_IsActive",
                table: "AIModels",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Name_Version",
                table: "AIModels",
                columns: new[] { "Name", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_ProviderId",
                table: "AIModels",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AIProviders_IsActive",
                table: "AIProviders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AIProviders_Name",
                table: "AIProviders",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIProviders_ProviderType",
                table: "AIProviders",
                column: "ProviderType");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_DefaultModelId",
                table: "AIRoutingPolicies",
                column: "DefaultModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_IsActive",
                table: "AIRoutingPolicies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_Name",
                table: "AIRoutingPolicies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_Priority",
                table: "AIRoutingPolicies",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_ProviderId",
                table: "AIRoutingPolicies",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AIRoutingPolicies_RoutingStrategy",
                table: "AIRoutingPolicies",
                column: "RoutingStrategy");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_AssistantId",
                table: "AITokenUsages",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ConversationId",
                table: "AITokenUsages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_CreatedAt",
                table: "AITokenUsages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ModelId",
                table: "AITokenUsages",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ModelId_CreatedAt",
                table: "AITokenUsages",
                columns: new[] { "ModelId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_ProviderId",
                table: "AITokenUsages",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_UsageType",
                table: "AITokenUsages",
                column: "UsageType");

            migrationBuilder.CreateIndex(
                name: "IX_AITokenUsages_UserId",
                table: "AITokenUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMemories_Conversation_Type_Key",
                table: "ConversationMemories",
                columns: new[] { "ConversationId", "MemoryType", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMemories_ConversationId",
                table: "ConversationMemories",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMemories_ExpiresAt",
                table: "ConversationMemories",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_Conversation_Sequence",
                table: "ConversationMessages",
                columns: new[] { "ConversationId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_ConversationId",
                table: "ConversationMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_CreatedAt",
                table: "ConversationMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_Role",
                table: "ConversationMessages",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AssistantId",
                table: "Conversations",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AssistantId_Status",
                table: "Conversations",
                columns: new[] { "AssistantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastMessageAt",
                table: "Conversations",
                column: "LastMessageAt");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ParticipantUserId",
                table: "Conversations",
                column: "ParticipantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Status",
                table: "Conversations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingChunks_Document_ChunkIndex",
                table: "EmbeddingChunks",
                columns: new[] { "DocumentId", "ChunkIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingChunks_DocumentId",
                table: "EmbeddingChunks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingChunks_KnowledgeBaseId",
                table: "EmbeddingChunks",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_ChunkId",
                table: "Embeddings",
                column: "ChunkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_KnowledgeBaseId",
                table: "Embeddings",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_KnowledgeBaseId_Status",
                table: "Embeddings",
                columns: new[] { "KnowledgeBaseId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_ModelId",
                table: "Embeddings",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_Status",
                table: "Embeddings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_EmbeddingModelId",
                table: "KnowledgeBases",
                column: "EmbeddingModelId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_IsActive",
                table: "KnowledgeBases",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_KnowledgeBaseType",
                table: "KnowledgeBases",
                column: "KnowledgeBaseType");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_Name",
                table: "KnowledgeBases",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_OwnerUserId",
                table: "KnowledgeBases",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_VectorIndexId",
                table: "KnowledgeBases",
                column: "VectorIndexId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_ContentHash",
                table: "KnowledgeDocuments",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_KnowledgeBaseId",
                table: "KnowledgeDocuments",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_KnowledgeBaseId_Status",
                table: "KnowledgeDocuments",
                columns: new[] { "KnowledgeBaseId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_KnowledgeSourceId",
                table: "KnowledgeDocuments",
                column: "KnowledgeSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_Status",
                table: "KnowledgeDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_IngestionStatus",
                table: "KnowledgeSources",
                column: "IngestionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_IsActive",
                table: "KnowledgeSources",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_KnowledgeBaseId",
                table: "KnowledgeSources",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_KnowledgeBaseId_Name",
                table: "KnowledgeSources",
                columns: new[] { "KnowledgeBaseId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_SourceType",
                table: "KnowledgeSources",
                column: "SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_AssistantId",
                table: "PromptTemplates",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_AssistantId_Name",
                table: "PromptTemplates",
                columns: new[] { "AssistantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_IsActive",
                table: "PromptTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_PromptType",
                table: "PromptTemplates",
                column: "PromptType");

            migrationBuilder.CreateIndex(
                name: "IX_PromptVersions_IsActive",
                table: "PromptVersions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PromptVersions_PromptTemplateId",
                table: "PromptVersions",
                column: "PromptTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptVersions_Template_Version",
                table: "PromptVersions",
                columns: new[] { "PromptTemplateId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_ConversationId",
                table: "SemanticSearchRequests",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_CreatedAt",
                table: "SemanticSearchRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_KnowledgeBaseId",
                table: "SemanticSearchRequests",
                column: "KnowledgeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_Status",
                table: "SemanticSearchRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchRequests_VectorIndexId",
                table: "SemanticSearchRequests",
                column: "VectorIndexId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchResults_ChunkId",
                table: "SemanticSearchResults",
                column: "ChunkId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchResults_DocumentId",
                table: "SemanticSearchResults",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchResults_Request_Rank",
                table: "SemanticSearchResults",
                columns: new[] { "SemanticSearchRequestId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_SemanticSearchResults_SearchRequestId",
                table: "SemanticSearchResults",
                column: "SemanticSearchRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_AgentId",
                table: "ToolDefinitions",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_AgentId_Name",
                table: "ToolDefinitions",
                columns: new[] { "AgentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_IsActive",
                table: "ToolDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ToolDefinitions_ToolType",
                table: "ToolDefinitions",
                column: "ToolType");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_AgentExecutionId",
                table: "ToolExecutions",
                column: "AgentExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_CreatedAt",
                table: "ToolExecutions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_Status",
                table: "ToolExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_ToolDefinitionId",
                table: "ToolExecutions",
                column: "ToolDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolExecutions_WorkflowExecutionId",
                table: "ToolExecutions",
                column: "WorkflowExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_VectorIndexes_IsActive",
                table: "VectorIndexes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_VectorIndexes_Name",
                table: "VectorIndexes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VectorIndexes_Provider",
                table: "VectorIndexes",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_VectorIndexes_Status",
                table: "VectorIndexes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_IsActive_IsPublished",
                table: "WorkflowDefinitions",
                columns: new[] { "IsActive", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_Name",
                table: "WorkflowDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_WorkflowType",
                table: "WorkflowDefinitions",
                column: "WorkflowType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_CorrelationId",
                table: "WorkflowExecutions",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_CreatedAt",
                table: "WorkflowExecutions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_Status",
                table: "WorkflowExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_TriggerType",
                table: "WorkflowExecutions",
                column: "TriggerType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowExecutions_WorkflowDefinitionId",
                table: "WorkflowExecutions",
                column: "WorkflowDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "ConversationMessages");

            migrationBuilder.DropTable(
                name: "Embeddings");

            migrationBuilder.DropTable(
                name: "PromptVersions");

            migrationBuilder.DropTable(
                name: "SemanticSearchResults");

            migrationBuilder.DropTable(
                name: "ToolExecutions");

            migrationBuilder.DropTable(
                name: "PromptTemplates");

            migrationBuilder.DropTable(
                name: "EmbeddingChunks");

            migrationBuilder.DropTable(
                name: "SemanticSearchRequests");

            migrationBuilder.DropTable(
                name: "AgentExecutions");

            migrationBuilder.DropTable(
                name: "ToolDefinitions");

            migrationBuilder.DropTable(
                name: "KnowledgeDocuments");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "WorkflowExecutions");

            migrationBuilder.DropTable(
                name: "AgentDefinitions");

            migrationBuilder.DropTable(
                name: "KnowledgeSources");

            migrationBuilder.DropTable(
                name: "AIAssistants");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "KnowledgeBases");

            migrationBuilder.DropTable(
                name: "AIModels");

            migrationBuilder.DropTable(
                name: "VectorIndexes");

            migrationBuilder.DropTable(
                name: "AIProviders");
        }
    }
}
