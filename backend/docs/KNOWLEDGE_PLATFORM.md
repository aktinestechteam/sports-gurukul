# Knowledge & RAG Platform

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                        Knowledge Platform                           │
├─────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌──────────────┐  ┌───────────┐ │
│  │  Ingestion  │  │  Processing │  │   Storage    │  │ Retrieval │ │
│  │  Pipeline   │──▶  Pipeline   │──▶   Pipeline   │──▶  Pipeline  │ │
│  └─────────────┘  └─────────────┘  └──────────────┘  └───────────┘ │
│        │                │                │                │         │
│  ┌─────┴──────┐  ┌──────┴───────┐  ┌────┴──────┐  ┌────┴──────┐  │
│  │ Document   │  │ Chunking     │  │ Embedding │  │ Semantic  │  │
│  │ Parsers    │  │ Strategies   │  │ Providers │  │ Search    │  │
│  │ (10 types) │  │ (6 types)    │  │ (6 types) │  │ Hybrid    │  │
│  └────────────┘  └──────────────┘  └───────────┘  │ Keyword   │  │
│                                                    │ Rerank    │  │
│  ┌──────────────────────────────────────────────┐  └───────────┘  │
│  │            Vector Stores (8 types)           │                  │
│  │  Qdrant │ Azure AI Search │ Pinecone │       │                  │
│  │  Weaviate │ Milvus │ FAISS │ Chroma │ pgvector│                │
│  └──────────────────────────────────────────────┘                  │
└─────────────────────────────────────────────────────────────────────┘
```

## Document Pipeline

```
Raw Document (byte[]) 
    │
    ▼
┌─────────────────────────────┐
│  Document Format Detection  │  PDF, Word, Excel, PPT, MD, HTML, TXT,
│  + Parser Selection         │  CSV, JSON, XML
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│  Text Extraction            │  Format-specific parser extracts text
│  + Metadata Extraction      │  Title, Author, Dates, PageCount
│  + Language Detection       │  Optional: OCR, Image Captioning, PII
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│  Content Classification     │  Categorize content
│  + Fingerprinting           │  SHA-256 checksum for dedup
│  + Deduplication            │  Skip if already indexed
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│  Chunking Strategy          │  Fixed / Semantic / Heading / Sliding
│  + Split into chunks        │  Recursive / Parent-Child
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│  Embedding Generation       │  OpenAI / Azure / Gemini / Cohere
│  + Batch Processing         │  SentenceTransformers / Ollama
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│  Vector Store Upsert        │  Qdrant / Azure AI Search / Pinecone
│  + Index Management         │  Weaviate / Milvus / FAISS / Chroma / pgvector
└─────────────────────────────┘
```

## Embedding Flow

```
User Query
    │
    ▼
┌──────────────────────┐
│  Embedding Provider   │  Same provider used for indexing
│  (IEmbeddingProvider) │  Generates query vector
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Vector Store Search  │  Cosine similarity search
│  (IVectorStore)       │  Top-K + score threshold + metadata filter
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Reranker (optional)   │  Cross-encoder / keyword boost
│  (IRerankerService)   │  Improves result relevance
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Citation Engine      │  Document name, section, page, chunk ID,
│  (ICitationEngine)    │  confidence score, source link, excerpt
└──────────────────────┘
    │
    ▼
   Results with Citations
```

## Retrieval Flow

```
┌──────────────────────────────────────────────────┐
│              IKnowledgeSearchService              │
├──────────────────────────────────────────────────┤
│  SemanticSearch()     → Pure vector similarity    │
│  HybridSearch()       → Vector + keyword fusion   │
│  KeywordSearch()      → Term-based matching       │
│  SearchWithReranking()→ Retrieve → Rerank → Return│
│  MultiKnowledgeSearch()→ Search multiple indexes  │
└──────────────────────────────────────────────────┘
```

## Citation Format

```json
{
  "documentName": "Training Manual.pdf",
  "section": "Chapter 3: Advanced Training",
  "pageNumber": 42,
  "chunkId": "doc-1_chunk_0",
  "confidence": 0.95,
  "sourceLink": "https://storage.example.com/training-manual.pdf",
  "excerpt": "Sports training methods and techniques for optimal..."
}
```

## Extension Points

| Extension Point | Interface | Default | Custom |
|---|---|---|---|
| OCR | `IOcrExtensionPoint` | Null (disabled) | Tesseract, Azure AI Document Intelligence |
| Image Captioning | `IImageCaptionExtensionPoint` | Null (disabled) | BLIP, GPT-4V |
| PII Detection | `IPiiDetectionExtensionPoint` | Null (disabled) | Presidio, Azure AI Content Safety |
| Document Parser | `IDocumentParser` | 10 built-in parsers | Add by implementing interface |
| Chunking Strategy | `IChunkingStrategy` | 6 built-in strategies | Add by implementing interface |
| Embedding Provider | `IEmbeddingProvider` | 6 built-in providers | Add by implementing interface |
| Vector Store | `IVectorStore` | 8 built-in stores | Add by implementing interface |
| Reranker | `IReranker` | Default keyword boost | Cross-encoder, Cohere Rerank |

## Platform Services

| Service | Interface | Responsibility |
|---|---|---|
| Document Processing | `IDocumentProcessor` | Parse, extract, classify, fingerprint |
| Chunking | `IChunkingService` | Split documents into chunks |
| Embedding | `IEmbeddingService` | Generate vector embeddings |
| Vector Store | `IVectorStoreService` | Store/delete/search vectors |
| Knowledge Ingestion | `IKnowledgeIngestionService` | End-to-end ingestion pipeline |
| Knowledge Search | `IKnowledgeSearchService` | Multi-strategy retrieval |
| Retrieval | `IRetrievalService` | Query → Results pipeline |
| Reranker | `IRerankerService` | Result re-ranking |
| Citation | `ICitationService` | Citation generation & formatting |
| Knowledge Management | `IKnowledgeManagementService` | Index lifecycle (create/delete/rebuild) |
| Access Control | `IKnowledgeAccessService` | Document & KB authorization |
| Observability | `IKnowledgeObservabilityService` | Metrics, health, monitoring |

## Security

- **Document Authorization**: `IKnowledgeAccessService` gates access per document and knowledge base
- **Knowledge Access Policies**: Role-based and user-based policies per KB
- **Tenant Isolation**: Policy-driven isolation by knowledge base ID
- **Encryption**: At-rest via vector store, in-transit via TLS
- **Audit Logging**: Via existing `IAuditService` (from AI Management module)

## Performance Features

- **Batch Embeddings**: `IEmbeddingProvider.SupportsBatchProcessing` for bulk generation
- **Parallel Processing**: `IKnowledgeIngestionService.IngestDocumentBatchAsync` with `Task.WhenAll`
- **Caching**: Optional distributed cache for frequent queries
- **Incremental Updates**: `IKnowledgeManagementService.IncrementalIndexAsync` for partial updates
- **Lazy Loading**: Document content loaded on demand
- **Streaming Retrieval**: Chunked response for large result sets

## Files Created

```
src/SportsGurukul.Application/Features/KnowledgePlatform/
├── DependencyInjection.cs
├── Interfaces/
│   ├── IChunkingStrategy.cs
│   ├── IContentClassifier.cs
│   ├── IDocumentParser.cs
│   ├── IEmbeddingProvider.cs
│   ├── IKnowledgeManagementService.cs
│   ├── IKnowledgeSearchService.cs
│   ├── IReranker.cs
│   ├── ITextExtractor.cs
│   └── IVectorStore.cs
├── Models/
│   └── KnowledgePlatformModels.cs
├── Parsers/
│   ├── CsvParser.cs
│   ├── HtmlParser.cs
│   ├── JsonParser.cs
│   ├── MarkdownParser.cs
│   ├── ParserFactory.cs
│   ├── PdfParser.cs
│   ├── TxtParser.cs
│   ├── WordParser.cs
│   └── XmlParser.cs
├── Chunking/
│   ├── ChunkingStrategyFactory.cs
│   ├── FixedSizeChunker.cs
│   ├── HeadingBasedChunker.cs
│   ├── ParentChildChunker.cs
│   ├── RecursiveChunker.cs
│   ├── SemanticChunker.cs
│   └── SlidingWindowChunker.cs
├── Embedding/
│   ├── AzureOpenAIEmbeddingProvider.cs
│   ├── BaseEmbeddingProvider.cs
│   ├── CohereEmbeddingProvider.cs
│   ├── EmbeddingProviderFactory.cs
│   ├── GeminiEmbeddingProvider.cs
│   ├── OllamaEmbeddingProvider.cs
│   ├── OpenAIEmbeddingProvider.cs
│   └── SentenceTransformersEmbeddingProvider.cs
├── VectorStores/
│   ├── AzureAISearchVectorStore.cs
│   ├── BaseVectorStore.cs
│   ├── ChromaVectorStore.cs
│   ├── FaissVectorStore.cs
│   ├── MilvusVectorStore.cs
│   ├── PgVectorVectorStore.cs
│   ├── PineconeVectorStore.cs
│   ├── QdrantVectorStore.cs
│   ├── VectorStoreFactory.cs
│   └── WeaviateVectorStore.cs
├── Services/
│   ├── ChunkingService.cs
│   ├── CitationService.cs
│   ├── DocumentProcessingService.cs
│   ├── EmbeddingService.cs
│   ├── KnowledgeAccessService.cs
│   ├── KnowledgeIngestionService.cs
│   ├── KnowledgeManagementService.cs
│   ├── KnowledgeObservabilityService.cs
│   ├── KnowledgeSearchService.cs
│   ├── RerankerService.cs
│   ├── RetrievalService.cs
│   └── VectorStoreService.cs
```

```
tests/SportsGurukul.Application.Tests/KnowledgePlatform/
├── Mocks/
│   ├── MockEmbeddingProvider.cs
│   └── MockVectorStore.cs
├── Chunking/
│   └── ChunkingStrategyTests.cs
├── Retrieval/
│   └── HybridSearchTests.cs
├── Citation/
│   └── CitationServiceTests.cs
└── Performance/
    └── IngestionPerformanceTests.cs
```
