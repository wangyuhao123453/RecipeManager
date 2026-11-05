using System;
using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// A1 - the catalogue dictionary: build, add, find, remove and the invalid cases.
/// </summary>
public sealed class RecipeCatalogueTests
{
    [Fact]
    public void Constructor_LoadsEveryRecipeIntoTheCatalogue()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.Equal(3, manager.RecipeCount);
        Assert.NotNull(manager.FindRecipe(1));
        Assert.NotNull(manager.FindRecipe(3));
    }

    [Fact]
    public void Constructor_RejectsNullRecipeCollection()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_RejectsDuplicateId()
    {
        var first = TestRecipes.Curry();
        var second = new Recipe { Id = first.Id, Title = "Another curry" };

        Assert.Throws<ArgumentException>(() => new RecipeManager(new[] { first, second }));
    }

    [Fact]
    public void Constructor_RejectsNonPositiveId()
    {
        var recipe = new Recipe { Id = 0, Title = "No ID" };

        Assert.Throws<ArgumentException>(() => new RecipeManager(new[] { recipe }));
    }

    [Fact]
    public void Constructor_RejectsBlankTitle()
    {
        var recipe = new Recipe { Id = 7, Title = "   " };

        Assert.Throws<ArgumentException>(() => new RecipeManager(new[] { recipe }));
    }

    [Fact]
    public void AddRecipe_RejectsNull()
    {
        RecipeManager manager = TestRecipes.CreateEmptyManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Fact]
    public void AddRecipe_ReturnsFalseForInvalidRecipe()
    {
        RecipeManager manager = TestRecipes.CreateEmptyManager();

        Assert.False(manager.AddRecipe(new Recipe { Id = -4, Title = "Negative ID" }));
        Assert.False(manager.AddRecipe(new Recipe { Id = 4, Title = string.Empty }));
        Assert.Equal(0, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_ReturnsFalseForDuplicateIdAndKeepsTheOriginal()
    {
        RecipeManager manager = TestRecipes.CreateEmptyManager();
        Assert.True(manager.AddRecipe(TestRecipes.Curry()));

        bool added = manager.AddRecipe(new Recipe { Id = 1, Title = "Curry again" });

        Assert.False(added);
        Assert.Equal(1, manager.RecipeCount);
        Assert.Equal("Chicken Curry", manager.FindRecipe(1)?.Title);
    }

    [Fact]
    public void AddRecipe_StoresANewRecipeThatCanBeFound()
    {
        RecipeManager manager = TestRecipes.CreateEmptyManager();
        var recipe = new Recipe { Id = 42, Title = "Lentil Soup" };

        Assert.True(manager.AddRecipe(recipe));

        Assert.Equal(1, manager.RecipeCount);
        Assert.Same(recipe, manager.FindRecipe(42));
    }

    [Fact]
    public void FindRecipe_ReturnsNullForMissingId()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.Null(manager.FindRecipe(999));
        Assert.Null(manager.FindRecipe(0));
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalseForMissingId()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.False(manager.RemoveRecipe(999));
        Assert.Equal(3, manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_RemovesAnUnplannedRecipe()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.True(manager.RemoveRecipe(2));

        Assert.Equal(2, manager.RecipeCount);
        Assert.Null(manager.FindRecipe(2));
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalseWhileTheRecipeIsInTheCookingPlan()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        Assert.True(manager.AddRecipeToCookingPlan(1));

        Assert.False(manager.RemoveRecipe(1));
        Assert.Equal(3, manager.RecipeCount);

        // Once it leaves the plan, removal is allowed again.
        Assert.True(manager.RemoveRecipeFromCookingPlan(1));
        Assert.True(manager.RemoveRecipe(1));
    }

    [Fact]
    public void RemoveRecipe_AllowsTheSameIdToBeAddedBack()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.RemoveRecipe(2);

        Assert.True(manager.AddRecipe(new Recipe { Id = 2, Title = "New soup" }));
        Assert.Equal("New soup", manager.FindRecipe(2)?.Title);
    }
}
