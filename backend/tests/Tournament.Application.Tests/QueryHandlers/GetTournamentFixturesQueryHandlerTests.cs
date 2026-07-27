using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentFixtures;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using TournamentTestShared;

namespace Tournament.Application.Tests.QueryHandlers;

public class GetTournamentFixturesQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ILogger<GetTournamentFixturesQueryHandler>> _loggerMock;
    private readonly GetTournamentFixturesQueryHandler _handler;

    public GetTournamentFixturesQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _loggerMock = MockLoggerBuilder.Create<GetTournamentFixturesQueryHandler>();
        _handler = new GetTournamentFixturesQueryHandler(
            _contextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var fixtures = new List<TournamentFixture>
        {
            TestDataBuilder.CreateFixture(tournamentId),
            TestDataBuilder.CreateFixture(tournamentId)
        };

        SetupDbSet(fixtures);

        var query = new GetTournamentFixturesQuery
        {
            TournamentId = tournamentId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FilterByStageId_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var stageId = Guid.NewGuid();
        var fixture = TestDataBuilder.CreateFixture(tournamentId);
        fixture.TournamentStageId = stageId;

        var fixtures = new List<TournamentFixture> { fixture };

        SetupDbSet(fixtures);

        var query = new GetTournamentFixturesQuery
        {
            TournamentId = tournamentId,
            StageId = stageId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
    }

    private void SetupDbSet(List<TournamentFixture> data)
    {
        var asyncProvider = new InMemoryAsyncQueryProvider<TournamentFixture>(data);

        var queryable = new InMemoryAsyncEnumerable<TournamentFixture>(data, asyncProvider);

        var dbSetMock = new Mock<DbSet<TournamentFixture>>();
        dbSetMock.As<IAsyncEnumerable<TournamentFixture>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<TournamentFixture>(data.GetEnumerator()));
        dbSetMock.As<IQueryable<TournamentFixture>>()
            .Setup(m => m.Provider).Returns(asyncProvider);
        dbSetMock.As<IQueryable<TournamentFixture>>()
            .Setup(m => m.Expression).Returns(Expression.Constant(queryable));
        dbSetMock.As<IQueryable<TournamentFixture>>()
            .Setup(m => m.ElementType).Returns(typeof(TournamentFixture));
        dbSetMock.As<IQueryable<TournamentFixture>>()
            .Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        _contextMock
            .Setup(c => c.TournamentFixtures)
            .Returns(dbSetMock.Object);
    }

    private class InMemoryAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IEnumerable<T> _data;
        public InMemoryAsyncQueryProvider(IEnumerable<T> data) => _data = data;

        public IQueryable CreateQuery(Expression expression)
        {
            var result = Expression.Lambda<Func<IEnumerable<T>>>(expression).Compile()();
            return new InMemoryAsyncEnumerable<T>(result, this);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var innerQuery = _data.AsQueryable().Provider.CreateQuery<TElement>(expression);
            return new InMemoryAsyncEnumerable<TElement>(innerQuery, this);
        }

        public object Execute(Expression expression) => ExecuteInternal(expression);
        public TResult Execute<TResult>(Expression expression) => (TResult)ExecuteInternal(expression);

        private object ExecuteInternal(Expression expression)
        {
            var compiled = Expression.Lambda<Func<IEnumerable<T>>>(Expression.Quote(expression)).Compile();
            return compiled.Invoke().ToList();
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = ExecuteInternal(expression);
            var expectedResultType = typeof(TResult);

            if (expectedResultType.IsGenericType && expectedResultType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var taskResult = typeof(Task).GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(expectedResultType.GenericTypeArguments[0])
                    .Invoke(null, new[] { result })!;
                return (TResult)taskResult;
            }

            return (TResult)result;
        }
    }

    private class InMemoryAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>, IOrderedQueryable<T>
    {
        private readonly IAsyncQueryProvider _asyncProvider;

        public InMemoryAsyncEnumerable(IEnumerable<T> inner, IAsyncQueryProvider asyncProvider) : base(inner)
        {
            _asyncProvider = asyncProvider;
        }

        public InMemoryAsyncEnumerable(Expression expression, IAsyncQueryProvider asyncProvider) : base(expression)
        {
            _asyncProvider = asyncProvider;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(((IEnumerable<T>)this).GetEnumerator());

        IQueryProvider IQueryable.Provider => _asyncProvider;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    }
}
