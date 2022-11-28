using WalletWasabi.Crypto;

namespace WalletWasabi.WabiSabiClientLibrary.Models;

public record GetZeroCredentialRequestsRequest(
	long MaxAmountCredentialValue,
	CredentialIssuerParameters CredentialIssuerParameters
);
