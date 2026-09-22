namespace ClubCordobaWallet.Domain.Enums;

// Serializa como integer (no string) en el JSON canónico, tal cual el enunciado:
// {"credentialStatus":0,...}
public enum CredentialStatus
{
    active = 0,
    revoked = 1,
    suspended = 2
}
