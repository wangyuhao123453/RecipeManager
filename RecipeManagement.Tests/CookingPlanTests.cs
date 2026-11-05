using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// A3 - the cooking plan as a LinkedList, including duplicate prevention.
/// </summary>
public sealed class CookingPlanTests
{
    [Fact]
    public void AddRecipeToCookingPlan_AppendsRecipesInPlanOrder()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(2));
        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.True(manager.AddRecipeToCookingPlan(3));

        Assert.Equal(new[] { 2, 1, 3 }, manager.GetCookingPlan());
        Assert.Equal(3, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_ReturnsFalseForMissingRecipe()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.False(manager.AddRecipeToCookingPlan(999));
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_RejectsADuplicatePlanEntry()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        Assert.True(manager.AddRecipeToCookingPlan(1));

        Assert.False(manager.AddRecipeToCookingPlan(1));
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_RemovesTheEntryAndKeepsTheOrder()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.AddRecipeToCookingPlan(3);

        Assert.True(manager.RemoveRecipeFromCookingPlan(2));
        Assert.Equal(new[] { 1, 3 }, manager.GetCookingPlan());
        Assert.Equal(2, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_ReturnsFalseWhenTheRecipeIsNotPlanned()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RemoveRecipeFromCookingPlan(2));
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());
    }

    [Fact]
    public void GetCookingPlan_ReturnsACopySoCallersCannotEditThePlan()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        IReadOnlyList<int> snapshot = manager.GetCookingPlan();

        manager.AddRecipeToCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.Equal(new[] { 1 }, snapshot);
    }
}
