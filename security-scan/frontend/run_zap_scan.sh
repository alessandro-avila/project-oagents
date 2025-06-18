#!/bin/bash
 cd $(dirname $0)
# Run OWASP ZAP baseline scan on the frontend web application and generate an HTML report

# URL of the frontend app to scan - adjust as needed
TARGET_URL="http://localhost:3000"

# Output report path
REPORT_DIR="../../reports/frontend"
REPORT_HTML="$REPORT_DIR/owasp_zap_report.html"

mkdir -p "$REPORT_DIR"

# Run OWASP ZAP baseline scan with report output
docker run -t owasp/zap2docker-stable zap-baseline.py -t "$TARGET_URL" -r "$(basename "$REPORT_HTML")" -d

# Move the generated report out of the container volume mount (zap-baseline.py outputs report in current dir)
# We run the container with current dir mounted to /zap/wrk so report writes there
docker run --rm -v "$(pwd)":/zap/wrk owasp/zap2docker-stable zap-baseline.py -t "$TARGET_URL" -r "$(basename "$REPORT_HTML")" -d

mv "$(basename "$REPORT_HTML")" "$REPORT_HTML"
