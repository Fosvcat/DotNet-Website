namespace Geekspace.ViewModels;

public sealed record ResourceDownload(
    string Title,
    string Description,
    string Url,
    string Format);

public static class ResourceReadingExtras
{
    private static readonly IReadOnlyDictionary<string, ResourceDownload[]> Downloads =
        new Dictionary<string, ResourceDownload[]>(StringComparer.Ordinal)
        {
            ["Python Scripting for Security Automation"] =
            [
                new(
                    "Security log parser starter",
                    "A safe Python starter script for the article's JSON log exercise.",
                    "/media/files/security-log-parser.py",
                    "PY · 1.5 KB")
            ],
            ["Understanding the CIA Triad"] =
            [
                new(
                    "CIA Triad review checklist",
                    "A compact checklist for translating risks into confidentiality, integrity and availability controls.",
                    "/media/files/cia-triad-review-checklist.md",
                    "MD · 1.2 KB")
            ],
            ["Simulation: Phishing Email Detection"] =
            [
                new(
                    "Phishing triage checklist",
                    "A printable pause, verify, report and contain workflow.",
                    "/media/files/phishing-triage-checklist.md",
                    "MD · 1.3 KB")
            ],
            ["TCP/IP Fundamentals Self-Assessment"] =
            [
                new(
                    "TCP/IP analysis cheat sheet",
                    "Layer mapping, handshake states and common troubleshooting questions.",
                    "/media/files/tcp-ip-analysis-cheatsheet.md",
                    "MD · 1.6 KB")
            ],
            ["What is CTF? An Introduction"] =
            [
                new(
                    "CTF investigation notes template",
                    "A reusable evidence, hypothesis and remediation worksheet.",
                    "/media/files/ctf-notes-template.md",
                    "MD · 1.1 KB")
            ],
            ["Virtual Lab: Setting Up a Home Pentest Environment"] =
            [
                new(
                    "Home lab safety checklist",
                    "A pre-flight checklist for isolation, snapshots and test data.",
                    "/media/files/home-lab-safety-checklist.md",
                    "MD · 1.4 KB")
            ]
        };

    public static IReadOnlyList<ResourceDownload> For(string title) =>
        Downloads.TryGetValue(title, out var downloads) ? downloads : [];
}
