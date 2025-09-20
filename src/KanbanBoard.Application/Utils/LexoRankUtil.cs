using LexoAlgorithm;

namespace KanbanBoard.Application.Utils;

public static class LexoRankUtil
{
    /// Generates a new LexoRank for creating a task at the end of a column.
    /// If the column is empty, it uses the middle rank as the initial value.
    public static string GetLexoRankIdForCreation(string? latestLexoRankId)
    {
        if (string.IsNullOrWhiteSpace(latestLexoRankId))
        {
            // Column is empty, create the first task in the middle of the rank space
            return LexoRank.Middle().ToString();
        }

        // Column has tasks, create a new task after the last one
        var lastRank = LexoRank.Parse(latestLexoRankId);
        return lastRank.GenNext().ToString();
    }

    /// Generates a new LexoRank for moving a task within a column.
    /// Handles moving to the top, bottom, between tasks, or into an empty column.
    public static string GetLexoRankIdForMove(string? prevLexoRankId, string? nextLexoRankId)
    {
        // Column is empty → first task
        if (string.IsNullOrWhiteSpace(prevLexoRankId) && string.IsNullOrWhiteSpace(nextLexoRankId))
        {
            return LexoRank.Middle().ToString();
        }
        else if (string.IsNullOrWhiteSpace(prevLexoRankId))
        {
            // Moving to the top of the column (before the first task)
            var nextRank = LexoRank.Parse(nextLexoRankId!);
            return nextRank.GenPrev().ToString();
        }
        else if (string.IsNullOrWhiteSpace(nextLexoRankId))
        {
            // Moving to the end of the column (after the last task)
            var prevRank = LexoRank.Parse(prevLexoRankId);
            return prevRank.GenNext().ToString();
        }
        else
        {
            // Moving between two tasks
            var prevRank = LexoRank.Parse(prevLexoRankId);
            var nextRank = LexoRank.Parse(nextLexoRankId);
            return prevRank.Between(nextRank).ToString();
        }
    }
}


