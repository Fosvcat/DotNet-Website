namespace Geekspace.Models;

// Standalone quiz content for the /Questions feature. Not database-backed
// for now — each quiz is identified by a plain integer id (1-6) used in
// the /Questions/{id} route. If this ever needs to be editable through
// the admin UI, this is the natural place to migrate into a real table.
public sealed record QuizOption(string Value, string Text);

public sealed record QuizQuestion(string Prompt, string CorrectValue, QuizOption[] Options);

public sealed record QuizContent(int Id, string Title, string Description, QuizQuestion[] Questions);

public static class QuestionsCatalog
{
    private static QuizOption Option(string value, string text) => new(value, text);

    private static QuizQuestion Question(string prompt, string correctValue, params QuizOption[] options) =>
        new(prompt, correctValue, options);

    private static readonly IReadOnlyList<QuizContent> Quizzes = new List<QuizContent>
    {
        new(
            Id: 1,
            Title: "Understanding the CIA Triad",
            Description: "Test your understanding of confidentiality, integrity, and availability — the three pillars behind every security decision.",
            Questions:
            [
                Question(
                    "Payroll records are visible only to authorized HR staff. Which objective is being protected most directly?",
                    "confidentiality",
                    Option("availability", "Availability"),
                    Option("confidentiality", "Confidentiality"),
                    Option("integrity", "Integrity")),
                Question(
                    "Which control most directly detects an unauthorized change to a critical binary?",
                    "hash",
                    Option("backup", "A weekly availability backup"),
                    Option("hash", "A trusted cryptographic hash or signed-release comparison"),
                    Option("mfa", "Multi-factor authentication for a different application")),
                Question(
                    "A service uses redundant nodes and a tested failover procedure. Which objective is primarily improved?",
                    "availability",
                    Option("availability", "Availability"),
                    Option("confidentiality", "Confidentiality"),
                    Option("nonrepudiation", "Non-repudiation"))
            ]),

        new(
            Id: 2,
            Title: "Python Scripting for Security Automation",
            Description: "Apply the operating principles behind safe, auditable security automation scripts.",
            Questions:
            [
                Question(
                    "A scheduled script needs an API credential. Where should it be stored?",
                    "vault",
                    Option("source", "Directly in the source file"),
                    Option("vault", "In an approved secrets vault or protected environment variable"),
                    Option("log", "In the first line of the audit log")),
                Question(
                    "Which subprocess pattern best reduces shell-injection risk?",
                    "arguments",
                    Option("string", "Build one command string from user input"),
                    Option("arguments", "Pass a validated argument list with shell=False and a timeout"),
                    Option("retry", "Retry the same shell command until it succeeds")),
                Question(
                    "Why should the log parser return structured results instead of sending alerts itself?",
                    "separate",
                    Option("faster", "It guarantees every file will process faster"),
                    Option("smaller", "It makes the Python installation smaller"),
                    Option("separate", "It separates responsibilities so parsing and notification can be tested independently"))
            ]),

        new(
            Id: 3,
            Title: "Simulation: Phishing Email Detection",
            Description: "Practice separating sender identity, message intent, and destination before trusting a message.",
            Questions:
            [
                Question(
                    "A message shows your manager's name but comes from an unrelated domain. What is the safest first action?",
                    "verify",
                    Option("reply", "Reply and ask whether the message is genuine"),
                    Option("verify", "Verify through a previously trusted, out-of-band channel"),
                    Option("forward", "Forward it to a personal mailbox for inspection")),
                Question(
                    "An urgent login link displays the expected brand. Which detail matters most before any interaction?",
                    "hostname",
                    Option("logo", "Whether the logo looks sharp"),
                    Option("tone", "Whether the wording is polite"),
                    Option("hostname", "The actual destination hostname and approved sign-in path")),
                Question(
                    "A user entered credentials into a suspected phishing page. What response is strongest?",
                    "contain",
                    Option("delete", "Delete the message and take no further action"),
                    Option("contain", "Report immediately, reset the credential and revoke active sessions"),
                    Option("wait", "Wait to see whether the account behaves unusually"))
            ]),

        new(
            Id: 4,
            Title: "TCP/IP Fundamentals Self-Assessment",
            Description: "Check your understanding of the TCP/IP model, the three-way handshake, and network troubleshooting.",
            Questions:
            [
                Question(
                    "Which sequence represents a normal TCP three-way handshake?",
                    "handshake",
                    Option("handshake", "SYN → SYN-ACK → ACK"),
                    Option("reverse", "ACK → SYN → FIN"),
                    Option("udp", "DISCOVER → OFFER → REQUEST")),
                Question(
                    "Which TCP/IP layer is primarily responsible for routing packets between networks?",
                    "internet",
                    Option("link", "Link"),
                    Option("internet", "Internet"),
                    Option("application", "Application")),
                Question(
                    "A client receives a TCP reset immediately after SYN. What is the strongest initial interpretation?",
                    "closed",
                    Option("closed", "The host is reachable but the destination port is probably closed"),
                    Option("dns", "DNS has definitely failed"),
                    Option("success", "The application request completed successfully"))
            ]),

        new(
            Id: 5,
            Title: "What is CTF? An Introduction",
            Description: "Confirm you understand what makes security testing inside a Capture the Flag challenge appropriate — and productive.",
            Questions:
            [
                Question(
                    "What makes security testing inside a CTF appropriate?",
                    "scope",
                    Option("public", "The target is reachable from the internet"),
                    Option("scope", "The organizers provide explicit authorization and a defined scope"),
                    Option("flag", "A flag string exists somewhere on the target")),
                Question(
                    "What should happen before selecting an exploit technique?",
                    "observe",
                    Option("observe", "Read the rules and collect evidence about the target"),
                    Option("scan", "Run the largest possible scan immediately"),
                    Option("guess", "Guess common administrator passwords")),
                Question(
                    "Which habit produces the most transferable learning?",
                    "document",
                    Option("speed", "Submitting without understanding the root cause"),
                    Option("copy", "Copying commands without recording why they worked"),
                    Option("document", "Documenting evidence, reasoning, solution and remediation"))
            ]),

        new(
            Id: 6,
            Title: "Virtual Lab: Setting Up a Home Pentest Environment",
            Description: "Verify you know how to keep a home penetration-testing lab isolated and safely recoverable.",
            Questions:
            [
                Question(
                    "Which network mode is usually safest for an intentionally vulnerable target that only needs to reach the attacker VM?",
                    "hostonly",
                    Option("bridged", "Bridged directly to the household or corporate LAN"),
                    Option("hostonly", "A dedicated host-only or internal lab network"),
                    Option("public", "A public cloud network with an open security group")),
                Question(
                    "Why create a clean snapshot before an exercise?",
                    "restore",
                    Option("performance", "It always doubles virtual-machine performance"),
                    Option("restore", "It provides a known-good, repeatable recovery point"),
                    Option("internet", "It automatically grants internet access")),
                Question(
                    "Which host integration feature should remain disabled unless specifically required?",
                    "sharing",
                    Option("sharing", "Shared folders and bidirectional clipboard"),
                    Option("clock", "The virtual clock"),
                    Option("display", "Basic display output"))
            ])
    };

    public static IReadOnlyList<QuizContent> All => Quizzes;

    public static QuizContent? Get(int id) => Quizzes.FirstOrDefault(q => q.Id == id);
}
