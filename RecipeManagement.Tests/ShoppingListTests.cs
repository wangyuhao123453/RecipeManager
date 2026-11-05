using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// A2 - the shopping list, a List&lt;string&gt; of copied ingredient lines.
/// </summary>
public sealed class ShoppingListTests
{
    [Fact]
    public void AddIngredients_CopiesTheOriginalStringsInOrder()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        int added = manager.AddIngredientsToShoppingList(2);

        Assert.Equal(2, added);
        Assert.Equal(new[] { "1 onion", "2 carrots" }, manager.GetShoppingList());
        Assert.Equal(2, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddIngredients_AppendsRatherThanReplaces()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddIngredientsToShoppingList(1);

        manager.AddIngredientsToShoppingList(2);

        Assert.Equal(
            new[] { "500g chicken", "2 tbsp curry paste", "1 onion", "2 carrots" },
            manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_ReturnsZeroForMissingRecipeAndChangesNothing()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        int added = manager.AddIngredientsToShoppingList(999);

        Assert.Equal(0, added);
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_CountsOnlyTheIngredientsOfOneRecipe()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.Equal(2, manager.AddIngredientsToShoppingList(1));
        Assert.Equal(2, manager.ShoppingItemCount);
    }

    [Fact]
    public void ClearShoppingList_RemovesEveryItem()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddIngredientsToShoppingList(1);
        manager.AddIngredientsToShoppingList(2);

        manager.ClearShoppingList();

        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());

        // The list is still usable after clearing.
        Assert.Equal(2, manager.AddIngredientsToShoppingList(2));
    }

    [Fact]
    public void GetShoppingList_ReturnsACopySoLaterChangesDoNotLeakOut()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.AddIngredientsToShoppingList(1);
        IReadOnlyList<string> snapshot = manager.GetShoppingList();

        manager.AddIngredientsToShoppingList(2);
        manager.ClearShoppingList();

        Assert.Equal(2, snapshot.Count);
        Assert.Equal("500g chicken", snapshot[0]);
    }
}
