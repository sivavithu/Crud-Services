# Security Policy

## Reporting Security Vulnerabilities

If you discover a security vulnerability within this project, please send an email to the repository owner. All security vulnerabilities will be promptly addressed.

## Secure Configuration

### Development Environment

This project has been configured to prevent accidental exposure of secrets:

1. **appsettings.Development.json** is gitignored to prevent committing local secrets
2. **User Secrets** are the recommended approach for local development (already configured)
3. **.env files** are gitignored to prevent environment variable leaks

### Secrets Management

**Never commit the following to source control:**
- JWT secret keys
- Database connection strings with credentials
- API keys or tokens
- Passwords or other sensitive data

**Recommended Approaches:**

#### For Development
- Use .NET User Secrets (already configured with UserSecretsId in .csproj)
- Use environment variables loaded from .env files (gitignored)
- Use local-only configuration files (appsettings.Development.json - gitignored)

#### For Production
- **Azure**: Use Azure Key Vault
- **AWS**: Use AWS Secrets Manager or Systems Manager Parameter Store
- **Docker**: Use Docker secrets or environment variables injected at runtime
- **Kubernetes**: Use Kubernetes Secrets

### Configuration Priority

.NET Core loads configuration in the following order (later sources override earlier ones):
1. appsettings.json
2. appsettings.{Environment}.json
3. User Secrets (Development only)
4. Environment variables
5. Command-line arguments

### Best Practices

1. **Rotate secrets regularly**: Change JWT keys, database passwords, and API keys periodically
2. **Use strong keys**: JWT keys should be at least 32 characters with high entropy
3. **Restrict access**: Limit who can access production secrets
4. **Audit access**: Log and monitor access to sensitive configuration
5. **Use HTTPS**: Always use HTTPS in production
6. **Validate tokens**: Ensure proper JWT validation with issuer, audience, and lifetime checks

### Example Files

- `.env.example` - Template for environment variables
- `appsettings.Development.json.example` - Template for local configuration

Copy these files and update with your actual values (the copies will be gitignored).

## Important Note About Git History

**If you previously committed secrets to this repository**, they still exist in the git history. Simply removing them from the current files is not sufficient. 

To completely remove secrets from git history, you need to:

1. **For Public Repositories**: Immediately rotate/change all exposed secrets (JWT keys, database passwords, API keys)
2. **Clean Git History**: Use tools like `git filter-branch`, `BFG Repo-Cleaner`, or `git filter-repo` to remove sensitive data from history
3. **Force Push**: After cleaning, force push the cleaned history (this is a destructive operation)

**Note**: Anyone who has already cloned the repository will still have the old history with secrets. Inform all collaborators to delete and re-clone the repository after cleaning.

### Recommended Immediate Actions if Secrets Were Exposed

1. ✅ Rotate all exposed JWT keys
2. ✅ Change all database passwords
3. ✅ Revoke and regenerate any API keys or tokens
4. ✅ Update all deployment configurations
5. ✅ Notify team members of the security incident
6. ✅ Monitor for any unauthorized access

