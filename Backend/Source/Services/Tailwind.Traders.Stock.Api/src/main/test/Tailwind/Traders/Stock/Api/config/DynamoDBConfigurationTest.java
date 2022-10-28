package Tailwind.Traders.Stock.Api.config;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class DynamoDBConfigurationTest {

    @Test
    void buildAmazonDynamoDB() {
        DynamoDBConfiguration config = new DynamoDBConfiguration();
        config.buildAmazonDynamoDB();
    }
}