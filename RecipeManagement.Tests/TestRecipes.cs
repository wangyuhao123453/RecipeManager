using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Small in-memory recipes used by the student tests, so no test depends on the
/// large supplied JSON dataset.
/// </summary>
internal static class TestRecipes
{
    public static Recipe Curry() => new()
    {
        Id = 1,
        Title = "Chicken Curry",
        Ingredients = new List<string> { "500g chicken", "2 tbsp curry paste" },
        Instructions = new List<string>
        {
            "Brown the chicken",
            "Stir in the curry paste",
            "Simmer for 20 minutes"
        }
    };

    public static Recipe Soup() => new()
    {
        Id = 2,
        Title = "Vegetable Soup",
        Ingredients = new List<string> { "1 onion", "2 carrots" },
        Instructions = new List<string> { "Chop the vegetables", "Boil for 15 minutes" }
    };

    // Deliberately has no instructions, for the "cannot start cooking" case.
    public static Recipe Toast() => new()
    {
        Id = 3,
        Title = "Plain Toast",
        Ingredients = new List<string> { "2 slices of bread" }
    };

    public static RecipeManager CreateManager() =>
        new(new[] { Curry(), Soup(), Toast() });

    public static RecipeManager CreateEmptyManager() =>
        new(new Recipe[0]);
}
