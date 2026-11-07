using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Holds the recipe catalogue and the Part A working collections.
/// Catalogue: Dictionary. Shopping list: List. Cooking plan: LinkedList.
/// Removed history: Stack. Instruction queue: Queue.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // Part A collections, one field per nominated role.
    private readonly Dictionary<int, Recipe> _catalogue = new();
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _instructionQueue = new();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(recipes);

        foreach (Recipe recipe in recipes)
        {
            if (recipe is null)
            {
                throw new ArgumentException("Recipe collection contains a null entry.", nameof(recipes));
            }

            RequireCatalogueReady(recipe);

            // A duplicate ID is rejected rather than silently overwriting the first one.
            if (_catalogue.TryAdd(recipe.Id, recipe) == false)
            {
                throw new ArgumentException(
                    $"Recipe ID {recipe.Id} appears more than once.",
                    nameof(recipes));
            }
        }
    }

    public int RecipeCount => _catalogue.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (IsCatalogueReady(recipe) == false)
        {
            return false;
        }

        return _catalogue.TryAdd(recipe.Id, recipe);
    }

    public Recipe? FindRecipe(int recipeId) =>
        _catalogue.TryGetValue(recipeId, out Recipe? recipe) ? recipe : null;

    public bool RemoveRecipe(int recipeId)
    {
        // Two guards, either one means "do not remove":
        // 1) the ID must actually exist in the catalogue, and
        // 2) the recipe must not be sitting in the cooking plan - a planned
        //    recipe can only be deleted after it has been removed from the plan.
        if (_catalogue.ContainsKey(recipeId) == false || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        return _catalogue.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe is null)
        {
            return 0;
        }

        int added = 0;
        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
            added++;
        }

        return added;
    }

    public IReadOnlyList<string> GetShoppingList() =>
        new List<string>(_shoppingList).AsReadOnly();

    public void ClearShoppingList() => _shoppingList.Clear();

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (_catalogue.ContainsKey(recipeId) == false || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        LinkedListNode<int>? node = _cookingPlan.Find(recipeId);
        if (node is null)
        {
            return false;
        }

        _cookingPlan.Remove(node);
        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        // Nothing to restore when the history stack is empty.
        if (_removedRecipes.Count == 0)
        {
            return false;
        }

        // Pop the most recently removed ID first. It leaves the history stack
        // no matter what happens next: the restoration only succeeds when the
        // recipe is still in the catalogue AND is not already back in the plan.
        int recipeId = _removedRecipes.Pop();
        if (_catalogue.ContainsKey(recipeId) == false || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        // Success: the recipe goes back onto the end of the cooking plan.
        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe() =>
        _removedRecipes.Count == 0 ? null : _removedRecipes.Peek();

    public IReadOnlyList<int> GetCookingPlan() =>
        new List<int>(_cookingPlan).AsReadOnly();

    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe is null || recipe.Instructions.Count == 0)
        {
            return false;
        }

        // A new session replaces whatever was left in the previous queue.
        _instructionQueue.Clear();
        foreach (string instruction in recipe.Instructions)
        {
            _instructionQueue.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction() =>
        _instructionQueue.Count == 0 ? null : _instructionQueue.Peek();

    public string? CompleteNextInstruction() =>
        _instructionQueue.Count == 0 ? null : _instructionQueue.Dequeue();

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");

    // A recipe may enter the catalogue only with a positive ID and a real title.
    private static bool IsCatalogueReady(Recipe recipe) =>
        recipe.Id > 0 && string.IsNullOrWhiteSpace(recipe.Title) == false;

    private static void RequireCatalogueReady(Recipe recipe)
    {
        if (recipe.Id <= 0)
        {
            throw new ArgumentException($"Recipe ID must be positive, but was {recipe.Id}.");
        }

        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ArgumentException($"Recipe {recipe.Id} must have a title.");
        }
    }
}
