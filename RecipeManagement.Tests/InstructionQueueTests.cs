using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// A4 - the instruction queue: FIFO loading, peek, completion and empty cases.
/// </summary>
public sealed class InstructionQueueTests
{
    [Fact]
    public void StartCooking_LoadsInstructionsInTheirOriginalOrder()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.True(manager.StartCooking(1));

        Assert.Equal(3, manager.PendingInstructionCount);
        Assert.Equal("Brown the chicken", manager.PeekNextInstruction());
    }

    [Fact]
    public void PeekNextInstruction_DoesNotRemoveTheInstruction()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.StartCooking(2);

        manager.PeekNextInstruction();
        manager.PeekNextInstruction();

        Assert.Equal(2, manager.PendingInstructionCount);
    }

    [Fact]
    public void CompleteNextInstruction_RemovesExactlyOneInstructionInFifoOrder()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.StartCooking(1);

        Assert.Equal("Brown the chicken", manager.CompleteNextInstruction());
        Assert.Equal("Stir in the curry paste", manager.CompleteNextInstruction());
        Assert.Equal(1, manager.PendingInstructionCount);
        Assert.Equal("Simmer for 20 minutes", manager.PeekNextInstruction());
    }

    [Fact]
    public void EmptyQueue_PeekAndCompleteReturnNull()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void QueueBecomesEmpty_AfterEveryInstructionIsCompleted()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.StartCooking(2);

        manager.CompleteNextInstruction();
        manager.CompleteNextInstruction();

        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void StartCooking_ReturnsFalseForMissingRecipe()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.False(manager.StartCooking(999));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_ReturnsFalseWhenTheRecipeHasNoInstructions()
    {
        RecipeManager manager = TestRecipes.CreateManager();

        Assert.False(manager.StartCooking(3));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void FailedStart_KeepsThePreviousQueue()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.StartCooking(1);

        Assert.False(manager.StartCooking(3));

        Assert.Equal(3, manager.PendingInstructionCount);
        Assert.Equal("Brown the chicken", manager.PeekNextInstruction());
    }

    [Fact]
    public void StartCooking_ReplacesThePreviousQueue()
    {
        RecipeManager manager = TestRecipes.CreateManager();
        manager.StartCooking(1);
        manager.CompleteNextInstruction();

        Assert.True(manager.StartCooking(2));

        Assert.Equal(2, manager.PendingInstructionCount);
        Assert.Equal("Chop the vegetables", manager.PeekNextInstruction());
    }
}
