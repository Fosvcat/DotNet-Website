namespace Geekspace.ViewModels;

public sealed record MarkdownHeadingViewModel(
    string Id,
    string Text,
    int Level);

public sealed record MarkdownArticleViewModel(
    string Html,
    IReadOnlyList<MarkdownHeadingViewModel> Headings,
    int ReadingMinutes,
    int WordCount);
