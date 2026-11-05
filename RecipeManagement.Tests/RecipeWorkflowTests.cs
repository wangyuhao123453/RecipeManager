using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// A5 - one recipe moving through catalogue, shopping list, cooking plan and
/// instruction queue, plus the two components that interact directly.
/// </summary>
public sealed class RecipeWorkflowTests
{
    [Fact]
    public void ARecipeCanTravelThroughTheWholePartAWorkflow()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        // 1. Catalogue lookup.
        Recipe found = Assert.IsType<Recipe>(manager.FindRecipe(1));
        Assert.Equal("Chicken Curry", found.Title);

        // 2. Shopping list.
        Assert.Equal(2, manager.AddIngredientsToShoppingList(found.Id));

        // 3. Cooking plan.
        Assert.True(manager.AddRecipeToCookingPlan(found.Id));

        // 4. Cooking session drains the instruction queue in order.
        Assert.True(manager.StartCooking(found.Id));
        Assert.Equal("Brown the chicken", manager.CompleteNextInstruction());
        Assert.Equal("Stir in the curry paste", manager.CompleteNextInstruction());
        Assert.Equal("Simmer for 20 minutes", manager.CompleteNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());

        Assert.Equal(2, manager.ShoppingItemCount);
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Equal(3, manager.RecipeCount);
    }

    [Fact]
    public void PlanAndHistoryInteractThroughAppendRemoveRestore()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);

        manager.RemoveRecipeFromCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(1);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(0, manager.CookingPlanCount);

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());

        Assert.Equal(new[] { 1, 2 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemovingARecipeFromThePlanReleasesTheCatalogueEntry()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddRecipeToCookingPlan(3);

        Assert.False(manager.RemoveRecipe(3));
        manager.RemoveRecipeFromCookingPlan(3);
        Assert.True(manager.RemoveRecipe(3));

        // The ID stays in the history, but the recipe itself is gone.
        Assert.Null(manager.FindRecipe(3));
        Assert.Equal(3, manager.PeekLastRemovedRecipe());
        Assert.False(manager.StartCooking(3));
    }

    [Fact]
    public void MissingRecipeIsHandledSafelyByEveryPartAOperation()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        const int missingId = 999;

        Assert.Null(manager.FindRecipe(missingId));
        Assert.Equal(0, manager.AddIngredientsToShoppingList(missingId));
        Assert.False(manager.AddRecipeToCookingPlan(missingId));
        Assert.False(manager.RemoveRecipeFromCookingPlan(missingId));
        Assert.False(manager.RemoveRecipe(missingId));
        Assert.False(manager.StartCooking(missingId));
    }
}
