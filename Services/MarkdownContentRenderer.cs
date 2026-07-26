using System.Text;
using System.Text.RegularExpressions;
using Geekspace.ViewModels;
using Markdig;

namespace Geekspace.Services;

public sealed partial class MarkdownContentRenderer
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownContentRenderer()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .DisableHtml()
            .Build();
    }

    public MarkdownArticleViewModel Render(string? markdown)
    {
        var source = string.IsNullOrWhiteSpace(markdown)
            ? "_No article content has been published yet._"
            : markdown.Trim();

        var headings = new List<MarkdownHeadingViewModel>();
        var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var processed = new StringBuilder();

        foreach (var line in source.Replace("\r\n", "\n").Split('\n'))
        {
            var match = HeadingPattern().Match(line);
            if (!match.Success)
            {
                processed.AppendLine(line);
                continue;
            }

            var level = match.Groups[1].Value.Length;
            var headingMarkdown = match.Groups[2].Value.Trim();
            var headingText = PlainHeadingText(headingMarkdown);
            var id = UniqueSlug(headingText, headings.Count + 1, usedIds);

            headings.Add(new MarkdownHeadingViewModel(id, headingText, level));
            processed.AppendLine($"{match.Groups[1].Value} {headingMarkdown} {{#{id}}}");
        }

        var html = Markdown.ToHtml(processed.ToString(), _pipeline);
        var wordCount = WordPattern().Matches(source).Count;
        var readingMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 220d));

        return new MarkdownArticleViewModel(html, headings, readingMinutes, wordCount);
    }

    private static string PlainHeadingText(string markdown)
    {
        var text = MarkdownLinkPattern().Replace(markdown, "$1");
        text = InlineMarkerPattern().Replace(text, string.Empty);
        text = GenericAttributePattern().Replace(text, string.Empty);
        return string.IsNullOrWhiteSpace(text) ? "Untitled section" : text.Trim();
    }

    private static string UniqueSlug(string heading, int fallbackIndex, HashSet<string> usedIds)
    {
        var normalized = heading.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        var lastWasDash = false;

        foreach (var character in normalized)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
                lastWasDash = false;
            }
            else if (!lastWasDash && builder.Length > 0)
            {
                builder.Append('-');
                lastWasDash = true;
            }
        }

        var baseSlug = builder.ToString().Trim('-');
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = $"section-{fallbackIndex}";
        }

        var candidate = baseSlug;
        var suffix = 2;
        while (!usedIds.Add(candidate))
        {
            candidate = $"{baseSlug}-{suffix++}";
        }

        return candidate;
    }

    [GeneratedRegex(@"^(#{2,3})\s+(.+?)\s*$")]
    private static partial Regex HeadingPattern();

    [GeneratedRegex(@"\[([^\]]+)\]\([^)]+\)")]
    private static partial Regex MarkdownLinkPattern();

    [GeneratedRegex(@"[*_`~]")]
    private static partial Regex InlineMarkerPattern();

    [GeneratedRegex(@"\s*\{#[^}]+\}\s*$")]
    private static partial Regex GenericAttributePattern();

    [GeneratedRegex(@"[\p{L}\p{N}]+")]
    private static partial Regex WordPattern();
}
