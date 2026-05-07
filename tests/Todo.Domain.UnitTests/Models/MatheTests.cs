using System;
using FluentAssertions;
using Todo.Domain.Models;
using Xunit;
using Moq;

namespace Todo.Domain.UnitTests.Models;

public class MatheTests
{
    
[Fact]
    public void Factorial_Should_Throw_OutOfRangeException_When_IsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Mathe.Factorial(-2));
    }

    [Fact]
    public void Factorial_Should_Return_1_Given_0()
    {
        Assert.Equal(1, Mathe.Factorial(0));
    }
    [Fact]
    public void Factorial_Should_Return_1_Given_1()
    {
        Assert.Equal(1, Mathe.Factorial(1));
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    public void Factorial_Should_Return_Right_Value(int value, int expected)
    {
        Assert.Equal(Mathe.Factorial(value), expected);
    }

    [Fact]
    public void Factorial_Should_Throw_OverflowException_When_GreaterThan_20()
    {
        Assert.Throws<OverflowException>(() => Mathe.Factorial(21));
    }

    [Fact]
    public void Factorial_Should_Return_Value_When_WorkingToday()
    {
        var sut = new MatheService(new FakeExternalServiceWorkingToday(true));
        Assert.Equal(1, sut.Factorial(0));
    }
    [Fact]
    public void Factorial_Should_Throw_When_Not_WorkingToday()
    {
        var sut = new MatheService(new FakeExternalServiceWorkingToday(false));
        Assert.Throws<Exception>(() => sut.Factorial(0));
    }


    [Fact]
    public void Factorial_Should_Return_Value_When_WorkingToday2()
    {
        var sut = new MatheService(new FakeExternalServiceWorkingToday(true));
        Assert.Equal(1, sut.Factorial(0));
    }
    [Fact]
    public void Factorial_Should_Throw_When_Not_WorkingToday2()
    {
        var sut = new MatheService( new FakeExternalServiceWorkingToday(false));
        Assert.Throws<Exception>(() => sut.Factorial(0));
    }

    [Fact]
    public void Factorial_Should_Return_Value_When_WorkToday_Is_True()
    {
        var mockExternalService = new Mock<IExternalService>();

        mockExternalService
            .Setup(s => s.WorkToday())
            .Returns(true);

        var service = new MatheService(mockExternalService.Object);

        var result = service.Factorial(5);

        Assert.Equal(120, result);

        mockExternalService.Verify(
            s => s.WorkToday(),
            Times.Once
        );
    }

    [Fact]
    public void Factorial_Should_Throw_When_WorkToday_Is_False()
    {
        
        var mockExternalService = new Mock<IExternalService>();

        
        mockExternalService
            .Setup(s => s.WorkToday())
            .Returns(false);

        
        var service = new MatheService(mockExternalService.Object);

        
        Assert.Throws<Exception>(() => service.Factorial(5));

        
        mockExternalService.Verify(
            s => s.WorkToday(),
            Times.Once
        );
    }
}
