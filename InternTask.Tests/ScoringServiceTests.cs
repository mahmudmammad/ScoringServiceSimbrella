using InternTask.Interfaces;
using InternTask.Models;
using InternTask.Entities;
using InternTask.DB;
using InternTask.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ScoringServiceTests
{
    private readonly Mock<ICondition> _conditionMock;
    private readonly Mock<ScoringMetricsService> _metricsMock;
    private readonly Mock<ILogger<ScoringService>> _loggerMock;
    private readonly ScoringDbContext _dbContext;
    private readonly ScoringService _service;

    public ScoringServiceTests()
    {
        _conditionMock = new Mock<ICondition>();
        _metricsMock = new Mock<ScoringMetricsService>();
        _loggerMock = new Mock<ILogger<ScoringService>>();

        var options = new DbContextOptionsBuilder<ScoringDbContext>()
            .UseInMemoryDatabase(databaseName: "ScoringDb")
            .Options;
        _dbContext = new ScoringDbContext(options);

        _service = new ScoringService(
            new List<ICondition> { _conditionMock.Object },
            _loggerMock.Object,
            _dbContext,
            _metricsMock.Object
        );
    }

    [Fact]
    public async Task EvaluateCustomerAsync_ValidCustomerAndPassedCondition_ReturnsApprovedResult()
    {
        var customer = new Customer { Id = 1, Salary = 5000 };

        var conditionResult = new ConditionResult
        {
            Passed = true,
            EligibleAmount = 10000,
            Message = "Condition passed"
        };

        _conditionMock.Setup(c => c.Evaluate(It.IsAny<Customer>()))
            .Returns(conditionResult);
        _conditionMock.Setup(c => c.IsRequired).Returns(true);
        _conditionMock.Setup(c => c.Id).Returns("1");
        _conditionMock.Setup(c => c.Name).Returns("MockCondition");
        _conditionMock.Setup(c => c.Priority).Returns(1);
        _conditionMock.Setup(c => c.GetConfiguration())
            .Returns(new Dictionary<string, object>());

        var result = await _service.EvaluateCustomerAsync(customer);

        Assert.True(result.IsApproved);
        Assert.Equal(10000, result.EligibleAmount);
        Assert.Contains("approved", result.Message.ToLower());
    }

    [Fact]
    public async Task EvaluateCustomerAsync_NullCustomer_ReturnsNotApprovedResult()
    {
        var result = await _service.EvaluateCustomerAsync(null);

        Assert.False(result.IsApproved);
        Assert.Equal(result.EligibleAmount, 0);
        Assert.Equal("Customer information is missing", result.Message);
    }

    [Fact]
    public async Task EvaluateCustomerAsync_RequiredConditionFails_ReturnsNotApproved()
    {
        
        var customer = new Customer { Id = 2, Salary = 3000 };

        var failedCondition = new ConditionResult
        {
            Passed = false,
            Message = "Condition failed",
            EligibleAmount = 0m 
        };

        _conditionMock.Setup(c => c.Evaluate(It.IsAny<Customer>()))
            .Returns(failedCondition);
        _conditionMock.Setup(c => c.IsRequired).Returns(true);
        _conditionMock.Setup(c => c.Id).Returns("2");
        _conditionMock.Setup(c => c.Name).Returns("RequiredCondition");
        _conditionMock.Setup(c => c.Priority).Returns(1);
        _conditionMock.Setup(c => c.GetConfiguration())
            .Returns(new Dictionary<string, object>());

        
        var result = await _service.EvaluateCustomerAsync(customer);

        Assert.False(result.IsApproved);
        Assert.Equal("Customer not approved", result.Message);
    }

    [Fact]
    public async Task EvaluateCustomerAsync_ConditionThrowsException_HandlesGracefully()
    {
        
        var customer = new Customer { Id = 3, Salary = 4500 };

        _conditionMock.Setup(c => c.Evaluate(It.IsAny<Customer>()))
            .Throws(new System.Exception("Unexpected error"));
        _conditionMock.Setup(c => c.IsRequired).Returns(false);
        _conditionMock.Setup(c => c.Id).Returns("3");
        _conditionMock.Setup(c => c.Name).Returns("ExceptionCondition");
        _conditionMock.Setup(c => c.Priority).Returns(1);
        _conditionMock.Setup(c => c.GetConfiguration())
            .Returns(new Dictionary<string, object>());

        var result = await _service.EvaluateCustomerAsync(customer);

        Assert.False(result.IsApproved);
        Assert.Equal("Customer not approved", result.Message);
        Assert.Single(result.ConditionResults);
        Assert.False(result.ConditionResults.First().Result.Passed);
        Assert.Contains("Error during evaluation", result.ConditionResults.First().Result.Message);
    }
}
