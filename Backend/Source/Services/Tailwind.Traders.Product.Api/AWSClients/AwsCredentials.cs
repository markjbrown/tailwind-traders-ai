using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.Product.Api.AWSClients
{
	public class AwsCredentials : AWSCredentials
	{
		private readonly AppSettings _appConfig;

		public AwsCredentials(AppSettings appConfig)
		{
			_appConfig = appConfig;
		}

		public override ImmutableCredentials GetCredentials()
		{
			return new ImmutableCredentials(_appConfig.AwsAccessKey,
							_appConfig.AwsSecretKey, null);
		}
	}
}
