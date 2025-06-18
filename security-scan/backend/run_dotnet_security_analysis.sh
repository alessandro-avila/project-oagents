#!/bin/bash
 cd $(dirname $0)
# Run dotnet security analyzers on backend solution and generate remediation report

# Path to backend solution or csproj file - adjust as needed
BACKEND_PATH="../../backend/MyApp.sln"

# Output report path
REPORT_DIR="../../reports/backend"
REPORT_SARIF="$REPORT_DIR/dotnet_security_report.sarif"
REPORT_TXT="$REPORT_DIR/dotnet_security_report.txt"

mkdir -p "$REPORT_DIR"

# Restore tools and dependencies first
dotnet restore "$BACKEND_PATH"

# Run dotnet build with analyzers enabled and output in SARIF format
dotnet build "$BACKEND_PATH" /p:RunAnalyzers=true /p:ReportAnalyzer=true /p:ErrorLog="$REPORT_SARIF"

# Convert SARIF to human-readable format using sarif-viewer tool (if installed), else fallback to basic text extraction
if command -v sarif-viewer >/dev/null 2>&1; then
    sarif-viewer "$REPORT_SARIF" -o "$REPORT_TXT"
else
    # Simple extraction of ruleId and messages from SARIF JSON (using jq)
    if command -v jq >/dev/null 2>&1; then
        jq -r '.runs[].results[] | "Rule: \(.ruleId)\nMessage: \(.message.text)\nLocation: \(.locations[0].physicalLocation.artifactLocation.uri):\(.locations[0].physicalLocation.region.startLine)\n---"' "$REPORT_SARIF" > "$REPORT_TXT"
    else
        echo "SARIF report generated at $REPORT_SARIF. Install 'sarif-viewer' or 'jq' to convert to text report." > "$REPORT_TXT"
    fi
fi
