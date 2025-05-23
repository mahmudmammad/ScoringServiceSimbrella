using InternTask.Controllers;
using InternTask.Interfaces;
using InternTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace InternTask.Tests.Unit
{
    public class ScoringControllerTests
    {
        private readonly Mock<IScoringService> _serviceMock;
        private readonly Mock<ILogger<ScoringController>> _loggerMock;
        private readonly ScoringController _controller;

        public ScoringControllerTests()
        {
            _serviceMock = new Mock<IScoringService>();
            _loggerMock = new Mock<ILogger<ScoringController>>();
            _controller = new ScoringController(_serviceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task EvaluateCustomer_ValidCustomer_ReturnsOkResult()
        {
            var customer = new Customer { Id = 1, Salary = 5000 };
            var scoringResult = new ScoringResult
            {
                IsApproved = true,
                EligibleAmount = 15000,
                Message = "Customer approved for a loan up to $15,000.00"
            };

            _serviceMock.Setup(s => s.EvaluateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(scoringResult);

            var result = await _controller.EvaluateCustomer(customer);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ScoringResult>(okResult.Value);
            Assert.Equal(scoringResult.IsApproved, returnValue.IsApproved);
            Assert.Equal(scoringResult.EligibleAmount, returnValue.EligibleAmount);
            Assert.Equal(scoringResult.Message, returnValue.Message);
            _serviceMock.Verify(s => s.EvaluateCustomerAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task EvaluateCustomer_ServiceReturnsNotApproved_ReturnsOkWithNotApproved()
        {
        
            var customer = new Customer { Id = 1, Salary = 2000 };
            var scoringResult = new ScoringResult
            {
                IsApproved = false,
                EligibleAmount = 0m,
                Message = "Customer not approved"
            };

            _serviceMock.Setup(s => s.EvaluateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(scoringResult);

            
            var result = await _controller.EvaluateCustomer(customer);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ScoringResult>(okResult.Value);
            Assert.False(returnValue.IsApproved);
            Assert.Equal(0m,returnValue.EligibleAmount);
            Assert.Equal("Customer not approved", returnValue.Message);
        }

        [Fact]
        public async Task EvaluateCustomer_NullCustomer_ProcessesCorrectly()
        {
  
            Customer customer = null;
            var scoringResult = new ScoringResult
            {
                IsApproved = false,
                EligibleAmount = 0m,
                Message = "Customer information is missing"
            };

            _serviceMock.Setup(s => s.EvaluateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(scoringResult);

      
            var result = await _controller.EvaluateCustomer(customer);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ScoringResult>(okResult.Value);
            Assert.False(returnValue.IsApproved);
            Assert.Equal("Customer information is missing", returnValue.Message);
        }
    }
}