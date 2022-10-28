package Tailwind.Traders.Stock.Api.config;

import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;

import com.amazonaws.auth.AWSStaticCredentialsProvider;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.services.dynamodbv2.AmazonDynamoDB;
import com.amazonaws.services.dynamodbv2.AmazonDynamoDBClientBuilder;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapper;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapperConfig;
import com.amazonaws.services.dynamodbv2.model.CreateTableRequest;
import com.amazonaws.services.dynamodbv2.model.ProvisionedThroughput;
import com.amazonaws.services.dynamodbv2.util.TableUtils;

import Tailwind.Traders.Stock.Api.models.StockItem;

@Configuration
public class DynamoDBConfiguration {

	/// @Value("${aws.dynamodb.endpoint}")
	/// private String dynamodbEndpoint;

	@Value("${aws.region}")
	private String awsRegion;

	@Value("${aws.dynamodb.accessKey}")
	private String dynamodbAccessKey;

	@Value("${aws.dynamodb.secretKey}")
	private String dynamodbSecretKey;

	@Value("${aws.dynamodb.stockItemTableName}")
	private String stockItemTable;

	public DynamoDBMapper buildAmazonDynamoDB() {
		AmazonDynamoDBClientBuilder builder = AmazonDynamoDBClientBuilder
				.standard();
		if (!StringUtils.isEmpty(dynamodbAccessKey)) {
			builder = builder
				.withCredentials(
					new AWSStaticCredentialsProvider(new BasicAWSCredentials(dynamodbAccessKey, dynamodbSecretKey)))
				.withRegion(awsRegion);
		}
		AmazonDynamoDB client = builder.build();
		DynamoDBMapperConfig mapperConfig = DynamoDBMapperConfig.builder()
				.withTableNameOverride(DynamoDBMapperConfig.TableNameOverride.withTableNameReplacement(stockItemTable))
				.build();

		DynamoDBMapper dynamoDBMapper = new DynamoDBMapper(client, mapperConfig);
		init(dynamoDBMapper, client);
		return dynamoDBMapper;
	}

	public void init(DynamoDBMapper dynamoDBMapper, AmazonDynamoDB client) {

		CreateTableRequest tableRequest = dynamoDBMapper.generateCreateTableRequest(StockItem.class);
		tableRequest.setProvisionedThroughput(new ProvisionedThroughput(1L, 1L));

		if (TableUtils.createTableIfNotExists(client, tableRequest)) {
			System.out.println("Table created");
		} else {
			System.out.println("Table found");
		}

	}
}
