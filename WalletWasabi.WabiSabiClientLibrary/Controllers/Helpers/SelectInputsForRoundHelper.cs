using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NBitcoin;
using WalletWasabi.Blockchain.TransactionOutputs;
using WalletWasabi.Crypto.Randomness;
using WalletWasabi.WabiSabi.Backend.Rounds;
using WalletWasabi.WabiSabi.Client;
using WalletWasabi.WabiSabiClientLibrary.Models;
using WalletWasabi.WabiSabiClientLibrary.Models.SelectInputsForRound;

namespace WalletWasabi.WabiSabiClientLibrary.Controllers.Helpers;

public class SelectInputsForRoundHelper
{
	public static SelectInputsForRoundResponse SelectInputsForRound(SelectInputsForRoundRequest request, WasabiRandom? rnd = null)
	{
		rnd ??= SecureRandom.Instance;
		UtxoSelectionParameters utxoSelectionParameters = new(request.AllowedInputAmounts, request.AllowedOutputAmounts, request.CoordinationFeeRate, request.MiningFeeRate, request.AllowedInputTypes);
		ImmutableList<Utxo> coins = CoinJoinClient.SelectCoinsForRound<Utxo>(request.Utxos, utxoSelectionParameters, request.ConsolidationMode, request.AnonScoreTarget, request.SemiPrivateThreshold, request.LiquidityClue, rnd);

		Dictionary<ISmartCoin, int> coinIndices = request.Utxos
			.Select((x, i) => ((ISmartCoin)x, i))
			.ToDictionary(x => x.Item1, x => x.Item2);

		// Find corresponding indices for the found coins.
		int[] indices = coins.Select(c => coinIndices[c]).ToArray();

		return new SelectInputsForRoundResponse(indices);
	}
}
