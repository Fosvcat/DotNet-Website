"""Safe starter for summarizing failed login events from JSON Lines logs."""

from collections import Counter
from pathlib import Path
import json


def summarize_failed_logins(log_path: Path) -> Counter[str]:
    counts: Counter[str] = Counter()

    with log_path.open(encoding="utf-8") as stream:
        for line_number, line in enumerate(stream, start=1):
            try:
                event = json.loads(line)
            except json.JSONDecodeError as error:
                raise ValueError(f"Invalid JSON on line {line_number}") from error

            if event.get("result") == "failed":
                counts[str(event.get("source_ip", "unknown"))] += 1

    return counts


if __name__ == "__main__":
    print("Import summarize_failed_logins() from your authorized lab workflow.")
