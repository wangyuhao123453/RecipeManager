using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Removed-recipe history: the Stack that restores the most recent removal first.
/// </summary>
public sealed class RemovedHistoryTests
{
    [Fact]
    public void RemoveRecipeFromCookingPlan_PushesTheIdOntoTheHistory()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);

        manager.RemoveRecipeFromCookingPlan(1);

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void HistoryIsLastInFirstOut()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.AddRecipeToCookingPlan(3);

        manager.RemoveRecipeFromCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(3);

        Assert.Equal(3, manager.PeekLastRemovedRecipe());

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2, 3 }, manager.GetCookingPlan());

        Assert.Equal(1, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void FailedRemovalDoesNotChangeTheHistory()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.False(manager.RemoveRecipeFromCookingPlan(2));

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void EmptyHistory_PeekReturnsNullAndRestoreReturnsFalse()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void PeekLastRemovedRecipe_DoesNotRemoveTheId()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_AppendsToTheEndOfThePlan()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.True(manager.RestoreLastRemovedRecipe());

        Assert.Equal(new[] { 2, 1 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_ReturnsFalseWhenTheRecipeWasDeletedFromTheCatalogue()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);
        Assert.True(manager.RemoveRecipe(1));

        Assert.False(manager.RestoreLastRemovedRecipe());

        Assert.Equal(0, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_ReturnsFalseWhenTheIdIsAlreadyBackInThePlan()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void FailedRestoreAdvancesTheStackSoTheNextRestoreCanStillSucceed()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);

        manager.RemoveRecipeFromCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(1);
        Assert.True(manager.RemoveRecipe(1));

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(2, manager.PeekLastRemovedRecipe());

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2 }, manager.GetCookingPlan());
    }
}
