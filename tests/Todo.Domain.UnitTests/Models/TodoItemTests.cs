using System;
using FluentAssertions;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Domain.UnitTests.Models;

public class TodoItemTests
{
    #region Tests de modification (SetName)

    [Fact]
    public void SetName_WithValidName_ShouldUpdateTodoName()
    {
        // ARRANGE
        var todo = new TodoItem("Ancien nom");
        var nouveauNom = "Nouveau nom";

        // ACT
        todo.SetName(nouveauNom);

        // ASSERT
        todo.Name.Should().Be(nouveauNom);
    }

    [Fact]
    public void SetName_WithValidName_ShouldTrimWhitespace()
    {
        // ARRANGE
        var todo = new TodoItem("Ancien nom");
        var nomAvecEspaces = "  Nouveau nom  ";

        // ACT
        todo.SetName(nomAvecEspaces);

        // ASSERT
        todo.Name.Should().Be("Nouveau nom");
    }

    [Fact]
    public void SetName_WithNameTooShort_ShouldThrowArgumentException()
    {
        // ARRANGE
        var todo = new TodoItem("Nom original");
        var nomTropCourt = "a";

        // ACT
        Action act = () => todo.SetName(nomTropCourt);

        // ASSERT
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom doit contenir au minimum 2 caractères");
    }

    [Fact]
    public void SetName_WithNameTooLong_ShouldThrowArgumentException()
    {
        // ARRANGE
        var todo = new TodoItem("Nom original");
        var nomTropLong = new string('a', 101);

        // ACT
        Action act = () => todo.SetName(nomTropLong);

        // ASSERT
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom doit contenir au maximum 100 caractères");
    }

    [Fact]
    public void SetName_WithEmptyName_ShouldThrowArgumentException()
    {
        // ARRANGE
        var todo = new TodoItem("Nom original");

        // ACT & ASSERT - Nom vide
        Action actVide = () => todo.SetName("");
        actVide.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom ne peut pas être vide");

        // ACT & ASSERT - Nom null
        Action actNull = () => todo.SetName(null);
        actNull.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom ne peut pas être vide");
    }

    [Fact]
    public void SetName_ShouldUpdateUpdatedAtTimestamp()
    {
        // ARRANGE
        var todo = new TodoItem("Nom original");
        var dateAvant = DateTime.UtcNow;

        // ACT
        Task.Delay(10).Wait();
        todo.SetName("Nouveau nom");

        // ASSERT
        todo.UpdatedAt.Should().NotBeNull();
        todo.UpdatedAt.Should().BeAfter(dateAvant);
    }

    [Fact]
    public void SetName_WithSameName_ShouldStillUpdateTimestamp()
    {
        // ARRANGE
        var nom = "Nom identique";
        var todo = new TodoItem(nom);
        var dateAvant = DateTime.UtcNow;

        // ACT
        Task.Delay(10).Wait();
        todo.SetName(nom);

        // ASSERT
        todo.UpdatedAt.Should().BeAfter(dateAvant);
    }

    #endregion

    #region Tests de création (Constructeur)

    [Fact]
    public void Constructor_WithValidName_ShouldCreateTodo()
    {
        // ARRANGE
        var nom = "Faire les courses";

        // ACT
        var todo = new TodoItem(nom);

        // ASSERT
        todo.Id.Should().NotBeEmpty();
        todo.Name.Should().Be(nom);
        todo.IsCompleted.Should().BeFalse();
        todo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todo.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string nomInvalide)
    {
        // ACT
        Action act = () => new TodoItem(nomInvalide);

        // ASSERT
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom ne peut pas être vide");
    }

    [Fact]
    public void Constructor_WithNameTooShort_ShouldThrowArgumentException()
    {
        // ACT
        Action act = () => new TodoItem("a");

        // ASSERT
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom doit contenir au minimum 2 caractères");
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrowArgumentException()
    {
        // ARRANGE
        var longName = new string('a', 101);

        // ACT
        Action act = () => new TodoItem(longName);

        // ASSERT
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Le nom doit contenir au maximum 100 caractères");
    }

    #endregion

    #region Tests de changement de statut

    [Fact]
    public void MarkAsCompleted_ShouldSetIsCompletedToTrue()
    {
        // ARRANGE
        var todo = new TodoItem("Faire les courses");
        var dateAvant = DateTime.UtcNow;

        // ACT
        Task.Delay(10).Wait();
        todo.MarkAsCompleted();

        // ASSERT
        todo.IsCompleted.Should().BeTrue();
        todo.UpdatedAt.Should().BeAfter(dateAvant);
    }

    [Fact]
    public void MarkAsNotCompleted_ShouldSetIsCompletedToFalse()
    {
        // ARRANGE
        var todo = new TodoItem("Faire les courses");
        todo.MarkAsCompleted();
        var dateAvant = DateTime.UtcNow;

        // ACT
        Task.Delay(10).Wait();
        todo.MarkAsNotCompleted();

        // ASSERT
        todo.IsCompleted.Should().BeFalse();
        todo.UpdatedAt.Should().BeAfter(dateAvant);
    }

    [Fact]
    public void ToggleComplete_WhenNotCompleted_ShouldMarkAsCompleted()
    {
        // ARRANGE
        var todo = new TodoItem("Faire les courses");

        // ACT
        todo.ToggleComplete();

        // ASSERT
        todo.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void ToggleComplete_WhenCompleted_ShouldMarkAsNotCompleted()
    {
        // ARRANGE
        var todo = new TodoItem("Faire les courses");
        todo.MarkAsCompleted();

        // ACT
        todo.ToggleComplete();

        // ASSERT
        todo.IsCompleted.Should().BeFalse();
    }

    #endregion
}