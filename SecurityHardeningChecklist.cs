// SecurityHardeningChecklist.cs
// This file contains a checklist class with methods describing
// C# code changes and Azure configuration settings for security hardening,
// including HTTPS redirection, input validation middleware, and Azure Security Center integration.

using System;
using System.Collections.Generic;

namespace SecurityHardening
{
    public static class SecurityChecklist
    {
        // Returns checklist items for C# code changes
        public static List<string> GetCodeChangesChecklist()
        {
            return new List<string>
            {
                // Enforce HTTPS by redirecting HTTP requests to HTTPS
                "Enable HTTPS redirection in Startup.cs using app.UseHttpsRedirection();",

                // Implement input validation middleware to sanitize and validate all incoming requests
                "Create custom middleware to validate user inputs and reject malformed requests",

                // Use built-in ASP.NET Core Data Annotations and FluentValidation packages for model validation
                "Apply model validation attributes and handle validation errors gracefully",

                // Configure HSTS to instruct browsers to use HTTPS only
                "Add app.UseHsts() in production environment to enable HTTP Strict Transport Security",

                // Limit request body size to mitigate DoS attacks
                "Configure MaxRequestBodySize in Kestrel or IIS settings",

                // Use HTTPS-only cookies and set secure cookie flags
                "Set Cookie.SecurePolicy = CookieSecurePolicy.Always and HttpOnly to true",

                // Enable Content Security Policy headers to reduce XSS risks
                "Add middleware to include CSP headers in HTTP responses",

                // Validate anti-forgery tokens on state changing requests to prevent CSRF
                "Use [ValidateAntiForgeryToken] attribute on POST actions"
            };
        }

        // Returns checklist items for Azure configuration settings
        public static List<string> GetAzureConfigChecklist()
        {
            return new List<string>
            {
                // Enable HTTPS only on Azure App Service by enforcing HTTPS via TLS/SSL settings
                "Configure TLS/SSL bindings and enforce HTTPS-only in Azure Portal",

                // Integrate Azure Security Center to monitor security posture and get recommendations
                "Enable Azure Security Center Standard tier for continuous security assessment",

                // Use Azure Application Gateway with Web Application Firewall (WAF) policies
                "Deploy Application Gateway with WAF to protect against common web vulnerabilities",

                // Enable Azure Defender features for App Services to detect threats

                "Activate Microsoft Defender for App Service in Azure Security Center",

                // Use managed identities for Azure resources to avoid secret management

                "Assign managed identities and use them in app configurations instead of storing secrets",

                // Enable diagnostic logging and send logs to Azure Monitor or Log Analytics workspace

                "Set up App Service diagnostics and integrate with Azure Monitor logs",

                // Configure Azure Key Vault to store sensitive configuration like connection strings and certificates

                "Use Azure Key Vault references in App Service settings for secret management"
            };
        }
    }
}
