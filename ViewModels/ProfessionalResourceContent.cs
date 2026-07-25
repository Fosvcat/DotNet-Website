namespace Geekspace.ViewModels;

public sealed record ProfessionalResourceCard(
    string Marker,
    string Title,
    string Body);

public sealed record ProfessionalResourceStep(
    string Title,
    string Body);

public sealed record ProfessionalResourceSection(
    string Eyebrow,
    string Title,
    string Introduction,
    ProfessionalResourceCard[] Cards,
    ProfessionalResourceStep[]? Steps = null,
    string? CalloutTitle = null,
    string? CalloutBody = null);

public sealed record ProfessionalQuizOption(
    string Value,
    string Text);

public sealed record ProfessionalQuizQuestion(
    string Prompt,
    string CorrectValue,
    ProfessionalQuizOption[] Options);

public sealed record ProfessionalResourceContent(
    string Slug,
    string FieldGuide,
    string ArticleCode,
    string ReadingTime,
    string Level,
    string ImageAlt,
    string VisualCaption,
    string FallbackVisualCode,
    string FallbackVisualTitle,
    string[] FallbackVisualLabels,
    ProfessionalResourceSection[] Sections,
    ProfessionalQuizQuestion[] Questions);

public static class ProfessionalResourceCatalog
{
    private static ProfessionalResourceCard Card(string marker, string title, string body) =>
        new(marker, title, body);

    private static ProfessionalResourceStep Step(string title, string body) =>
        new(title, body);

    private static ProfessionalQuizOption Option(string value, string text) =>
        new(value, text);

    private static ProfessionalQuizQuestion Question(
        string prompt,
        string correctValue,
        params ProfessionalQuizOption[] options) =>
        new(prompt, correctValue, options);

    private static readonly IReadOnlyDictionary<string, ProfessionalResourceContent> Resources =
        new Dictionary<string, ProfessionalResourceContent>(StringComparer.Ordinal)
        {
            ["Understanding the CIA Triad"] = new(
                Slug: "cia-triad",
                FieldGuide: "Security Foundations Field Guide",
                ArticleCode: "Article 02",
                ReadingTime: "10 min read",
                Level: "Foundation",
                ImageAlt: "Confidentiality, integrity and availability security model",
                VisualCaption: "The CIA Triad turns business requirements into three measurable security objectives.",
                FallbackVisualCode: "C · I · A",
                FallbackVisualTitle: "Three objectives. One balanced security model.",
                FallbackVisualLabels: ["Confidentiality", "Integrity", "Availability"],
                Sections:
                [
                    new(
                        Eyebrow: "Core model",
                        Title: "Three objectives behind every security decision",
                        Introduction:
                            "The CIA Triad is a compact way to ask whether information is protected from unauthorized access, unwanted change and unacceptable interruption. Strong designs consider all three objectives instead of treating security as a single control.",
                        Cards:
                        [
                            Card("C", "Confidentiality", "Ensure information is available only to authorized people, processes and systems through identity, access control and encryption."),
                            Card("I", "Integrity", "Preserve accuracy and trustworthiness by preventing or detecting unauthorized modification with validation, hashes and audit trails."),
                            Card("A", "Availability", "Keep services and data usable when required through redundancy, monitoring, capacity planning and tested recovery.")
                        ]),
                    new(
                        Eyebrow: "Control selection",
                        Title: "Translate objectives into layered controls",
                        Introduction:
                            "A control is useful only when it addresses a defined risk. Begin with the asset and its failure impact, then select preventive, detective and recovery measures that reinforce one another.",
                        Cards:
                        [
                            Card("IAM", "Least privilege", "Grant only the access needed for a role and review permissions regularly to reduce confidentiality exposure."),
                            Card("HASH", "Change verification", "Use signed releases, file-integrity monitoring and immutable logs to make unauthorized changes visible."),
                            Card("HA", "Resilience and recovery", "Combine redundancy with tested backups, recovery objectives and operational monitoring.")
                        ],
                        CalloutTitle: "Balance matters",
                        CalloutBody:
                            "A control can strengthen one objective while weakening another. Extremely restrictive access may protect confidentiality but harm availability during an incident, so document tradeoffs explicitly."),
                    new(
                        Eyebrow: "Decision workflow",
                        Title: "Assess a system with a repeatable sequence",
                        Introduction:
                            "Use the model as a practical review tool. The result should be a prioritized control decision, not merely three labels on a diagram.",
                        Cards: [],
                        Steps:
                        [
                            Step("Identify", "Name the information asset, owner and authorized users."),
                            Step("Measure", "Estimate the impact of disclosure, modification and outage."),
                            Step("Control", "Choose proportionate preventive, detective and recovery controls."),
                            Step("Verify", "Test access, integrity checks and recovery procedures with evidence.")
                        ],
                        CalloutTitle: "Review trigger",
                        CalloutBody:
                            "Repeat the assessment after architecture changes, new data classifications, major incidents or changes to business recovery requirements.")
                ],
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

            ["Simulation: Phishing Email Detection"] = new(
                Slug: "phishing-detection",
                FieldGuide: "Human-Layer Defense Field Guide",
                ArticleCode: "Simulation 06",
                ReadingTime: "12 min read",
                Level: "Foundation",
                ImageAlt: "Phishing email detection and response illustration",
                VisualCaption: "Effective phishing analysis separates sender identity, message intent and destination before any interaction.",
                FallbackVisualCode: "MAIL / VERIFY",
                FallbackVisualTitle: "Pause before trust becomes action.",
                FallbackVisualLabels: ["Identity", "Intent", "Destination"],
                Sections:
                [
                    new(
                        Eyebrow: "Detection model",
                        Title: "Inspect identity, intent and destination",
                        Introduction:
                            "Phishing succeeds by creating enough urgency or familiarity to bypass verification. Analyze the message as evidence: who actually sent it, what action it pressures you to take and where that action would lead.",
                        Cards:
                        [
                            Card("FROM", "Verify identity", "Compare the visible sender with the full domain, reply-to address and expected communication channel."),
                            Card("WHY", "Question the intent", "Treat unusual urgency, secrecy, payment changes and credential requests as signals that require confirmation."),
                            Card("URL", "Inspect the destination", "Check the real hostname and attachment type without opening or executing untrusted content.")
                        ]),
                    new(
                        Eyebrow: "Safe analysis",
                        Title: "Use a low-risk triage procedure",
                        Introduction:
                            "The goal is to collect enough evidence to decide and report without giving the message additional opportunities to execute code, capture credentials or confirm that an inbox is active.",
                        Cards:
                        [
                            Card("STOP", "Do not interact", "Avoid replying, enabling macros, opening unexpected attachments or using embedded sign-in links."),
                            Card("OOB", "Confirm out of band", "Contact the supposed sender using a known number or previously trusted conversation, not details from the message."),
                            Card("SOC", "Report through policy", "Use the organization’s reporting workflow so headers, URLs and attachments can be analyzed centrally.")
                        ],
                        CalloutTitle: "Display names are not identity",
                        CalloutBody:
                            "A familiar display name can be paired with an unrelated domain. Make the full sender address and destination hostname part of every review."),
                    new(
                        Eyebrow: "Response workflow",
                        Title: "Contain quickly when interaction has occurred",
                        Introduction:
                            "If a user clicked, opened a file or submitted credentials, treat speed and accurate reporting as more important than embarrassment. Early containment reduces the attacker’s usable time.",
                        Cards: [],
                        Steps:
                        [
                            Step("Disconnect", "Stop further interaction and isolate a device if policy requires it."),
                            Step("Report", "Tell the security team exactly what was clicked, opened or entered."),
                            Step("Protect", "Reset exposed credentials, revoke sessions and apply MFA controls."),
                            Step("Preserve", "Retain the original message and timeline for investigation.")
                        ],
                        CalloutTitle: "Simulation mindset",
                        CalloutBody:
                            "A good outcome is a repeatable habit: pause, verify through a trusted channel and report. Memorizing a single phishing template is not enough.")
                ],
                Questions:
                [
                    Question(
                        "A message shows your manager’s name but comes from an unrelated domain. What is the safest first action?",
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

            ["TCP/IP Fundamentals Self-Assessment"] = new(
                Slug: "tcp-ip-fundamentals",
                FieldGuide: "Network Analysis Field Guide",
                ArticleCode: "Assessment 03",
                ReadingTime: "14 min read",
                Level: "Intermediate",
                ImageAlt: "TCP IP network communication layers and packet flow",
                VisualCaption: "Network troubleshooting becomes easier when each observation is mapped to a layer, protocol and expected state transition.",
                FallbackVisualCode: "SYN → SYN-ACK → ACK",
                FallbackVisualTitle: "Follow the packet, then verify the state.",
                FallbackVisualLabels: ["Application", "Transport", "Internet", "Link"],
                Sections:
                [
                    new(
                        Eyebrow: "Layer model",
                        Title: "Map network behavior to four practical layers",
                        Introduction:
                            "The TCP/IP model separates responsibilities so analysts can ask focused questions. A failed web request may originate in name resolution, routing, transport establishment, TLS or the application itself.",
                        Cards:
                        [
                            Card("APP", "Application", "Protocols such as DNS, HTTP and SSH define how user-facing services exchange meaningful data."),
                            Card("TCP", "Transport", "TCP provides ordered, stateful delivery; UDP favors low overhead without connection establishment."),
                            Card("IP", "Internet", "IP addressing and routing move packets between networks without guaranteeing delivery."),
                            Card("L2", "Link", "Ethernet, Wi-Fi and ARP support local delivery on the current network segment.")
                        ]),
                    new(
                        Eyebrow: "Transport state",
                        Title: "Read the TCP handshake as evidence",
                        Introduction:
                            "A normal connection begins with SYN, SYN-ACK and ACK. The observed response helps distinguish a listening service, a closed port, a filtered path and an application-level failure.",
                        Cards:
                        [
                            Card("SYN", "Client initiates", "The client proposes a connection and an initial sequence number."),
                            Card("S/A", "Server acknowledges", "A listening server acknowledges the client and presents its own sequence number."),
                            Card("ACK", "State established", "The client acknowledges the server, after which application data can flow.")
                        ],
                        CalloutTitle: "Ports provide context, not proof",
                        CalloutBody:
                            "Common values such as 22, 53, 80 and 443 are useful hypotheses. Confirm the actual service and encryption state instead of trusting the port number alone."),
                    new(
                        Eyebrow: "Analysis workflow",
                        Title: "Troubleshoot from observable dependencies",
                        Introduction:
                            "Move through the connection in order and record the first failed expectation. This prevents application symptoms from being misdiagnosed as generic network outages.",
                        Cards: [],
                        Steps:
                        [
                            Step("Resolve", "Confirm DNS returns the expected address and record type."),
                            Step("Reach", "Check route selection and whether the destination is reachable."),
                            Step("Connect", "Inspect the TCP handshake, flags, retransmissions and resets."),
                            Step("Validate", "Confirm TLS identity and the application response.")
                        ],
                        CalloutTitle: "Capture responsibly",
                        CalloutBody:
                            "Packet captures can contain credentials, tokens and personal data. Collect only within approved scope and protect capture files as sensitive evidence.")
                ],
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

            ["What is CTF? An Introduction"] = new(
                Slug: "ctf-introduction",
                FieldGuide: "CTF Starter Field Guide",
                ArticleCode: "Video 01",
                ReadingTime: "11 min read",
                Level: "Beginner",
                ImageAlt: "Capture the Flag cybersecurity introduction",
                VisualCaption: "CTFs turn security concepts into legal, scoped puzzles where evidence and methodology matter more than guessing.",
                FallbackVisualCode: "flag{learn_by_doing}",
                FallbackVisualTitle: "A controlled environment for practical security learning.",
                FallbackVisualLabels: ["Web", "Crypto", "Forensics"],
                Sections:
                [
                    new(
                        Eyebrow: "Learning format",
                        Title: "Understand what a CTF challenge is",
                        Introduction:
                            "Capture the Flag competitions are intentionally vulnerable, authorized environments. Participants analyze a challenge, exploit or solve the intended weakness and submit a hidden flag as proof.",
                        Cards:
                        [
                            Card("WEB", "Web exploitation", "Reason about requests, sessions, input handling and server-side behavior in a sandbox."),
                            Card("CRY", "Cryptography", "Identify encoding, weak constructions and implementation mistakes before attempting decryption."),
                            Card("FOR", "Forensics", "Recover evidence from files, metadata, memory, network captures and event timelines."),
                            Card("REV", "Reverse engineering", "Study program behavior and data flow to understand how a binary validates or transforms input.")
                        ]),
                    new(
                        Eyebrow: "Working method",
                        Title: "Replace random attempts with an evidence loop",
                        Introduction:
                            "Strong CTF players keep notes, form small hypotheses and test one assumption at a time. This makes progress reproducible and turns every failed attempt into information.",
                        Cards:
                        [
                            Card("READ", "Read the scope", "Record the provided files, endpoints, rules and flag format before using tools."),
                            Card("OBS", "Enumerate carefully", "Collect visible technologies, metadata and behavior without jumping to an exploit."),
                            Card("NOTE", "Document evidence", "Save commands, outputs and reasoning so the solution can be explained and repeated.")
                        ],
                        CalloutTitle: "Authorization is part of the skill",
                        CalloutBody:
                            "Techniques learned in a CTF belong inside the challenge scope. Never test unrelated systems or reuse a competition target after authorization ends."),
                    new(
                        Eyebrow: "Challenge workflow",
                        Title: "Move from prompt to documented solution",
                        Introduction:
                            "Use a compact loop that keeps the challenge goal visible while leaving room to change direction when evidence contradicts an assumption.",
                        Cards: [],
                        Steps:
                        [
                            Step("Scope", "Read the rules, target boundaries and expected flag format."),
                            Step("Observe", "Enumerate files, services and responses with low-risk checks."),
                            Step("Test", "Form one hypothesis and validate it with the smallest useful experiment."),
                            Step("Explain", "Capture the flag, then write the root cause and remediation.")
                        ],
                        CalloutTitle: "When stuck",
                        CalloutBody:
                            "Return to raw evidence, list assumptions and try to disprove one. A short break and a clean set of notes are often more valuable than another tool.")
                ],
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

            ["Virtual Lab: Setting Up a Home Pentest Environment"] = new(
                Slug: "home-pentest-lab",
                FieldGuide: "Isolated Lab Field Guide",
                ArticleCode: "Lab 04",
                ReadingTime: "16 min read",
                Level: "Intermediate",
                ImageAlt: "Isolated home penetration testing lab architecture",
                VisualCaption: "A safe practice lab separates intentionally vulnerable targets from household, corporate and public networks.",
                FallbackVisualCode: "ATTACKER ⇄ HOST-ONLY ⇄ TARGET",
                FallbackVisualTitle: "Isolation first. Snapshots before experiments.",
                FallbackVisualLabels: ["Attacker VM", "Host-only network", "Target VM"],
                Sections:
                [
                    new(
                        Eyebrow: "Lab architecture",
                        Title: "Design isolation before installing tools",
                        Introduction:
                            "A home pentest lab should make accidental contact with real systems difficult. Use dedicated virtual machines, an internal or host-only network and deliberately vulnerable targets obtained from trusted training sources.",
                        Cards:
                        [
                            Card("ATK", "Attacker VM", "Keep assessment tools in a dedicated machine with controlled updates and no sensitive personal data."),
                            Card("NET", "Host-only segment", "Allow lab machines to communicate with one another without exposing the target to the physical LAN."),
                            Card("TGT", "Training target", "Use intentionally vulnerable images whose license and scope explicitly permit testing.")
                        ]),
                    new(
                        Eyebrow: "Safety controls",
                        Title: "Create recoverable boundaries",
                        Introduction:
                            "Isolation is not a single checkbox. Confirm adapter modes, disable unnecessary sharing and maintain clean snapshots so every exercise begins from a known state.",
                        Cards:
                        [
                            Card("NIC", "Verify every adapter", "Remove bridged adapters from vulnerable targets and document the expected lab subnet."),
                            Card("SNAP", "Snapshot clean states", "Capture a baseline after installation and before major exercises for fast, reliable recovery."),
                            Card("SHARE", "Limit host integration", "Disable shared clipboard, drag-and-drop and shared folders unless an exercise requires them.")
                        ],
                        CalloutTitle: "Do not trust default network settings",
                        CalloutBody:
                            "Virtualization products can retain adapter settings when machines are cloned or imported. Verify addressing and routes from inside every VM before testing."),
                    new(
                        Eyebrow: "Build workflow",
                        Title: "Provision and validate the lab in stages",
                        Introduction:
                            "Treat the environment as a small managed system. Keep a written inventory, validate isolation and restore known-good states instead of allowing experiments to accumulate.",
                        Cards: [],
                        Steps:
                        [
                            Step("Provision", "Create attacker and target VMs from trusted installation media."),
                            Step("Isolate", "Attach them to a dedicated host-only or internal network."),
                            Step("Validate", "Test lab connectivity and confirm the physical LAN is unreachable."),
                            Step("Snapshot", "Save a named clean baseline before beginning exercises.")
                        ],
                        CalloutTitle: "Evidence hygiene",
                        CalloutBody:
                            "Use fictional accounts and synthetic data in the lab. Never import production credentials, customer information or private keys into a vulnerable target.")
                ],
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

    public static bool Contains(string title) => Resources.ContainsKey(title);

    public static ProfessionalResourceContent Get(string title) => Resources[title];
}
